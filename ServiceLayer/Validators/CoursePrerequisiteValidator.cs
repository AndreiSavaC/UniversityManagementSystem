namespace ServiceLayer.Validators
{
    using FluentValidation;
    using DomainModel.Entities;
    using DataAccessLayer.Repositories.Interfaces;
    public class CoursePrerequisiteValidator : AbstractValidator<CoursePrerequisite>
    {
        private readonly ICoursePrerequisiteRepository _coursePrerequisiteRepository;
        private readonly ICourseRepository _courseRepository;

        public CoursePrerequisiteValidator(ICoursePrerequisiteRepository coursePrerequisiteRepository, ICourseRepository courseRepository)
        {
            _coursePrerequisiteRepository = coursePrerequisiteRepository;
            _courseRepository = courseRepository;

            RuleFor(cp => cp.Id)
                .NotEmpty().WithMessage("ID is required.")
                .Must(id => id > 0).WithMessage("ID must be a positive number.");

            RuleFor(cp => cp.CourseId)
                .NotEmpty().WithMessage("Course ID is required.")
                .GreaterThan(0).WithMessage("Course ID must be greater than 0.")
                .MustAsync(CourseExists).WithMessage("Course must exist.");

            RuleFor(cp => cp.PrereqId)
                .NotEmpty().WithMessage("Prerequisite Course ID is required.")
                .GreaterThan(0).WithMessage("Prerequisite Course ID must be greater than 0.")
                .MustAsync(CourseExists).WithMessage("Prerequisite Course must exist.");

            RuleFor(cp => cp)
                .Must(cp => cp.CourseId != cp.PrereqId)
                .WithMessage("Course cannot be a prerequisite of itself.");

            RuleFor(cp => cp)
                .MustAsync(CourseInSemesterAtLeastTwo)
                .WithMessage("Only courses from semester 2 or higher can have prerequisites.");

            RuleFor(cp => cp)
                .MustAsync(PrerequisiteInEarlierSemester)
                .WithMessage("Prerequisite course must be in an earlier semester than the course.");

            RuleFor(cp => cp)
                .MustAsync(NotCreateCircularDependency)
                .WithMessage("Adding this prerequisite would create a circular dependency.");
        }

        /// <summary>
        /// Checks if a course with the given ID exists in the repository.
        /// </summary>
        /// <param name="courseId">The ID of the course to check.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if the course exists; otherwise, false.</returns>
        private async Task<bool> CourseExists(int courseId, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            return course != null;
        }

        /// <summary>
        /// Ensures that the main course is offered in semester 2 or later.
        /// </summary>
        /// <param name="cp">The CoursePrerequisite instance being validated.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if the course is offered in semester 2 or later; otherwise, false.</returns>
        private async Task<bool> CourseInSemesterAtLeastTwo(CoursePrerequisite cp, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.GetByIdAsync(cp.CourseId);
            if (course == null)
                return false;

            var courseSemesters = course.CourseSemesters.Select(cs => cs.Semester.Number).ToList();
            if (courseSemesters.Count == 0)
                return false;

            return courseSemesters.All(s => s >= 2);
        }

        /// <summary>
        /// Ensures that the prerequisite course is offered in an earlier semester than the main course.
        /// </summary>
        /// <param name="cp">The CoursePrerequisite instance being validated.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if the prerequisite is in an earlier semester; otherwise, false.</returns>
        private async Task<bool> PrerequisiteInEarlierSemester(CoursePrerequisite cp, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.GetByIdAsync(cp.CourseId);
            var prereqCourse = await _courseRepository.GetByIdAsync(cp.PrereqId);

            if (course == null || prereqCourse == null)
                return false; 

            var courseSemesters = course.CourseSemesters.Select(cs => cs.Semester.Number).ToList();
            var prereqSemesters = prereqCourse.CourseSemesters.Select(cs => cs.Semester.Number).ToList();

            if (courseSemesters.Count == 0 || prereqSemesters.Count == 0)
                return false; 

            return prereqSemesters.Any(ps => courseSemesters.Any(cs => ps < cs));
        }

        /// <summary>
        /// Ensures that adding the new prerequisite does not create a circular dependency in the prerequisite graph.
        /// </summary>
        /// <param name="cp">The CoursePrerequisite instance being validated.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if no circular dependency is created; otherwise, false.</returns>
        private async Task<bool> NotCreateCircularDependency(CoursePrerequisite cp, CancellationToken cancellationToken)
        {
            var allPrereqs = await _coursePrerequisiteRepository.GetAllAsync();

            var prereqGraph = new Dictionary<int, List<int>>();

            foreach (var prereq in allPrereqs)
            {
                if (!prereqGraph.TryGetValue(prereq.CourseId, out var prereqList))
                {
                    prereqList = [];
                    prereqGraph[prereq.CourseId] = prereqList;
                }
                prereqList.Add(prereq.PrereqId);
            }

            if (!prereqGraph.TryGetValue(cp.CourseId, out var cpPrereqList))
            {
                cpPrereqList = [];
                prereqGraph[cp.CourseId] = cpPrereqList;
            }
            cpPrereqList.Add(cp.PrereqId);

            return !HasCycle(prereqGraph);
        }

        /// <summary>
        /// Determines whether the prerequisite graph contains any cycles.
        /// A cycle indicates a circular dependency.
        /// </summary>
        /// <param name="prereqGraph">The prerequisite graph represented as a dictionary.</param>
        /// <returns>True if a cycle is detected; otherwise, false.</returns>
        private static bool HasCycle(Dictionary<int, List<int>> prereqGraph)
        {
            var visited = new HashSet<int>(); 
            var recStack = new HashSet<int>();  

            foreach (var node in prereqGraph.Keys)
            {
                if (IsCyclicUtil(node, prereqGraph, visited, recStack))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Utility function to perform Depth-First Search (DFS) and detect cycles in the graph.
        /// </summary>
        /// <param name="node">The current node being visited.</param>
        /// <param name="prereqGraph">The prerequisite graph.</param>
        /// <param name="visited">Set of already visited nodes.</param>
        /// <param name="recStack">Set of nodes in the current recursion stack.</param>
        /// <returns>True if a cycle is detected; otherwise, false.</returns>
        private static bool IsCyclicUtil(int node, Dictionary<int, List<int>> prereqGraph, HashSet<int> visited, HashSet<int> recStack)
        {
            if (visited.Add(node))
            {
                recStack.Add(node);

                if (prereqGraph.TryGetValue(node, out var neighbors))
                {
                    foreach (var neighbor in neighbors)
                    {
                        if (!visited.Contains(neighbor) && IsCyclicUtil(neighbor, prereqGraph, visited, recStack))
                            return true;
                        else if (recStack.Contains(neighbor))
                            return true;
                    }
                }
            }
            recStack.Remove(node);
            return false;
        }
    }
}
