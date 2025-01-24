namespace ServiceLayer.Validators
{
    using DomainModel.Entities;
    using FluentValidation;
    public class StudentValidator : AbstractValidator<Student>
    {
        public StudentValidator()
        {
            RuleFor(s => s.Id)
                .NotEmpty().WithMessage("ID is required.")
                .Must(id => id > 0).WithMessage("ID must be a positive number.");

            RuleFor(s => s.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");

            RuleFor(s => s.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");

            RuleFor(s => s.CNP)
                .NotEmpty().WithMessage("CNP is required.")
                .Length(13).WithMessage("CNP must be exactly 13 characters.")
                .Matches(@"^\d{13}$").WithMessage("CNP must consist of exactly 13 digits.");

            RuleFor(s => s.Address)
                .NotEmpty().WithMessage("Address is required.");

            RuleFor(s => s.UnivCode)
                .NotEmpty().WithMessage("University code is required.")
                .MaximumLength(20).WithMessage("University code cannot exceed 20 characters.");

            RuleForEach(s => s.Emails)
                .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")
                .WithMessage("Each email must be a valid email address.");

            RuleForEach(s => s.PhoneNumbers)
                .Matches(@"^07\d{2}\s?\d{6}$")
                .WithMessage("Each phone number must follow the Romanian format: 07 followed by 2 digits, optionally a space, and then 6 digits.");

            RuleFor(s => s)
                .Must(s => (s.PhoneNumbers != null && s.PhoneNumbers.Count > 0) || (s.Emails != null && s.Emails.Count > 0))
                .WithMessage("At least one phone number or one email address must be provided.");
        }
    }
}
