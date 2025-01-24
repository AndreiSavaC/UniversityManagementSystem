namespace ServiceLayer.Services.Implementations
{
    using DomainModel.Entities;
    using DataAccessLayer.Repositories.Interfaces;
    using ServiceLayer.Services.Interfaces;
    using FluentValidation;
    using FluentValidation.Results;
    using log4net;
    public class CourseDiscountService(
        ICourseRepository courseRepository,
        ICourseDiscountRepository courseDiscountRepository,
        IValidator<CourseDiscount> courseDiscountValidator
    ) : ICourseDiscountService
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CourseDiscountService));
        private readonly ICourseRepository _courseRepository = courseRepository;
        private readonly ICourseDiscountRepository _courseDiscountRepository = courseDiscountRepository;
        private readonly IValidator<CourseDiscount> _courseDiscountValidator = courseDiscountValidator;

        #region CRUD Operations

        public async Task<CourseDiscount?> GetCourseDiscountByIdAsync(int id)
        {
            Logger.Info($"Fetching CourseDiscount with ID = {id}.");
            return await _courseDiscountRepository.GetByIdAsync(id);
        }

        public async Task<List<CourseDiscount>> GetAllCourseDiscountsAsync()
        {
            Logger.Info("Fetching all CourseDiscounts.");
            return await _courseDiscountRepository.GetAllAsync();
        }

        public async Task<CourseDiscount> CreateCourseDiscountAsync(CourseDiscount discount)
        {
            Logger.Info("Creating a new CourseDiscount.");

            ValidationResult validationResult = await _courseDiscountValidator.ValidateAsync(discount);
            if (!validationResult.IsValid)
            {
                Logger.Warn("CourseDiscount validation failed on create.");
                throw new ValidationException(validationResult.Errors);
            }

            await _courseDiscountRepository.AddAsync(discount);
            Logger.Info($"CourseDiscount created successfully with Group ID {discount.GroupId}.");
            return discount;
        }

        public async Task UpdateCourseDiscountAsync(CourseDiscount discount)
        {
            Logger.Info($"Updating CourseDiscount with ID = {discount.Id}.");

            ValidationResult validationResult = await _courseDiscountValidator.ValidateAsync(discount);
            if (!validationResult.IsValid)
            {
                Logger.Warn("CourseDiscount validation failed on update.");
                throw new ValidationException(validationResult.Errors);
            }

            await _courseDiscountRepository.UpdateAsync(discount);
            Logger.Info($"CourseDiscount updated successfully with ID {discount.Id}.");
        }

        public async Task DeleteCourseDiscountAsync(int id)
        {
            Logger.Info($"Deleting CourseDiscount with ID = {id}.");
            await _courseDiscountRepository.DeleteAsync(id);
            Logger.Info($"CourseDiscount with ID {id} deleted successfully.");
        }

        #endregion

        #region Apply Discounts

        public async Task ApplyDiscountsAsync(Student student)
        {
            Logger.Info($"Applying discounts for Student ID {student.Id}.");

            if (student.Enrollments == null || student.Enrollments.Count == 0)
            {
                Logger.Warn($"No enrolled courses found for Student ID {student.Id}. No discounts applied.");
                return;
            }

            var enrolledCourseIds = student.Enrollments.Select(e => e.CourseId).ToList();

            var discountGroups = await _courseDiscountRepository.GetAllAsync() ?? [];

            var maxDiscounts = new Dictionary<int, decimal>();

            foreach (var group in discountGroups)
            {
                var groupCourseIds = await _courseDiscountRepository.GetCourseIdsByGroupIdAsync(group.GroupId) ?? [];

                if (groupCourseIds.Count != 0 && groupCourseIds.All(id => enrolledCourseIds.Contains(id)))
                {
                    var courseDiscount = new CourseDiscount
                    {
                        GroupId = group.GroupId,
                        CourseId = groupCourseIds.First(),
                        DiscountPercentage = group.DiscountPercentage
                    };

                    ValidationResult validationResult = await _courseDiscountValidator.ValidateAsync(courseDiscount);
                    if (!validationResult.IsValid)
                    {
                        Logger.Warn($"Validation failed for Group ID {group.GroupId}: {string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))}");
                        continue;
                    }

                    foreach (var courseId in groupCourseIds)
                    {
                        if (!maxDiscounts.TryGetValue(courseId, out decimal value) || value < (decimal)group.DiscountPercentage)
                        {
                            maxDiscounts[courseId] = (decimal)group.DiscountPercentage;
                        }
                    }
                }
            }

            foreach (var kvp in maxDiscounts)
            {
                var courseId = kvp.Key;
                var discountPercentage = kvp.Value;

                var course = await _courseRepository.GetByIdAsync(courseId);
                if (course != null)
                {
                    decimal originalCost = course.Cost;
                    course.Cost *= (1 - (discountPercentage / 100m));
                    await _courseRepository.UpdateAsync(course);
                    Logger.Info($"Applied {discountPercentage}% discount to Course ID {courseId}. Cost: {originalCost} -> {course.Cost}");
                }
                else
                {
                    Logger.Warn($"Course with ID {courseId} not found. Discount not applied.");
                }
            }

            Logger.Info($"Discounts applied successfully for Student ID {student.Id}.");
        }



        #endregion
    }
}
