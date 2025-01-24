namespace TestServiceLayer.Validators
{
    using FluentValidation.TestHelper;
    using ServiceLayer.Validators;
    using DomainModel.Entities;
    using Moq;
    using DataAccessLayer.Repositories.Interfaces;
    using TestServiceLayer.Helpers;


    [TestClass]
    public class SemesterValidatorTests
    {
        private SemesterValidator _validator;
        private Mock<ISemesterRepository> _mockRepo;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<ISemesterRepository>();
            _validator = new SemesterValidator(_mockRepo.Object);
        }


        [TestMethod]
        public async Task Should_Not_Have_Error_For_Valid_Semester()
        {
            var semester = SemesterTestHelper.CreateSemester();

            var result = await _validator.TestValidateAsync(semester);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [TestMethod]
        public async Task Should_Have_Error_When_Semester_Id_Is_Invalid()
        {
            var semester = SemesterTestHelper.CreateSemester(id:0);


            var result = await _validator.TestValidateAsync(semester);

            result.ShouldHaveValidationErrorFor(e => e.Id)
                  .WithErrorMessage("ID must be a positive number.");
        }

        #region Number Tests

        [TestMethod]
        public async Task Should_Have_Error_When_Number_Is_Empty()
        {
            var semester = SemesterTestHelper.CreateSemester(number: 0);

            var result = await _validator.TestValidateAsync(semester);

            result.ShouldHaveValidationErrorFor(s => s.Number)
                  .WithErrorMessage("Semester number is required.");
        }

        [TestMethod]
        public async Task Should_Have_Error_When_Number_Is_Less_Than_1()
        {
            var semester = SemesterTestHelper.CreateSemester(number: -1);
            
            var result = await _validator.TestValidateAsync(semester);

            result.ShouldHaveValidationErrorFor(s => s.Number)
                  .WithErrorMessage("Semester number must be a positive integer.");
        }

        [TestMethod]
        public async Task Should_Not_Have_Error_When_Number_Is_Valid()
        {
            var semester = SemesterTestHelper.CreateSemester(number: 2);

            var result = await _validator.TestValidateAsync(semester);

            result.ShouldNotHaveValidationErrorFor(s => s.Number);
        }

        #endregion

        #region MinCredits Tests

        [TestMethod]
        public async Task Should_Have_Error_When_MinCredits_Is_Negative()
        {
            var semester = SemesterTestHelper.CreateSemester(minCredits: -1);

            var result = await _validator.TestValidateAsync(semester);

            result.ShouldHaveValidationErrorFor(s => s.MinCredits)
                  .WithErrorMessage("Minimum credits must be non-negative.");
        }

        [TestMethod]
        public async Task Should_Not_Have_Error_When_MinCredits_Is_Valid()
        {
            var semester = SemesterTestHelper.CreateSemester(minCredits: 15);

            var result = await _validator.TestValidateAsync(semester);

            result.ShouldNotHaveValidationErrorFor(s => s.MinCredits);
        }

        #endregion

        #region Number Uniqueness Tests

        [TestMethod]
        public async Task Should_Have_Error_When_Number_Is_Not_Unique_On_Create()
        {
            var semester = SemesterTestHelper.CreateSemester(id: 1,number:1);
          
            _mockRepo.Setup(r => r.GetByNumberAsync(1)).ReturnsAsync(SemesterTestHelper.CreateSemester(id: 2, number: 1));

            var result = await _validator.TestValidateAsync(semester);

            result.ShouldHaveValidationErrorFor(s => s.Number)
                  .WithErrorMessage("Semester number must be unique.");
        }

        [TestMethod]
        public async Task Should_Not_Have_Error_When_Number_Is_Unique_On_Create()
        {
            var semester = SemesterTestHelper.CreateSemester(id: 1, number: 2);

            _mockRepo.Setup(r => r.GetByNumberAsync(2)).ReturnsAsync((Semester)null);

            var result = await _validator.TestValidateAsync(semester);

            result.ShouldNotHaveValidationErrorFor(s => s.Number);
        }

        [TestMethod]
        public async Task Should_Not_Have_Error_When_Number_Is_Same_On_Update()
        {
            var semester = SemesterTestHelper.CreateSemester(id: 1, number: 1);

            _mockRepo.Setup(r => r.GetByNumberAsync(1)).ReturnsAsync(SemesterTestHelper.CreateSemester(id: 1, number: 1));

            var result = await _validator.TestValidateAsync(semester);

            result.ShouldNotHaveValidationErrorFor(s => s.Number);
        }

        [TestMethod]
        public async Task Should_Have_Error_When_Number_Is_Not_Unique_On_Update()
        {
            var semester = SemesterTestHelper.CreateSemester(id: 2, number: 1);

            _mockRepo.Setup(r => r.GetByNumberAsync(1)).ReturnsAsync(SemesterTestHelper.CreateSemester(id: 1, number: 1));

            var result = await _validator.TestValidateAsync(semester);

            result.ShouldHaveValidationErrorFor(s => s.Number)
                  .WithErrorMessage("Semester number must be unique.");
        }

        #endregion

        [TestMethod]
        public void Should_Access_CourseSemesters_From_Semester()
        {
            var courseSemester = CourseSemesterTestHelper.CreateCourseSemester(courseId: 1, semesterId: 1);
            var semester = SemesterTestHelper.CreateSemester(id: 1);
            semester.CourseSemesters = [ courseSemester ];

            var semesterCourseSemesters = semester.CourseSemesters;

            Assert.AreEqual(1, semesterCourseSemesters.Count); 
            Assert.AreEqual(1, semesterCourseSemesters.First().CourseId); 
            Assert.AreEqual(1, semesterCourseSemesters.First().SemesterId); 
        }

        [TestMethod]
        public void Should_Access_Enrollments_From_Semester()
        {
            var enrollment = EnrollmentTestHelper.CreateEnrollment(studentId: 1, courseId: 1, semesterId: 1);
            var semester = SemesterTestHelper.CreateSemester(id: 1);
            semester.Enrollments = [ enrollment ];

            var semesterEnrollments = semester.Enrollments;

            Assert.AreEqual(1, semesterEnrollments.Count); 
            Assert.AreEqual(1, semesterEnrollments.First().StudentId); 
            Assert.AreEqual(1, semesterEnrollments.First().CourseId);
            Assert.AreEqual(1, semesterEnrollments.First().SemesterId); 
        }


    }
}