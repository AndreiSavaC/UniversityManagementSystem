namespace ServiceLayer.Services.Implementations
{
    using DomainModel.Entities;
    using DataAccessLayer.Repositories.Interfaces;
    using FluentValidation;
    using FluentValidation.Results;
    using ServiceLayer.Services.Interfaces;
    using log4net;
    using DataAccessLayer.Repositories.Implementations;
    using ServiceLayer.Validators;

    public class StudentService(
        IStudentRepository studentRepository,
        IEnrollmentRepository enrollmentRepository,
        IExamRepository examRepository,
        ISemesterRepository semesterRepository,
        IValidator<Student> studentValidator
    ) : IStudentService
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(StudentService));
        private readonly IValidator<Student> _studentValidator = studentValidator;
        private readonly IStudentRepository _studentRepository = studentRepository;
        private readonly IEnrollmentRepository _enrollmentRepository = enrollmentRepository;
        private readonly IExamRepository _examRepository = examRepository;
        private readonly ISemesterRepository _semesterRepository = semesterRepository;



        public async Task<Student> CreateStudentAsync(Student student)
        {
            ValidationResult validationResult = await _studentValidator.ValidateAsync(student);
            if (!validationResult.IsValid)
            {
                Logger.Warn("Student validation failed on create.");
                throw new ValidationException(validationResult.Errors);
            }

            await _studentRepository.AddAsync(student);
            Logger.Info($"Student created successfully: {student.FirstName} {student.LastName}");
            return student;
        }

        public async Task<Student?> GetStudentByIdAsync(int studentId)
        {
            Logger.Info($"Fetching Student with ID = {studentId}");
            return await _studentRepository.GetByIdAsync(studentId);
        }

        public async Task<List<Student>> GetAllStudentsAsync()
        {
            Logger.Info("Fetching all students...");
            return await _studentRepository.GetAllAsync();
        }

        public async Task UpdateStudentAsync(Student student)
        {
            ValidationResult validationResult = await _studentValidator.ValidateAsync(student);
            if (!validationResult.IsValid)
            {
                Logger.Warn("Student validation failed on update.");
                throw new ValidationException(validationResult.Errors);
            }

            await _studentRepository.UpdateAsync(student);
            Logger.Info($"Student updated successfully: {student.FirstName} {student.LastName}");
        }

        public async Task DeleteStudentAsync(int studentId)
        {
            Logger.Info($"Deleting Student with ID = {studentId}");
            await _studentRepository.DeleteAsync(studentId);
        }

        public async Task PromoteStudentAsync(int studentId)
        {
            var student = await _studentRepository.GetByIdAsync(studentId) ?? throw new KeyNotFoundException($"Student with ID {studentId} not found.");
            var enrollments = await _enrollmentRepository.GetAllAsync();
            var studentEnrollments = enrollments
                .Where(e => e.StudentId == studentId)
                .ToList();

            if (studentEnrollments.Count == 0)
                throw new InvalidOperationException($"Student with ID {studentId} has no enrollments.");

            int currentSemesterId = studentEnrollments.First().SemesterId;
            var currentSemester = await _semesterRepository.GetByIdAsync(currentSemesterId) ?? throw new InvalidOperationException($"Semester with ID {currentSemesterId} not found.");
            var exams = await _examRepository.GetAllAsync();
            var studentExams = exams.Where(e => e.StudentId == studentId).ToList();

            int earnedCredits = 0;
            foreach (var enrollment in studentEnrollments)
            {
                var exam = studentExams.FirstOrDefault(e => e.CourseId == enrollment.CourseId);
                if (exam != null && exam.Grade >= 5) 
                {
                    earnedCredits += enrollment.Course.Credits;
                }
            }

            if (earnedCredits >= currentSemester.MinCredits)
            {
                var nextSemester = await _semesterRepository.GetByNumberAsync(currentSemester.Number + 1) ?? throw new InvalidOperationException($"No next semester available for promotion.");
                foreach (var enrollment in studentEnrollments)
                {
                    enrollment.SemesterId = nextSemester.Id;
                    await _enrollmentRepository.UpdateAsync(enrollment);
                }

                Console.WriteLine($"Student {studentId} promoted to Semester {nextSemester.Number}.");
            }
            else
            {
                throw new InvalidOperationException($"Student {studentId} has not earned enough credits ({earnedCredits}/{currentSemester.MinCredits}) for promotion.");
            }
        }
        public async Task<Dictionary<int, int>> GetStudentCreditReportAsync(int studentId)
        {
            var student = await _studentRepository.GetByIdAsync(studentId)
                ?? throw new KeyNotFoundException($"Student with ID {studentId} not found.");

            var enrollments = await _enrollmentRepository.GetAllAsync();
            var studentEnrollments = enrollments
                .Where(e => e.StudentId == studentId)
                .ToList();

            if (studentEnrollments.Count == 0)
                throw new InvalidOperationException($"Student with ID {studentId} has no enrollments.");

            var exams = await _examRepository.GetAllAsync();
            var studentExams = exams.Where(e => e.StudentId == studentId).ToList();

            var creditReport = new Dictionary<int, int>(); 

            foreach (var enrollment in studentEnrollments)
            {
                var semesterId = enrollment.SemesterId;
                var semester = await _semesterRepository.GetByIdAsync(semesterId);
                if (semester == null) continue;

                var exam = studentExams.FirstOrDefault(e => e.CourseId == enrollment.CourseId);
                if (exam != null && exam.Grade >= 5) 
                {
                    if (!creditReport.ContainsKey(semester.Number))
                        creditReport[semester.Number] = 0;

                    creditReport[semester.Number] += enrollment.Course.Credits;
                }
            }

            return creditReport;
        }

    }
}
