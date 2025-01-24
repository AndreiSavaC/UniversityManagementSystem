namespace ServiceLayer.Validators
{
    using FluentValidation;
    using DomainModel.Entities;
    using DataAccessLayer.Repositories.Interfaces;
    public class CourseSemesterValidator : AbstractValidator<CourseSemester>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ISemesterRepository _semesterRepository;

        public CourseSemesterValidator(
            ICourseRepository courseRepository,
            ISemesterRepository semesterRepository)
        {
            _courseRepository = courseRepository;
            _semesterRepository = semesterRepository;
            RuleFor(s => s.Id)
               .NotEmpty().WithMessage("ID is required.")
               .Must(id => id > 0).WithMessage("ID must be a positive number.");

            RuleFor(cs => cs.CourseId)
                .GreaterThan(0).WithMessage("CourseId must be greater than 0.")
                .MustAsync(CourseExists).WithMessage("Course does not exist.");

            RuleFor(cs => cs.SemesterId)
                .GreaterThan(0).WithMessage("SemesterId must be greater than 0.")
                .MustAsync(SemesterExists).WithMessage("Semester does not exist.");
        }

        private async Task<bool> CourseExists(int courseId, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            return course != null;
        }

        private async Task<bool> SemesterExists(int semesterId, CancellationToken cancellationToken)
        {
            var semester = await _semesterRepository.GetByIdAsync(semesterId);
            return semester != null;
        }
    }
}
