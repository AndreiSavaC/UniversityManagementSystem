namespace ServiceLayer.Services.Implementations
{
    using DomainModel.Entities;
    using DataAccessLayer.Repositories.Interfaces;
    using FluentValidation;
    using FluentValidation.Results;
    using ServiceLayer.Services.Interfaces;
    using log4net;

    public class EnrollmentService(
        IEnrollmentRepository enrollmentRepository,
        IValidator<Enrollment> enrollmentValidator,
        IStudentRepository studentRepository,
        ICourseRepository courseRepository,
        ISemesterRepository semesterRepository
    ) : IEnrollmentService
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(EnrollmentService));
        private readonly IEnrollmentRepository _enrollmentRepository = enrollmentRepository;
        private readonly IValidator<Enrollment> _enrollmentValidator = enrollmentValidator;
        private readonly IStudentRepository _studentRepository = studentRepository;
        private readonly ICourseRepository _courseRepository = courseRepository;
        private readonly ISemesterRepository _semesterRepository = semesterRepository;


        #region CRUD

        public async Task<Enrollment?> GetEnrollmentByIdAsync(int enrollmentId)
        {
            Logger.Info($"Fetching Enrollment with ID = {enrollmentId}.");
            return await _enrollmentRepository.GetByIdAsync(enrollmentId);
        }

        public async Task<List<Enrollment>> GetAllEnrollmentsAsync()
        {
            Logger.Info("Fetching all Enrollments...");
            return await _enrollmentRepository.GetAllAsync();
        }

        public async Task<Enrollment> CreateEnrollmentAsync(Enrollment enrollment)
        {
            ValidationResult validationResult = await _enrollmentValidator.ValidateAsync(enrollment);
            if (!validationResult.IsValid)
            {
                Logger.Warn("Enrollment validation failed on create.");
                throw new ValidationException(validationResult.Errors);
            }

            await _enrollmentRepository.AddAsync(enrollment);
            Logger.Info($"Enrollment created successfully. ID = {enrollment.Id}");
            return enrollment;
        }

        public async Task UpdateEnrollmentAsync(Enrollment enrollment)
        {
            ValidationResult validationResult = await _enrollmentValidator.ValidateAsync(enrollment);
            if (!validationResult.IsValid)
            {
                Logger.Warn("Enrollment validation failed on update.");
                throw new ValidationException(validationResult.Errors);
            }

            await _enrollmentRepository.UpdateAsync(enrollment);
            Logger.Info($"Enrollment updated successfully. ID = {enrollment.Id}");
        }

        public async Task DeleteEnrollmentAsync(int enrollmentId)
        {
            Logger.Info($"Deleting Enrollment with ID = {enrollmentId}");
            await _enrollmentRepository.DeleteAsync(enrollmentId);
        }

        #endregion

        #region Logica Avansată

        public async Task<Enrollment> EnrollStudentInCourseAsync(int studentId, int courseId, int semesterId, decimal initialPayment)
        {
            Logger.Info($"EnrollStudentInCourseAsync: studentId={studentId}, courseId={courseId}, semesterId={semesterId}, initialPayment={initialPayment}");

            var student = await _studentRepository.GetByIdAsync(studentId) ?? throw new InvalidOperationException($"Student with ID={studentId} not found.");

            var course = await _courseRepository.GetByIdAsync(courseId) ?? throw new InvalidOperationException($"Course with ID={courseId} not found.");

            var semester = await _semesterRepository.GetByIdAsync(semesterId) ?? throw new InvalidOperationException($"Semester with ID={semesterId} not found.");

            bool belongsToSemester = course.CourseSemesters.Any(cs => cs.SemesterId == semesterId);
            if (!belongsToSemester)
                throw new InvalidOperationException("Course does not belong to the specified semester.");

            decimal minCost = course.MinCostPerCredit * course.Credits;
            decimal maxCost = course.MaxCostPerCredit * course.Credits;
            if (course.Cost < minCost || course.Cost > maxCost)
                throw new InvalidOperationException($"Course cost is out of allowed range: [{minCost}, {maxCost}].");

            var existingEnrollments = await _enrollmentRepository.GetAllAsync();
            bool alreadyEnrolled = existingEnrollments.Any(e => e.StudentId == studentId && e.CourseId == courseId);
            if (alreadyEnrolled)
                throw new InvalidOperationException("Student is already enrolled in this course.");

            int currentSemesterCredits = existingEnrollments
                .Where(e => e.StudentId == studentId && e.SemesterId == semesterId)
                .Sum(e => e.Course.Credits);

            if (currentSemesterCredits + course.Credits < semester.MinCredits)
            {
                throw new InvalidOperationException($"Insufficient credits. Need at least {semester.MinCredits} in semester {semester.Number}.");
            }

            var newEnrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                SemesterId = semesterId,
                TotalPaid = initialPayment
            };

            var validationResult = await _enrollmentValidator.ValidateAsync(newEnrollment);
            if (!validationResult.IsValid)
            {
                Logger.Warn("Enrollment validation failed.");
                throw new ValidationException(validationResult.Errors);
            }

            await _enrollmentRepository.AddAsync(newEnrollment);
            Logger.Info($"Enrollment created. ID={newEnrollment.Id}, Student={studentId}, Course={courseId}, Semester={semesterId}");

            return newEnrollment;
        }

        #endregion
    }
}
