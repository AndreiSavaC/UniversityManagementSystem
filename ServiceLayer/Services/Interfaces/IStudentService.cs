namespace ServiceLayer.Services.Interfaces
{
    using DomainModel.Entities;

    public interface IStudentService
    {
        Task<Student> CreateStudentAsync(Student student);
        Task<Student?> GetStudentByIdAsync(int studentId);
        Task<List<Student>> GetAllStudentsAsync();
        Task UpdateStudentAsync(Student student);
        Task DeleteStudentAsync(int studentId);
        Task PromoteStudentAsync(int studentId);
        Task<Dictionary<int, int>> GetStudentCreditReportAsync(int studentId);
    }
}
