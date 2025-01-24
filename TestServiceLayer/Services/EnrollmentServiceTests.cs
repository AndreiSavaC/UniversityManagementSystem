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
    public class EnrollmentServiceTests
    {
        private Mock<IEnrollmentRepository> _mockEnrollmentRepo;
        private Mock<IValidator<Enrollment>> _mockValidator;
        private Mock<IStudentRepository> _mockStudentRepo;
        private Mock<ICourseRepository> _mockCourseRepo;
        private Mock<ISemesterRepository> _mockSemesterRepo;
        private IEnrollmentService _enrollmentService;

        [TestInitialize]
        public void Setup()
        {
            _mockEnrollmentRepo = new Mock<IEnrollmentRepository>();
            _mockValidator = new Mock<IValidator<Enrollment>>();
            _mockStudentRepo = new Mock<IStudentRepository>();
            _mockCourseRepo = new Mock<ICourseRepository>();
            _mockSemesterRepo = new Mock<ISemesterRepository>();

            _enrollmentService = new EnrollmentService(
                _mockEnrollmentRepo.Object,
                _mockValidator.Object,
                _mockStudentRepo.Object,
                _mockCourseRepo.Object,
                _mockSemesterRepo.Object
            );
        }
        [TestMethod]
        public async Task GetEnrollmentByIdAsync_Should_Return_Enrollment_When_Found()
        {
            var enrollment = new Enrollment { Id = 1, StudentId = 1, CourseId = 1, SemesterId = 1 };
            _mockEnrollmentRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(enrollment);

            var result = await _enrollmentService.GetEnrollmentByIdAsync(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(enrollment.Id, result.Id);
            _mockEnrollmentRepo.Verify(r => r.GetByIdAsync(1), Times.Once);
        }

        [TestMethod]
        public async Task GetEnrollmentByIdAsync_Should_Return_Null_When_Not_Found()
        {
            _mockEnrollmentRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Enrollment)null);

            var result = await _enrollmentService.GetEnrollmentByIdAsync(99);

            Assert.IsNull(result);
            _mockEnrollmentRepo.Verify(r => r.GetByIdAsync(99), Times.Once);
        }
        [TestMethod]
        public async Task GetAllEnrollmentsAsync_Should_Return_All_Enrollments()
        {
            var enrollments = new List<Enrollment>
    {
        new() { Id = 1, StudentId = 1, CourseId = 1, SemesterId = 1 },
        new() { Id = 2, StudentId = 2, CourseId = 2, SemesterId = 2 }
    };
            _mockEnrollmentRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(enrollments);

            var result = await _enrollmentService.GetAllEnrollmentsAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            _mockEnrollmentRepo.Verify(r => r.GetAllAsync(), Times.Once);
        }
        [TestMethod]
        public async Task DeleteEnrollmentAsync_Should_Delete_Enrollment()
        {
            var enrollmentId = 1;

            await _enrollmentService.DeleteEnrollmentAsync(enrollmentId);

            _mockEnrollmentRepo.Verify(r => r.DeleteAsync(enrollmentId), Times.Once);
        }

        [TestMethod]
        public async Task UpdateEnrollmentAsync_Should_Update_Enrollment_When_Valid()
        {
            var enrollment = new Enrollment { Id = 1, StudentId = 1, CourseId = 1, SemesterId = 1 };
            _mockValidator.Setup(v => v.ValidateAsync(enrollment, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new ValidationResult());

            await _enrollmentService.UpdateEnrollmentAsync(enrollment);

            _mockEnrollmentRepo.Verify(r => r.UpdateAsync(enrollment), Times.Once);
        }

        [TestMethod]
        public async Task UpdateEnrollmentAsync_Should_Throw_Exception_When_Invalid()
        {
            var enrollment = new Enrollment { Id = 1, StudentId = 1, CourseId = 1, SemesterId = 1 };
            var validationErrors = new List<ValidationFailure> { new("StudentId", "Invalid Student") };
            _mockValidator.Setup(v => v.ValidateAsync(enrollment, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new ValidationResult(validationErrors));

            await Assert.ThrowsExceptionAsync<ValidationException>(() => _enrollmentService.UpdateEnrollmentAsync(enrollment));
            _mockEnrollmentRepo.Verify(r => r.UpdateAsync(enrollment), Times.Never);
        }
        [TestMethod]
        public async Task EnrollStudentInCourseAsync_Should_Enroll_Student_When_Valid()
        {
            var courseSemester = new CourseSemester
            {
                Id = 1,
                CourseId = 1,
                SemesterId = 1,
                Semester = new Semester { Id = 1, Number = 1, MinCredits = 5 }
            };

            var course = new Course
            {
                Id = 1,
                Cost = 100m,
                Credits = 5,
                MinCostPerCredit = 10m,
                MaxCostPerCredit = 20m,
                CourseSemesters = [courseSemester] 
            };

            var semester = new Semester { Id = 1, Number = 1, MinCredits = 5 };
            var student = new Student { Id = 1 };

            _mockStudentRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(student);
            _mockCourseRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(course);
            _mockSemesterRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(semester);
            _mockEnrollmentRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Enrollment>());

            _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<Enrollment>(), It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new ValidationResult());

            var result = await _enrollmentService.EnrollStudentInCourseAsync(1, 1, 1, 100m);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.StudentId);
            Assert.AreEqual(1, result.CourseId);
            Assert.AreEqual(1, result.SemesterId);
            _mockEnrollmentRepo.Verify(r => r.AddAsync(It.IsAny<Enrollment>()), Times.Once);
        }


        [TestMethod]
        public async Task Should_Create_Enrollment_When_Valid()
        {
            var enrollment = new Enrollment { StudentId = 1, CourseId = 1, SemesterId = 1, TotalPaid = 100m };
            _mockValidator.Setup(v => v.ValidateAsync(enrollment, default))
                          .ReturnsAsync(new ValidationResult());
            _mockEnrollmentRepo.Setup(r => r.AddAsync(enrollment)).Returns(Task.CompletedTask);

            var result = await _enrollmentService.CreateEnrollmentAsync(enrollment);

            _mockValidator.Verify(v => v.ValidateAsync(enrollment, default), Times.Once);
            _mockEnrollmentRepo.Verify(r => r.AddAsync(enrollment), Times.Once);
            Assert.AreEqual(enrollment, result);
        }

        [TestMethod]
        public async Task Should_Throw_ValidationException_When_Enrollment_Invalid()
        {
            var enrollment = new Enrollment { StudentId = 1, CourseId = 1, SemesterId = 1, TotalPaid = 100m };
            var validationResult = new ValidationResult(new List<ValidationFailure>
            {
                new("StudentId", "StudentId must be valid.")
            });

            _mockValidator.Setup(v => v.ValidateAsync(enrollment, default))
                          .ReturnsAsync(validationResult);

            await Assert.ThrowsExceptionAsync<ValidationException>(() => _enrollmentService.CreateEnrollmentAsync(enrollment));
            _mockEnrollmentRepo.Verify(r => r.AddAsync(It.IsAny<Enrollment>()), Times.Never);
        }

        [TestMethod]
        public async Task Should_Throw_Exception_When_Student_Does_Not_Exist()
        {
            var enrollment = new Enrollment { StudentId = 1, CourseId = 1, SemesterId = 1, TotalPaid = 100m };
            _mockStudentRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Student?)null);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _enrollmentService.EnrollStudentInCourseAsync(1, 1, 1, 100m));
        }

        [TestMethod]
        public async Task Should_Throw_Exception_When_Course_Does_Not_Exist()
        {
            var enrollment = new Enrollment { StudentId = 1, CourseId = 1, SemesterId = 1, TotalPaid = 100m };
            _mockStudentRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Student { Id = 1 });
            _mockCourseRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Course?)null);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _enrollmentService.EnrollStudentInCourseAsync(1, 1, 1, 100m));
        }


    }
}
