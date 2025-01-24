namespace TestServiceLayer.Helpers
{
    using DomainModel.Entities;
    using System.Collections.Generic;

    public static class SemesterTestHelper
    {
        public static Semester CreateSemester(
            int? id = null,
            int? number = null,
            int? minCredits = null)
        {
            return new Semester
            {
                Id = id ?? 1,
                Number = number ?? 1,
                MinCredits = minCredits ?? 30
            };
        }
    }
}
