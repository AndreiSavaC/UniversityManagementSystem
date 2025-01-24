namespace ServiceLayer.Services.Implementations
{
    using DomainModel.Entities;
    using DataAccessLayer.Repositories.Interfaces;
    using ServiceLayer.Services.Interfaces;
    using FluentValidation;
    using FluentValidation.Results;
    using log4net;

    public class CoursePrerequisiteService(
        ICoursePrerequisiteRepository coursePrerequisiteRepository,
        ICourseRepository courseRepository,
        ICourseSemesterRepository courseSemesterRepository,
        IValidator<CoursePrerequisite> coursePrerequisiteValidator
    ) : ICoursePrerequisiteService
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CoursePrerequisiteService));
        private readonly ICoursePrerequisiteRepository _coursePrerequisiteRepository = coursePrerequisiteRepository;
        private readonly ICourseRepository _courseRepository = courseRepository;
        private readonly ICourseSemesterRepository _courseSemesterRepository = courseSemesterRepository;
        private readonly IValidator<CoursePrerequisite> _coursePrerequisiteValidator = coursePrerequisiteValidator;

        #region CRUD Operations

        public async Task<CoursePrerequisite?> GetCoursePrerequisiteByIdAsync(int id)
        {
            Logger.Info($"Fetching CoursePrerequisite with ID = {id}.");
            return await _coursePrerequisiteRepository.GetByIdAsync(id);
        }

        public async Task<List<CoursePrerequisite>> GetAllCoursePrerequisitesAsync()
        {
            Logger.Info("Fetching all CoursePrerequisites.");
            return await _coursePrerequisiteRepository.GetAllAsync();
        }

        public async Task<CoursePrerequisite> CreateCoursePrerequisiteAsync(CoursePrerequisite cp)
        {
            Logger.Info("Creating a new CoursePrerequisite.");

            ValidationResult validationResult = await _coursePrerequisiteValidator.ValidateAsync(cp);
            if (!validationResult.IsValid)
            {
                Logger.Warn("CoursePrerequisite validation failed on create.");
                throw new ValidationException(validationResult.Errors);
            }

            await _coursePrerequisiteRepository.AddAsync(cp);
            Logger.Info($"CoursePrerequisite created successfully with ID {cp.Id}.");
            return cp;
        }

        public async Task UpdateCoursePrerequisiteAsync(CoursePrerequisite cp)
        {
            Logger.Info($"Updating CoursePrerequisite with ID = {cp.Id}.");

            ValidationResult validationResult = await _coursePrerequisiteValidator.ValidateAsync(cp);
            if (!validationResult.IsValid)
            {
                Logger.Warn("CoursePrerequisite validation failed on update.");
                throw new ValidationException(validationResult.Errors);
            }

            await _coursePrerequisiteRepository.UpdateAsync(cp);
            Logger.Info($"CoursePrerequisite updated successfully with ID {cp.Id}.");
        }

        public async Task DeleteCoursePrerequisiteAsync(int id)
        {
            Logger.Info($"Deleting CoursePrerequisite with ID = {id}.");
            await _coursePrerequisiteRepository.DeleteAsync(id);
            Logger.Info($"CoursePrerequisite with ID {id} deleted successfully.");
        }

        #endregion
    }
}
