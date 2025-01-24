namespace TestServiceLayer.Helpers
{
    using DomainModel.Entities;

    public static class EnrollmentTestHelper
    {
        public static Enrollment CreateEnrollment(
            int? id = null,
            int? studentId = null,
            int? courseId = null,
            int? semesterId = null,
            decimal? totalPaid = null)
        {
            return new Enrollment
            {
                Id = id ?? 1,
                StudentId = studentId ?? 1,
                CourseId = courseId ?? 101,
                SemesterId = semesterId ?? 2025,
                TotalPaid = totalPaid ?? 1500.50m
            };
        }
    }
}
