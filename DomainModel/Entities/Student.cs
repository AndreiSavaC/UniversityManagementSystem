namespace DomainModel.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class Student
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public  string  FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public  string LastName { get; set; }

        [Required(ErrorMessage = "CNP is required.")]
        [StringLength(13, MinimumLength = 13, ErrorMessage = "CNP must be exactly 13 characters.")]
        public  string CNP { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public  string Address { get; set; }

        [Required(ErrorMessage = "University code is required.")]
        [MaxLength(20, ErrorMessage = "University code cannot exceed 20 characters.")]
        public  string UnivCode { get; set; }

        [Column(TypeName = "text[]")]
        public  List<string> Emails { get; set; } = [];

        [Column(TypeName = "text[]")]
        public  List<string> PhoneNumbers { get; set; } = [];

        public ICollection<Enrollment> Enrollments { get; set; }
        public ICollection<Exam> Exams { get; set; }
    }
}
