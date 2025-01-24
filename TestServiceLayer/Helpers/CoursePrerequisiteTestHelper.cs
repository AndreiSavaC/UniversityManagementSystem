namespace TestServiceLayer.Helpers
{
    using DomainModel.Entities;

    public static class CoursePrerequisiteTestHelper
    {
        public static CoursePrerequisite CreateCoursePrerequisite(
            int? id = null,
            int? courseId = null,
            int? prereqId = null,
            int? minGrade = null)
        {
            return new CoursePrerequisite
            {
                Id = id ?? 1,
                CourseId = courseId ?? 101,
                PrereqId = prereqId ?? 201,
                MinGrade = minGrade ?? 7
            };
        }
    }
}
