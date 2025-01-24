namespace DomainModel.Entities
{
using System.ComponentModel.DataAnnotations;
    public class Semester
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Semester number is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Semester number must be a positive integer.")]
        public int Number { get; set; }

        [Required(ErrorMessage = "Minimum credits for the semester are required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Minimum credits must be non-negative.")]
        public int MinCredits { get; set; }
        public ICollection<CourseSemester> CourseSemesters { get; set; } = [];
        public ICollection<Enrollment> Enrollments { get; set; } = [];
    }

}
