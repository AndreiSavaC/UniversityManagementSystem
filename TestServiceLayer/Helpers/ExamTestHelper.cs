namespace TestServiceLayer.Helpers
{
    using DomainModel.Entities;
    using System;

    public static class ExamTestHelper
    {
        public static Exam CreateExam(
            int? id = null,
            int? studentId = null,
            int? courseId = null,
            DateTime? date = null,
            int? grade = null)
        {
            return new Exam
            {
                Id = id ?? 1,
                StudentId = studentId ?? 1,
                CourseId = courseId ?? 101,
                Date = date ?? DateTime.Now,
                Grade = grade ?? 8
            };
        }
    }
}
