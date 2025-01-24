namespace ServiceLayer.Validators
{
    using DomainModel.Entities;
    using FluentValidation;
    using DataAccessLayer.Repositories.Interfaces;
    using DataAccessLayer.Repositories.Implementations;

    public class EnrollmentValidator : AbstractValidator<Enrollment>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly ISemesterRepository _semesterRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;

        public EnrollmentValidator(
            IStudentRepository studentRepository,
            ICourseRepository courseRepository,
            ISemesterRepository semesterRepository,
            IEnrollmentRepository enrollmentRepository)
        {
            _studentRepository = studentRepository;
            _courseRepository = courseRepository;
            _semesterRepository = semesterRepository;
            _enrollmentRepository = enrollmentRepository;
            RuleFor(s => s.Id)
                .NotEmpty().WithMessage("ID is required.")
                .Must(id => id > 0).WithMessage("ID must be a positive number.");

            RuleFor(e => e.StudentId)
                .GreaterThan(0).WithMessage("StudentId must be greater than 0.")
                .MustAsync(StudentExists).WithMessage("Student does not exist.");

            RuleFor(e => e.CourseId)
                .GreaterThan(0).WithMessage("CourseId must be greater than 0.")
                .MustAsync(CourseExists).WithMessage("Course does not exist.");

            RuleFor(e => e.SemesterId)
                .GreaterThan(0).WithMessage("SemesterId must be greater than 0.")
                .MustAsync(SemesterExists).WithMessage("Semester does not exist.");

            RuleFor(e => e.TotalPaid)
                .GreaterThanOrEqualTo(0).WithMessage("TotalPaid must be non-negative.");
        
        }

        private async Task<bool> StudentExists(int studentId, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(studentId);
            return student != null;
        }

        private async Task<bool> CourseExists(int courseId, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            return course != null;
        }

        private async Task<bool> SemesterExists(int semesterId, CancellationToken cancellationToken)
        {
            var semester = await _semesterRepository.GetByIdAsync(semesterId);
            return semester != null;
        }
       
    }
}
