namespace ServiceLayer.Validators
{
    using FluentValidation;
    using DomainModel.Entities;
    using DataAccessLayer.Repositories.Interfaces;
    public class SemesterValidator : AbstractValidator<Semester>
    {
        private readonly ISemesterRepository _semesterRepository;

        public SemesterValidator(ISemesterRepository semesterRepository)
        {
            _semesterRepository = semesterRepository;

            RuleFor(s => s.Id)
                .NotEmpty().WithMessage("ID is required.")
                .Must(id => id > 0).WithMessage("ID must be a positive number.");

            RuleFor(s => s.Number)
                .NotEmpty().WithMessage("Semester number is required.")
                .GreaterThanOrEqualTo(1).WithMessage("Semester number must be a positive integer.")
                .MustAsync(BeUniqueSemesterNumber).WithMessage("Semester number must be unique."); 

            RuleFor(s => s.MinCredits)
                .NotNull().WithMessage("Minimum credits are required.")
                .GreaterThanOrEqualTo(0).WithMessage("Minimum credits must be non-negative.");
        }

        private async Task<bool> BeUniqueSemesterNumber(Semester semester, int number, CancellationToken cancellationToken)
        {
            var existingSemester = await _semesterRepository.GetByNumberAsync(number);
            if (existingSemester == null)
                return true;

            return existingSemester.Id == semester.Id;
        }
    }
}
