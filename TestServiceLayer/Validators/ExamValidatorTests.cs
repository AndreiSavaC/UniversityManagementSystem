namespace TestServiceLayer.Validators
{
    using FluentValidation.TestHelper;
    using Moq;
    using DomainModel.Entities;
    using ServiceLayer.Validators;
    using DataAccessLayer.Repositories.Interfaces;
    using TestServiceLayer.Helpers;

    [TestClass]
    public class ExamValidatorTests
    {
        private ExamValidator _validator;
        private Mock<IStudentRepository> _mockStudentRepo;
        private Mock<ICourseRepository> _mockCourseRepo;
        private Mock<IExamRepository> _mockExamRepo;

        [TestInitialize]
        public void Setup()
        {
            _mockStudentRepo = new Mock<IStudentRepository>();
            _mockCourseRepo = new Mock<ICourseRepository>();
            _mockExamRepo = new Mock<IExamRepository>();

            _validator = new ExamValidator(
                _mockStudentRepo.Object,
                _mockCourseRepo.Object,
                _mockExamRepo.Object
            );
        }

        [TestMethod]
        public async Task Should_Not_Have_Error_For_Valid_Exam()
        {
            var exam = ExamTestHelper.CreateExam(studentId: 1, courseId: 101);

            _mockStudentRepo.Setup(repo => repo.GetByIdAsync(1))
                            .ReturnsAsync(StudentTestHelper.CreateStudent());

            _mockCourseRepo.Setup(repo => repo.GetByIdAsync(101))
                           .ReturnsAsync(CourseTestHelper.CreateCourse());

            var result = await _validator.TestValidateAsync(exam);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [TestMethod]
        public async Task Should_Have_Error_When_Exam_Id_Is_Invalid()
        {
            var exam = ExamTestHelper.CreateExam(id: 0); 

            var result = await _validator.TestValidateAsync(exam);

            result.ShouldHaveValidationErrorFor(e => e.Id)
                  .WithErrorMessage("ID must be a positive number.");
        }

        [TestMethod]
        public async Task Should_Have_Error_When_Student_Does_Not_Exist()
        {
            _mockStudentRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Student?)null);

            var exam = ExamTestHelper.CreateExam(studentId: 1);

            var result = await _validator.TestValidateAsync(exam);
            result.ShouldHaveValidationErrorFor(e => e.StudentId)
                  .WithErrorMessage("Student does not exist.");
        }

        [TestMethod]
        public async Task Should_Not_Have_Error_When_Student_Exists()
        {
            _mockStudentRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(StudentTestHelper.CreateStudent(id:1));

            var exam = ExamTestHelper.CreateExam(studentId: 1);

            var result = await _validator.TestValidateAsync(exam);
            result.ShouldNotHaveValidationErrorFor(e => e.StudentId);
        }

        [TestMethod]
        public async Task Should_Not_Have_Error_When_Course_Exists()
        {
            _mockCourseRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(CourseTestHelper.CreateCourse());

            var exam = ExamTestHelper.CreateExam(courseId: 1);

            var result = await _validator.TestValidateAsync(exam);
            result.ShouldNotHaveValidationErrorFor(e => e.CourseId);
        }

        [TestMethod]
        public async Task Should_Not_Have_Error_When_Exam_Date_Is_Valid()
        {
            var exam = ExamTestHelper.CreateExam(date: DateTime.Now);

            var result = await _validator.TestValidateAsync(exam);
            result.ShouldNotHaveValidationErrorFor(e => e.Date);
        }

        [TestMethod]
        public async Task Should_Have_Error_When_Grade_Is_Out_Of_Range()
        {
            var exam = ExamTestHelper.CreateExam(grade: 11);

            var result = await _validator.TestValidateAsync(exam);
            result.ShouldHaveValidationErrorFor(e => e.Grade)
                  .WithErrorMessage("Grade must be between 1 and 10.");
        }

        [TestMethod]
        public async Task Should_Have_Error_When_Grade_Is_Below_Minimum()
        {
            var exam = ExamTestHelper.CreateExam(grade: 0); 

            var result = await _validator.TestValidateAsync(exam);

            result.ShouldHaveValidationErrorFor(e => e.Grade)
                  .WithErrorMessage("Grade must be between 1 and 10.");
        }


        [TestMethod]
        public async Task Should_Have_Error_When_Multiple_Exams_On_Same_Day()
        {
            _mockExamRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([
                ExamTestHelper.CreateExam(id:1,studentId:1,date:DateTime.Today)
            ]);

            var exam = ExamTestHelper.CreateExam(id: 2, studentId: 1, date: DateTime.Today);

            var result = await _validator.TestValidateAsync(exam);
            result.ShouldHaveValidationErrorFor(e => e)
                  .WithErrorMessage("A student cannot have multiple exams on the same day.");
        }
        [TestMethod]
        public void Should_Access_Course_From_Exam()
        {
            var course = CourseTestHelper.CreateCourse(id: 101, name: "Test Course", credits: 3);
            var exam = ExamTestHelper.CreateExam(courseId: 101);
            exam.Course = course;

            var examCourse = exam.Course;

            Assert.IsNotNull(examCourse); 
            Assert.AreEqual(101, examCourse.Id);
            Assert.AreEqual(3, examCourse.Credits); 
        }
        [TestMethod]
        public void Should_Access_Student_From_Exam()
        {
            var student = StudentTestHelper.CreateStudent(id: 1, firstName: "Test Student");
            var exam = ExamTestHelper.CreateExam(studentId: 1);
            exam.Student = student;

            var examStudent = exam.Student;

            Assert.AreEqual(1, examStudent.Id); 
            Assert.AreEqual("Test Student", examStudent.FirstName); 
        }


    }
}
