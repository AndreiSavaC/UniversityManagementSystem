namespace ServiceLayer.Validators
{
    using DomainModel.Entities;
    using FluentValidation;

    public class CourseValidator : AbstractValidator<Course>
    {
        public CourseValidator()
        {
            RuleFor(s => s.Id)
                .NotEmpty().WithMessage("ID is required.")
                .Must(id => id > 0).WithMessage("ID must be a positive number.");

            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Course name is required.")
                .MaximumLength(100).WithMessage("Course name cannot exceed 100 characters.");

            RuleFor(c => c.Description)
                .NotEmpty().WithMessage("Course description is required.")
                .MaximumLength(500).WithMessage("Course description cannot exceed 500 characters.");

            RuleFor(c => c.Credits)
                .GreaterThan(0).WithMessage("Credits must be a positive integer.");

            RuleFor(c => c.Cost)
                .GreaterThan(0).WithMessage("Cost per credit must be positive.");

            RuleFor(c => c)
                .Must(c => c.MinCostPerCredit <= c.MaxCostPerCredit)
                .WithMessage("Minimum cost per credit must be less than or equal to maximum cost per credit.");

            RuleFor(c => c.Cost)
                .Must((course, cost) =>
                    cost >= (course.MinCostPerCredit * course.Credits) &&
                    cost <= (course.MaxCostPerCredit * course.Credits))
                .WithMessage("Cost must be in range [MinCostPerCredit * Credits, MaxCostPerCredit * Credits].");
        }
    }
}
