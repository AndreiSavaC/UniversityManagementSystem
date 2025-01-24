
namespace DomainModel.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    public class CourseDiscount
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Group ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Group ID must be greater than 0.")]
        public int GroupId { get; set; }

        [ForeignKey("Course")]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Discount percentage is required.")]
        [Range(0, 100, ErrorMessage = "Discount percentage must be between 0 and 100.")]
        public double DiscountPercentage { get; set; }

        public Course Course { get; set; }

    }
}
