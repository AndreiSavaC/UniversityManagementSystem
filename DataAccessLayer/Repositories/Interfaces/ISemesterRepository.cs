namespace DataAccessLayer.Repositories.Interfaces
{
    using DomainModel.Entities;
    public interface ISemesterRepository : IRepository<Semester>
    {
        Task<Semester?> GetByNumberAsync(int number);
    }
}
