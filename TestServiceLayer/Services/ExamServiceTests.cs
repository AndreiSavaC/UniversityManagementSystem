namespace TestServiceLayer.Services
{
    using Moq;
    using DomainModel.Entities;
    using DataAccessLayer.Repositories.Interfaces;
    using ServiceLayer.Services.Interfaces;
    using ServiceLayer.Services.Implementations;
    using FluentValidation;
    using FluentValidation.Results;
    [TestClass]
    public class ExamServiceTests
    {
        private Mock<IExamRepository> _mockExamRepo;
        private Mock<IValidator<Exam>> _mockValidator;
        private Mock<IEnrollmentRepository> _mockEnrollmentRepo;
        private IExamService _examService;

        [TestInitialize]
        public void Setup()
        {
            _mockExamRepo = new Mock<IExamRepository>();
            _mockValidator = new Mock<IValidator<Exam>>();
            _mockEnrollmentRepo = new Mock<IEnrollmentRepository>();

            _examService = new ExamService(
                _mockExamRepo.Object,
                _mockValidator.Object,
                _mockEnrollmentRepo.Object,
                maxExamAttempts: 3
            );
        }

        [TestMethod]
        public async Task Should_Get_Exam_By_Id()
        {
            var exam = new Exam { Id = 1, StudentId = 1, CourseId = 1, Grade = 8 };
            _mockExamRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(exam);

            var result = await _examService.GetExamByIdAsync(1);

            _mockExamRepo.Verify(r => r.GetByIdAsync(1), Times.Once);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual(8, result.Grade);
        }

        [TestMethod]
        public async Task Should_Get_All_Exams()
        {
            var exams = new List<Exam>
    {
        new() { Id = 1, StudentId = 1, CourseId = 1, Grade = 7 },
        new() { Id = 2, StudentId = 2, CourseId = 2, Grade = 9 }
    };
            _mockExamRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(exams);

            var result = await _examService.GetAllExamsAsync();

            _mockExamRepo.Verify(r => r.GetAllAsync(), Times.Once);
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(7, result[0].Grade);
            Assert.AreEqual(9, result[1].Grade);
        }

        [TestMethod]
        public async Task Should_Update_Exam()
        {
            var exam = new Exam { Id = 1, StudentId = 1, CourseId = 1, Grade = 6 };

            _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<Exam>(), default))
                          .ReturnsAsync(new ValidationResult());

            _mockExamRepo.Setup(r => r.UpdateAsync(exam)).Returns(Task.CompletedTask);

            await _examService.UpdateExamAsync(exam);

            _mockExamRepo.Verify(r => r.UpdateAsync(It.IsAny<Exam>()), Times.Once);
        }

        [TestMethod]
        public async Task Should_Delete_Exam()
        {
            _mockExamRepo.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

            await _examService.DeleteExamAsync(1);

            _mockExamRepo.Verify(r => r.DeleteAsync(1), Times.Once);
        }


        [TestMethod]
        public async Task Should_Schedule_Exam_When_Valid()
        {
            var exam = new Exam { StudentId = 1, CourseId = 1, Date = DateTime.Now, Grade = 7 };
            _mockValidator.Setup(v => v.ValidateAsync(exam, default))
                          .ReturnsAsync(new ValidationResult());
            _mockExamRepo.Setup(r => r.AddAsync(exam)).Returns(Task.CompletedTask);

            var result = await _examService.ScheduleExamAsync(exam);

            _mockValidator.Verify(v => v.ValidateAsync(exam, default), Times.Once);
            _mockExamRepo.Verify(r => r.AddAsync(exam), Times.Once);
            Assert.AreEqual(exam, result);
        }



        [TestMethod]
        public async Task UpdateExamAsync_Should_Throw_Exception_When_Validation_Fails()
        {
            var exam = new Exam { Id = 1, StudentId = 1, CourseId = 1, Grade = 8 };

            _mockValidator.Setup(v => v.ValidateAsync(exam, default))
                          .ReturnsAsync(new ValidationResult(new List<ValidationFailure>
                          {
                      new ValidationFailure("Grade", "Grade must be between 1 and 10")
                          }));

            await Assert.ThrowsExceptionAsync<ValidationException>(() =>
                _examService.UpdateExamAsync(exam));

            _mockExamRepo.Verify(r => r.UpdateAsync(It.IsAny<Exam>()), Times.Never);
        }

        [TestMethod]
        public async Task ScheduleExamAsync_Should_Throw_Exception_When_Validation_Fails()
        {
            var exam = new Exam { Id = 1, StudentId = 1, CourseId = 1, Date = DateTime.Now, Grade = 10 };

            _mockValidator.Setup(v => v.ValidateAsync(exam, default))
                          .ReturnsAsync(new ValidationResult(new List<ValidationFailure>
                          {
                      new ValidationFailure("Date", "Exam date must be in the future")
                          }));

            await Assert.ThrowsExceptionAsync<ValidationException>(() =>
                _examService.ScheduleExamAsync(exam));

            _mockExamRepo.Verify(r => r.AddAsync(It.IsAny<Exam>()), Times.Never);
        }

        [TestMethod]
        public async Task TakeExamAsync_Should_Throw_Exception_If_Student_Not_Enrolled()
        {
            var studentId = 1;
            var courseId = 1;

            _mockEnrollmentRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]); 

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _examService.TakeExamAsync(studentId, courseId, 7, DateTime.Now));

            _mockExamRepo.Verify(r => r.AddAsync(It.IsAny<Exam>()), Times.Never);
        }

        [TestMethod]
        public async Task Should_Not_Allow_More_Than_Max_Attempts()
        {
            var courseId = 1;
            var studentId = 1;

            _mockEnrollmentRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(
            [
                new() { StudentId = studentId, CourseId = courseId, TotalPaid = 100m }
            ]);

            _mockExamRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(
            [
                new() { StudentId = studentId, CourseId = courseId, Grade = 3 },
                new() { StudentId = studentId, CourseId = courseId, Grade = 4 },
                new() { StudentId = studentId, CourseId = courseId, Grade = 2 },
                 new() { StudentId = studentId, CourseId = courseId, Grade = 2 }
            ]);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _examService.TakeExamAsync(studentId, courseId, 4, DateTime.Now));
        }

        [TestMethod]
        public async Task Should_Not_Allow_Taking_Exam_After_Passing()
        {
            var courseId = 1;
            var studentId = 1;

            _mockEnrollmentRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Enrollment>
    {
        new() { StudentId = studentId, CourseId = courseId, TotalPaid = 100m }
    });

            _mockExamRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(
    [
        new Exam { StudentId = studentId, CourseId = courseId, Grade = 6 }
    ]);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _examService.TakeExamAsync(studentId, courseId, 5, DateTime.Now));

            _mockExamRepo.Verify(r => r.AddAsync(It.IsAny<Exam>()), Times.Never);
        }


        [TestMethod]
        public async Task Should_Refund_50_Percent_On_Failure()
        {
            var courseId = 1;
            var studentId = 1;
            var enrollment = new Enrollment { StudentId = studentId, CourseId = courseId, TotalPaid = 100m };

            _mockEnrollmentRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([enrollment]);
            _mockExamRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);
            _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<Exam>(), default))
                          .ReturnsAsync(new ValidationResult());

            await _examService.TakeExamAsync(studentId, courseId, 4, DateTime.Now);

            _mockEnrollmentRepo.Verify(r => r.UpdateAsync(It.Is<Enrollment>(e => e.TotalPaid == 50m)), Times.Once);
        }

    }
}
