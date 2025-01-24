namespace TestServiceLayer.Validators
{
    using FluentValidation.TestHelper;
    using Moq;
    using DomainModel.Entities;
    using ServiceLayer.Validators;
    using DataAccessLayer.Repositories.Interfaces;
    using TestServiceLayer.Helpers;

    [TestClass]
    public class EnrollmentValidatorTests
    {
        private EnrollmentValidator _validator;
        private Mock<IStudentRepository> _mockStudentRepo;
        private Mock<ICourseRepository> _mockCourseRepo;
        private Mock<ISemesterRepository> _mockSemesterRepo;

        [TestInitialize]
        public void Setup()
        {
            _mockStudentRepo = new Mock<IStudentRepository>();
            _mockCourseRepo = new Mock<ICourseRepository>();
            _mockSemesterRepo = new Mock<ISemesterRepository>();
            var _mockEnrollmentRepo = new Mock<IEnrollmentRepository>(); 

            _validator = new EnrollmentValidator(
                _mockStudentRepo.Object,
                _mockCourseRepo.Object,
                _mockSemesterRepo.Object,
                _mockEnrollmentRepo.Object  
            );
        }


        [TestMethod]
        public async Task Should_Not_Have_Error_For_Valid_Enrollment()
        {
            var enrollment = EnrollmentTestHelper.CreateEnrollment(studentId: 1, courseId: 101, semesterId: 2);

            var student = StudentTestHelper.CreateStudent(id: 1, firstName: "John", lastName: "Doe");
            var course = CourseTestHelper.CreateCourse(id: 101, name: "Mathematics", credits: 3);
            var semester = new Semester { Id = 2, Number = 2 };

            _mockStudentRepo.Setup(r => r.GetByIdAsync(enrollment.StudentId)).ReturnsAsync(student);
            _mockCourseRepo.Setup(r => r.GetByIdAsync(enrollment.CourseId)).ReturnsAsync(course);
            _mockSemesterRepo.Setup(r => r.GetByIdAsync(enrollment.SemesterId)).ReturnsAsync(semester);

            var result = await _validator.TestValidateAsync(enrollment);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [TestMethod]
        public async Task Should_Have_Error_When_Enrollment_Id_Is_Invalid()
        {
            var enrollment = EnrollmentTestHelper.CreateEnrollment(id: 0, studentId: 1, courseId: 101, semesterId: 2);

            var student = StudentTestHelper.CreateStudent(id: 1, firstName: "John", lastName: "Doe");
            var course = CourseTestHelper.CreateCourse(id: 101, name: "Mathematics", credits: 3);
            var semester = SemesterTestHelper.CreateSemester(id: 2, number: 2);
            
            _mockStudentRepo.Setup(r => r.GetByIdAsync(enrollment.StudentId)).ReturnsAsync(student);
            _mockCourseRepo.Setup(r => r.GetByIdAsync(enrollment.CourseId)).ReturnsAsync(course);
            _mockSemesterRepo.Setup(r => r.GetByIdAsync(enrollment.SemesterId)).ReturnsAsync(semester);

            var result = await _validator.TestValidateAsync(enrollment);

            result.ShouldHaveValidationErrorFor(e => e.Id)
                  .WithErrorMessage("ID is required.");
        }


        [TestMethod]
        public async Task Should_Have_Error_When_Student_Does_Not_Exist()
        {
            var enrollment = EnrollmentTestHelper.CreateEnrollment(studentId: 1);

            _mockStudentRepo.Setup(r => r.GetByIdAsync(enrollment.StudentId)).ReturnsAsync((Student?)null);

            var result = await _validator.TestValidateAsync(enrollment);
            result.ShouldHaveValidationErrorFor(e => e.StudentId)
                  .WithErrorMessage("Student does not exist.");
        }


        [TestMethod]
        public async Task Should_Not_Have_Error_When_Student_Exists()
        {
            var enrollment = EnrollmentTestHelper.CreateEnrollment(studentId: 1);

            _mockStudentRepo.Setup(r => r.GetByIdAsync(enrollment.StudentId)).ReturnsAsync(new Student { Id = 1 });

            var result = await _validator.TestValidateAsync(enrollment);
            result.ShouldNotHaveValidationErrorFor(e => e.StudentId);
        }

        [TestMethod]
        public async Task Should_Have_Error_When_Course_Does_Not_Exist()
        {
            var enrollment = EnrollmentTestHelper.CreateEnrollment(courseId: 1);

            _mockCourseRepo.Setup(r => r.GetByIdAsync(enrollment.CourseId)).ReturnsAsync((Course?)null);

            var result = await _validator.TestValidateAsync(enrollment);
            result.ShouldHaveValidationErrorFor(e => e.CourseId)
                  .WithErrorMessage("Course does not exist.");
        }

        [TestMethod]
        public async Task Should_Not_Have_Error_When_Course_Exists()
        {
            var enrollment = EnrollmentTestHelper.CreateEnrollment(courseId: 1);

            _mockCourseRepo.Setup(r => r.GetByIdAsync(enrollment.CourseId)).ReturnsAsync(new Course { Id = 1 });

            var result = await _validator.TestValidateAsync(enrollment);
            result.ShouldNotHaveValidationErrorFor(e => e.CourseId);
        }

        [TestMethod]
        public async Task Should_Have_Error_When_Semester_Does_Not_Exist()
        {
            _mockSemesterRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Semester?)null);

            var enrollment = new Enrollment { SemesterId = 1 };

            var result = await _validator.TestValidateAsync(enrollment);
            result.ShouldHaveValidationErrorFor(e => e.SemesterId)
                  .WithErrorMessage("Semester does not exist.");
        }

        [TestMethod]
        public async Task Should_Not_Have_Error_When_Semester_Exists()
        {
            _mockSemesterRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Semester { Id = 1 });

            var enrollment = new Enrollment { SemesterId = 1 };

            var result = await _validator.TestValidateAsync(enrollment);
            result.ShouldNotHaveValidationErrorFor(e => e.SemesterId);
        }

        [TestMethod]
        public async Task Should_Have_Error_When_TotalPaid_Is_Negative()
        {
            var enrollment = new Enrollment { TotalPaid = -100m };

            var result = await _validator.TestValidateAsync(enrollment);
            result.ShouldHaveValidationErrorFor(e => e.TotalPaid)
                  .WithErrorMessage("TotalPaid must be non-negative.");
        }

        [TestMethod]
        public async Task Should_Not_Have_Error_When_TotalPaid_Is_Non_Negative()
        {
            var enrollment = new Enrollment { TotalPaid = 100m };

            var result = await _validator.TestValidateAsync(enrollment);
            result.ShouldNotHaveValidationErrorFor(e => e.TotalPaid);
        }

        [TestMethod]
        public void Should_Access_Course_From_Enrollment()
        {
            var course = CourseTestHelper.CreateCourse(id: 101, name: "Test Course", credits: 3);
            var enrollment = EnrollmentTestHelper.CreateEnrollment(courseId: 101);
            enrollment.Course = course;

            var enrolledCourse = enrollment.Course;

            Assert.AreEqual(101, enrolledCourse.Id);
            Assert.AreEqual("Test Course", enrolledCourse.Name);
            Assert.AreEqual(3, enrolledCourse.Credits);
        }

        [TestMethod]
        public void Should_Access_Student_From_Enrollment()
        {
            var student = StudentTestHelper.CreateStudent(id: 1, firstName: "Test Student");
            var enrollment = EnrollmentTestHelper.CreateEnrollment(studentId: 1);
            enrollment.Student = student;

            var enrolledStudent = enrollment.Student;

            Assert.AreEqual(1, enrolledStudent.Id);
            Assert.AreEqual("Test Student", enrolledStudent.FirstName);
        }

        [TestMethod]
        public void Should_Access_Semester_From_Enrollment()
        {
            var semester = SemesterTestHelper.CreateSemester(id: 2, number: 2);
            var enrollment = EnrollmentTestHelper.CreateEnrollment(semesterId: 2);
            enrollment.Semester = semester;

            var enrolledSemester = enrollment.Semester;

            Assert.AreEqual(2, enrolledSemester.Id);
            Assert.AreEqual(2, enrolledSemester.Number);
        }
    }
}
