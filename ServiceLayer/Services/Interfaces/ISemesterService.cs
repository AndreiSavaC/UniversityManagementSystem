namespace ServiceLayer.Services.Interfaces
{
    using DomainModel.Entities;
    public interface ISemesterService
    {
        Task<Semester> CreateSemesterAsync(Semester semester);
        Task<Semester?> GetSemesterByIdAsync(int semesterId);
        Task<List<Semester>> GetAllSemestersAsync();
        Task UpdateSemesterAsync(Semester semester);
        Task DeleteSemesterAsync(int semesterId);
    }
}
