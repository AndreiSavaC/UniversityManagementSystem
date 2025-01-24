namespace DomainModel.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class Enrollment
    {
        public int Id { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "StudentId must be greater than 0.")]
        [ForeignKey("Student")]
        public int StudentId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "CourseId must be greater than 0.")]
        [ForeignKey("Course")]
        public int CourseId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "SemesterId must be greater than 0.")]
        [ForeignKey("Semester")]
        public int SemesterId { get; set; }

        [Required(ErrorMessage = "TotalPaid is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "TotalPaid must be a non-negative value.")]
        public decimal TotalPaid { get; set; }

        public Student Student { get; set; }
        public Course Course { get; set; }
        public Semester Semester { get; set; }
    }
}
