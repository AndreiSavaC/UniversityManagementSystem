namespace ServiceLayer.Services.Interfaces
{
    using DomainModel.Entities;
    public interface IExamService
    {
        Task<Exam?> GetExamByIdAsync(int examId);
        Task<List<Exam>> GetAllExamsAsync();
        Task<Exam> ScheduleExamAsync(Exam exam); 
        Task UpdateExamAsync(Exam exam);
        Task DeleteExamAsync(int examId);
        Task<Exam> TakeExamAsync(int studentId, int courseId, int grade, System.DateTime date);
    }
}
