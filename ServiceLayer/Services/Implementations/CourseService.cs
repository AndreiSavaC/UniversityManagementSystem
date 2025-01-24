namespace ServiceLayer.Services.Implementations
{
    using DomainModel.Entities;
    using DataAccessLayer.Repositories.Interfaces;
    using FluentValidation;
    using FluentValidation.Results;
    using log4net;
    using ServiceLayer.Services.Interfaces;

    public class CourseService(ICourseRepository courseRepository, IValidator<Course> courseValidator) : ICourseService
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CourseService));
        private readonly ICourseRepository _courseRepository = courseRepository;
        private readonly IValidator<Course> _courseValidator = courseValidator;

        public async Task<Course> CreateCourseAsync(Course course)
        {
            ValidationResult validationResult = await _courseValidator.ValidateAsync(course);
            if (!validationResult.IsValid)
            {
                Logger.Warn("Course validation failed on create.");
                throw new ValidationException(validationResult.Errors);
            }

            await _courseRepository.AddAsync(course);
            Logger.Info($"Course created successfully: {course.Name}");
            return course;
        }

        public async Task<Course?> GetCourseByIdAsync(int courseId)
        {
            Logger.Info($"Fetching Course with ID = {courseId}");
            return await _courseRepository.GetByIdAsync(courseId);
        }

        public async Task<List<Course>> GetAllCoursesAsync()
        {
            Logger.Info("Fetching all courses...");
            return await _courseRepository.GetAllAsync();
        }

        public async Task UpdateCourseAsync(Course course)
        {
            ValidationResult validationResult = await _courseValidator.ValidateAsync(course);
            if (!validationResult.IsValid)
            {
                Logger.Warn("Course validation failed on update.");
                throw new ValidationException(validationResult.Errors);
            }

            await _courseRepository.UpdateAsync(course);
            Logger.Info($"Course updated successfully: {course.Name}");
        }

        public async Task DeleteCourseAsync(int courseId)
        {
            Logger.Info($"Deleting Course with ID = {courseId}");
            await _courseRepository.DeleteAsync(courseId);
        }
    }
}
