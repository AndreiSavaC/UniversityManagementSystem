namespace TestServiceLayer.Helpers
{
    using DomainModel.Entities;

    public static class CourseSemesterTestHelper
    {
        public static CourseSemester CreateCourseSemester(
            int? id = null,
            int? courseId = null,
            int? semesterId = null)
        {
            return new CourseSemester
            {
                Id = id ?? 1,
                CourseId = courseId ?? 101,
                SemesterId = semesterId ?? 1
            };
        }
    }
}
