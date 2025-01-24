namespace DomainModel.Entities
{
    using System.ComponentModel.DataAnnotations;
    public class Course
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Course name is required.")]
        [MaxLength(100, ErrorMessage = "Course name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Course description is required.")]
        [MaxLength(500, ErrorMessage = "Course description cannot exceed 500 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Credits are required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Credits must be a positive integer.")]
        public int Credits { get; set; }

        [Required(ErrorMessage = "Cost is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cost must be positive.")]
        public decimal Cost { get; set; }

        [Required(ErrorMessage = "Minimum cost per credit is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Minimum cost per credit must be positive.")]
        public decimal MinCostPerCredit { get; set; }

        [Required(ErrorMessage = "Maximum cost per credit is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Maximum cost per credit must be positive.")]
        public decimal MaxCostPerCredit { get; set; }

        public ICollection<CourseSemester> CourseSemesters { get; set; } = [];
        public ICollection<CoursePrerequisite> Prerequisites { get; set; } = [];
        public ICollection<CoursePrerequisite> DependentCourses { get; set; } = [];
        public ICollection<CourseDiscount> CourseDiscounts { get; set; } = [];
        public ICollection<Enrollment> Enrollments { get; set; } = [];
        public ICollection<Exam> Exams { get; set; } = [];

    }
}