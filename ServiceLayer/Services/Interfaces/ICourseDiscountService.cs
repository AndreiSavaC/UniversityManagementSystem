namespace ServiceLayer.Services.Interfaces
{
    using DomainModel.Entities;
    public interface ICourseDiscountService
    {
        Task<CourseDiscount?> GetCourseDiscountByIdAsync(int id);
        Task<List<CourseDiscount>> GetAllCourseDiscountsAsync();
        Task<CourseDiscount> CreateCourseDiscountAsync(CourseDiscount discount);
        Task UpdateCourseDiscountAsync(CourseDiscount discount);
        Task DeleteCourseDiscountAsync(int id);
        Task ApplyDiscountsAsync(Student student);
    }
}
