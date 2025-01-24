
namespace DataAccessLayer.Repositories.Interfaces
{
    using DomainModel.Entities;
    public interface ICourseDiscountRepository : IRepository<CourseDiscount>
    {
        Task<bool> ExistsGroupIdAsync(int groupId);
        Task<List<int>> GetCourseIdsByGroupIdAsync(int groupId);
    }
}
