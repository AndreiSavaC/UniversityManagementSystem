namespace ServiceLayer.Services.Interfaces
{
    using DomainModel.Entities;
    public interface ICoursePrerequisiteService
    {
        Task<CoursePrerequisite?> GetCoursePrerequisiteByIdAsync(int id);
        Task<List<CoursePrerequisite>> GetAllCoursePrerequisitesAsync();
        Task<CoursePrerequisite> CreateCoursePrerequisiteAsync(CoursePrerequisite cp);
        Task UpdateCoursePrerequisiteAsync(CoursePrerequisite cp);
        Task DeleteCoursePrerequisiteAsync(int id);
    }
}
