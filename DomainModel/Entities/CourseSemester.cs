namespace DomainModel.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class CourseSemester
    {
        public int Id { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "CourseId must be greater than 0.")]
        [ForeignKey("Course")]
        public int CourseId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "SemesterId must be greater than 0.")]
        [ForeignKey("Semester")]
        public int SemesterId { get; set; }

        public Course Course { get; set; }
        public Semester Semester { get; set; }
    }
}
