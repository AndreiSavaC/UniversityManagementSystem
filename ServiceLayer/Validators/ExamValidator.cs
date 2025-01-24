namespace ServiceLayer.Validators
{
    using FluentValidation;
    using DomainModel.Entities;
    using DataAccessLayer.Repositories.Interfaces;
    public class ExamValidator : AbstractValidator<Exam>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IExamRepository _examRepository;

        public ExamValidator(
            IStudentRepository studentRepository,
            ICourseRepository courseRepository,
            IExamRepository examRepository)
        {
            _studentRepository = studentRepository;
            _courseRepository = courseRepository;
            _examRepository = examRepository;

            RuleFor(s => s.Id)
                .NotEmpty().WithMessage("ID is required.")
                .Must(id => id > 0).WithMessage("ID must be a positive number.");

            RuleFor(e => e.StudentId)
                .GreaterThan(0).WithMessage("StudentId must be greater than 0.")
                .MustAsync(StudentExists).WithMessage("Student does not exist.");

            RuleFor(e => e.CourseId)
                .GreaterThan(0).WithMessage("CourseId must be greater than 0.")
                .MustAsync(CourseExists).WithMessage("Course does not exist.");

            RuleFor(e => e.Date)
                .NotEmpty().WithMessage("Exam date is required.")
                .Must(date => date != default).WithMessage("Exam date is invalid.");

            RuleFor(e => e.Grade)
                .InclusiveBetween(1, 10).WithMessage("Grade must be between 1 and 10.");

            RuleFor(e => e)
                .MustAsync(NoMultipleExamsSameDay)
                .WithMessage("A student cannot have multiple exams on the same day.");
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

        private async Task<bool> NoMultipleExamsSameDay(Exam exam, CancellationToken cancellationToken)
        {
            var allExams = await _examRepository.GetAllAsync() ?? [];
            return !allExams.Any(e =>
                e.StudentId == exam.StudentId
                && e.Date.Date == exam.Date.Date
                && e.Id != exam.Id);
        }
    }
}
