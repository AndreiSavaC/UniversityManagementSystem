namespace ServiceLayer.Services.Interfaces
{
    using DomainModel.Entities;
    public interface ICourseService
    {
        Task<Course> CreateCourseAsync(Course course);
        Task<Course?> GetCourseByIdAsync(int courseId);
        Task<List<Course>> GetAllCoursesAsync();
        Task UpdateCourseAsync(Course course);
        Task DeleteCourseAsync(int courseId);
    }
}
