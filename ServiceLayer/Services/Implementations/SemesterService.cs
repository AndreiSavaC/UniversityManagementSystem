namespace ServiceLayer.Services.Implementations
{
    using DomainModel.Entities;
    using DataAccessLayer.Repositories.Interfaces;
    using FluentValidation;
    using FluentValidation.Results;
    using ServiceLayer.Services.Interfaces;
    using log4net;

    public class SemesterService(
        ISemesterRepository semesterRepository,
        IValidator<Semester> semesterValidator
    ) : ISemesterService
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SemesterService));
        private readonly ISemesterRepository _semesterRepository = semesterRepository;
        private readonly IValidator<Semester> _semesterValidator = semesterValidator;

        public async Task<Semester> CreateSemesterAsync(Semester semester)
        {
            ValidationResult validationResult = await _semesterValidator.ValidateAsync(semester);
            if (!validationResult.IsValid)
            {
                Logger.Warn("Semester validation failed on create.");
                throw new ValidationException(validationResult.Errors);
            }

            await _semesterRepository.AddAsync(semester);
            Logger.Info($"Semester created successfully: Semester Number {semester.Number}");
            return semester;
        }

        public async Task<Semester?> GetSemesterByIdAsync(int semesterId)
        {
            Logger.Info($"Fetching Semester with ID = {semesterId}");
            return await _semesterRepository.GetByIdAsync(semesterId);
        }

        public async Task<List<Semester>> GetAllSemestersAsync()
        {
            Logger.Info("Fetching all Semesters...");
            return await _semesterRepository.GetAllAsync();
        }

        public async Task UpdateSemesterAsync(Semester semester)
        {
            ValidationResult validationResult = await _semesterValidator.ValidateAsync(semester);
            if (!validationResult.IsValid)
            {
                Logger.Warn("Semester validation failed on update.");
                throw new ValidationException(validationResult.Errors);
            }

            await _semesterRepository.UpdateAsync(semester);
            Logger.Info($"Semester updated successfully: Semester ID {semester.Id}");
        }

        public async Task DeleteSemesterAsync(int semesterId)
        {
            Logger.Info($"Deleting Semester with ID = {semesterId}");
            await _semesterRepository.DeleteAsync(semesterId);
        }
    }
}
