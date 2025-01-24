namespace ServiceLayer.Validators
{
    using FluentValidation;
    using DomainModel.Entities;
    using DataAccessLayer.Repositories.Interfaces;
    public class CourseDiscountValidator : AbstractValidator<CourseDiscount>
    {
        private readonly ICourseDiscountRepository _courseDiscountRepository;
        private readonly ICourseRepository _courseRepository;

        public CourseDiscountValidator(ICourseDiscountRepository courseDiscountRepository, ICourseRepository courseRepository)
        {
            _courseDiscountRepository = courseDiscountRepository;
            _courseRepository = courseRepository;

            RuleFor(s => s.Id)
                .NotEmpty().WithMessage("ID is required.")
                .Must(id => id > 0).WithMessage("ID must be a positive number.");

            RuleFor(cd => cd.GroupId)
                .NotEmpty().WithMessage("Group ID is required.")
                .MustAsync(BeUniqueGroupId).WithMessage("Group ID must be unique.");

            RuleFor(cd => cd.CourseId)
                .NotEmpty().WithMessage("Course ID is required.")
                .MustAsync(CourseExists).WithMessage("Course must exist.");

            RuleFor(cd => cd.DiscountPercentage)
                .InclusiveBetween(0, 100).WithMessage("Discount percentage must be between 0 and 100.");
        }

        private async Task<bool> BeUniqueGroupId(int groupId, CancellationToken cancellationToken)
        {
            return !await _courseDiscountRepository.ExistsGroupIdAsync(groupId);
        }

        private async Task<bool> CourseExists(int courseId, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            return course != null;
        }
    }
}
