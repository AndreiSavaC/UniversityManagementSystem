namespace ServiceLayer.Services.Interfaces
{
    using DomainModel.Entities;
    public interface IEnrollmentService
    {
        Task<Enrollment?> GetEnrollmentByIdAsync(int enrollmentId);
        Task<List<Enrollment>> GetAllEnrollmentsAsync();
        Task<Enrollment> CreateEnrollmentAsync(Enrollment enrollment);
        Task UpdateEnrollmentAsync(Enrollment enrollment);
        Task DeleteEnrollmentAsync(int enrollmentId);

        Task<Enrollment> EnrollStudentInCourseAsync(int studentId, int courseId, int semesterId, decimal initialPayment);

    }
}
