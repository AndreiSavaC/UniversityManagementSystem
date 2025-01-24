namespace TestServiceLayer.Helpers
{
    using DomainModel.Entities;

    public static class CourseDiscountTestHelper
    {
        public static CourseDiscount CreateCourseDiscount(
            int? id = null,
            int? groupId = null,
            int? courseId = null,
            double? discountPercentage = null)
        {
            return new CourseDiscount
            {
                Id = id ?? 1,
                GroupId = groupId ?? 1,
                CourseId = courseId ?? 101,
                DiscountPercentage = discountPercentage ?? 10.0
            };
        }
    }
}
