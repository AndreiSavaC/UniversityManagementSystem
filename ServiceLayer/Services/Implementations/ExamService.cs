namespace ServiceLayer.Services.Implementations
{
    using DomainModel.Entities;
    using DataAccessLayer.Repositories.Interfaces;
    using FluentValidation;
    using FluentValidation.Results;
    using ServiceLayer.Services.Interfaces;
    using log4net;

    public class ExamService(
        IExamRepository examRepository,
        IValidator<Exam> examValidator,
        IEnrollmentRepository enrollmentRepository,
        int maxExamAttempts = 3
    ) : IExamService
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ExamService));
        private readonly IExamRepository _examRepository = examRepository;
        private readonly IValidator<Exam> _examValidator = examValidator;
        private readonly IEnrollmentRepository _enrollmentRepository = enrollmentRepository;
        private readonly int _maxExamAttempts = maxExamAttempts;

        public async Task<Exam?> GetExamByIdAsync(int examId)
        {
            Logger.Info($"Fetching Exam with ID = {examId}");
            return await _examRepository.GetByIdAsync(examId);
        }

        public async Task<List<Exam>> GetAllExamsAsync()
        {
            Logger.Info("Fetching all Exams...");
            return await _examRepository.GetAllAsync();
        }

        public async Task<Exam> ScheduleExamAsync(Exam exam)
        {
            ValidationResult validationResult = await _examValidator.ValidateAsync(exam);
            if (!validationResult.IsValid)
            {
                Logger.Warn("Exam validation failed on create (schedule).");
                throw new ValidationException(validationResult.Errors);
            }

            await _examRepository.AddAsync(exam);
            Logger.Info($"Exam scheduled successfully. ID={exam.Id}");
            return exam;
        }

        public async Task UpdateExamAsync(Exam exam)
        {
            ValidationResult validationResult = await _examValidator.ValidateAsync(exam);
            if (!validationResult.IsValid)
            {
                Logger.Warn("Exam validation failed on update.");
                throw new ValidationException(validationResult.Errors);
            }

            await _examRepository.UpdateAsync(exam);
            Logger.Info($"Exam updated successfully. ID={exam.Id}");
        }

        public async Task DeleteExamAsync(int examId)
        {
            Logger.Info($"Deleting Exam with ID={examId}");
            await _examRepository.DeleteAsync(examId);
        }


        /// <summary>
        /// Method that executes the logic for "student takes an exam":
        /// - Checks the number of previous attempts for the same course
        /// - If the student has already passed (>=5), they cannot take it again
        /// - If they have exceeded the max failed attempts, they must choose another course
        /// - Refunds 50% of the cost if the student fails
        /// </summary>
        public async Task<Exam> TakeExamAsync(int studentId, int courseId, int grade, DateTime date)
        {
            Logger.Info($"TakeExamAsync called. StudentId={studentId}, CourseId={courseId}, Grade={grade}, Date={date}");

            // 1. Check if the student is enrolled in this course
            var allEnrollments = await _enrollmentRepository.GetAllAsync();
            var enrollment = allEnrollments.FirstOrDefault(e => e.StudentId == studentId && e.CourseId == courseId);
            if (enrollment == null)
            {
                throw new InvalidOperationException("Student is not enrolled in this course, cannot take exam.");
            }

            // 2. Retrieve previous exams
            var allExams = await _examRepository.GetAllAsync();
            var examAttempts = allExams
                .Where(x => x.StudentId == studentId && x.CourseId == courseId)
                .OrderBy(x => x.Date)
                .ToList();

            // 2.1. If the student has already passed (>=5), they cannot take it again
            if (examAttempts.Any(x => x.Grade >= 5))
            {
                throw new InvalidOperationException("Student already passed this course. No further attempts allowed.");
            }

            // 2.2. Check how many failed attempts exist
            int failedAttempts = examAttempts.Count(x => x.Grade < 5);
            if (failedAttempts >= _maxExamAttempts)
            {
                throw new InvalidOperationException($"Student has reached max attempts ({_maxExamAttempts}). Must choose another course.");
            }

            // 3. Create a new exam
            var newExam = new Exam
            {
                StudentId = studentId,
                CourseId = courseId,
                Grade = grade,
                Date = date
            };

            // 4. Validate the exam (includes rule: "no two exams on the same day for the same student")
            ValidationResult validationResult = await _examValidator.ValidateAsync(newExam);
            if (!validationResult.IsValid)
            {
                Logger.Warn("Exam validation failed on take exam.");
                throw new ValidationException(validationResult.Errors);
            }

            // 5. Save the exam
            await _examRepository.AddAsync(newExam);
            Logger.Info($"Exam saved. ID={newExam.Id}, Grade={grade}");

            // 6. If the student fails (grade < 5), refund 50% of the cost
            if (grade < 5)
            {
                decimal refundAmount = enrollment.TotalPaid * 0.5m;
                enrollment.TotalPaid -= refundAmount;
                await _enrollmentRepository.UpdateAsync(enrollment);

                Logger.Info($"Student failed exam. Refunding 50%. Refund={refundAmount}. New TotalPaid={enrollment.TotalPaid}");
            }

            return newExam;
        }
    }
}

