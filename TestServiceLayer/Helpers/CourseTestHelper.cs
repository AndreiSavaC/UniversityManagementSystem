namespace TestServiceLayer.Helpers
{
    using DomainModel.Entities;

    public static class CourseTestHelper
    {
        public static Course CreateCourse(
            int? id = null,
            string? name = null,
            string? description = null,
            int? credits = null,
            decimal? cost = null,
            decimal? minCostPerCredit = null,
            decimal? maxCostPerCredit = null)
        {
            return new Course
            {
                Id = id ?? 1,
                Name = name ?? "Default Course",
                Description = description ?? "Default Description",
                Credits = credits ?? 5,
                Cost = cost ?? 500.00m,
                MinCostPerCredit = minCostPerCredit ?? 100.00m,
                MaxCostPerCredit = maxCostPerCredit ?? 200.00m
            };
        }
    }
}
