namespace DomainModel.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    public class CoursePrerequisite
    {
        public int Id { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "CourseId must be greater than 0.")]
        public int CourseId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "PrereqId must be greater than 0.")]
        public int PrereqId { get; set; }

        [Required(ErrorMessage = "Minimum grade for prerequisite is required.")]
        [Range(1, 10, ErrorMessage = "Minimum grade must be between 1 and 10.")]
        public int MinGrade { get; set; }

        [ForeignKey("CourseId")]
        public Course Course { get; set; }

        [ForeignKey("PrereqId")]
        public Course PrerequisiteCourse { get; set; }
    }
}
