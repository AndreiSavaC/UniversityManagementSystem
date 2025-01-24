namespace DomainModel.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class Exam
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

        [Required(ErrorMessage = "Exam date is required.")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Grade is required.")]
        [Range(1, 10, ErrorMessage = "Grade must be between 1 and 10.")]
        public int Grade { get; set; }

        public Student Student { get; set; }
        public Course Course { get; set; }
    }
}
