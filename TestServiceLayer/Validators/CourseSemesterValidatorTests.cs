namespace TestServiceLayer.Validators
{
    using FluentValidation.TestHelper;
    using ServiceLayer.Validators;
    using DomainModel.Entities;
    using Moq;
    using DataAccessLayer.Repositories.Interfaces;
    using TestServiceLayer.Helpers;
    [TestClass]
    public class CourseSemesterValidatorTests
    {
        private CourseSemesterValidator _validator;
        private Mock<ICourseRepository> _mockCourseRepo;
        private Mock<ISemesterRepository> _mockSemesterRepo;

        [TestInitialize]
        public void Setup()
        {
            _mockCourseRepo = new Mock<ICourseRepository>();
            _mockSemesterRepo = new Mock<ISemesterRepository>();

            _validator = new CourseSemesterValidator(_mockCourseRepo.Object, _mockSemesterRepo.Object);
        }

        #region Id Tests

        [TestMethod]
        public async Task Should_Have_Error_When_Id_Is_Invalid()
        {
            var courseSemester = CourseSemesterTestHelper.CreateCourseSemester(id: -1);

            var result = await _validator.TestValidateAsync(courseSemester);
            result.ShouldHaveValidationErrorFor(s => s.Id)
                  .WithErrorMessage("ID must be a positive number.");
        }

        #endregion

        #region CourseId Tests

        [TestMethod]
        public async Task Should_Not_Have_Error_For_Valid_CourseId()
        {
            _mockCourseRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(CourseTestHelper.CreateCourse(id: 1));

            var courseSemester = CourseSemesterTestHelper.CreateCourseSemester(courseId: 1);

            var result = await _validator.TestValidateAsync(courseSemester);

            result.ShouldNotHaveValidationErrorFor(cs => cs.CourseId);
        }



        [TestMethod]
        public async Task Should_Have_Error_When_CourseId_Is_Invalid()
        {
            var courseSemester = CourseSemesterTestHelper.CreateCourseSemester(courseId: 0);

            var result = await _validator.TestValidateAsync(courseSemester);

            result.ShouldHaveValidationErrorFor(cs => cs.CourseId)
                  .WithErrorMessage("CourseId must be greater than 0.");
        }

        [TestMethod]
        public async Task Should_Have_Error_When_Course_Does_Not_Exist()
        {
            _mockCourseRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Course)null);

            var courseSemester = CourseSemesterTestHelper.CreateCourseSemester(courseId: 1);

            var result = await _validator.TestValidateAsync(courseSemester);

            result.ShouldHaveValidationErrorFor(cs => cs.CourseId)
                  .WithErrorMessage("Course does not exist.");
        }

        #endregion

        #region SemesterId Tests

        [TestMethod]
        public async Task Should_Not_Have_Error_For_Valid_SemesterId()
        {
            _mockSemesterRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(SemesterTestHelper.CreateSemester(id: 1));

            var courseSemester = CourseSemesterTestHelper.CreateCourseSemester(semesterId: 1);

            var result = await _validator.TestValidateAsync(courseSemester);

            result.ShouldNotHaveValidationErrorFor(cs => cs.SemesterId);
        }

        [TestMethod]
        public async Task Should_Have_Error_When_SemesterId_Is_Invalid()
        {
            var courseSemester = CourseSemesterTestHelper.CreateCourseSemester(semesterId: 0);

            var result = await _validator.TestValidateAsync(courseSemester);

            result.ShouldHaveValidationErrorFor(cs => cs.SemesterId)
                  .WithErrorMessage("SemesterId must be greater than 0.");
        }

        [TestMethod]
        public async Task Should_Have_Error_When_Semester_Does_Not_Exist()
        {
            _mockSemesterRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Semester)null);

            var courseSemester = CourseSemesterTestHelper.CreateCourseSemester(semesterId: 1);

            var result = await _validator.TestValidateAsync(courseSemester);

            result.ShouldHaveValidationErrorFor(cs => cs.SemesterId)
                  .WithErrorMessage("Semester does not exist.");
        }

        #endregion

        [TestMethod]
        public void Should_Access_Course_From_CourseSemester()
        {
            var course = CourseTestHelper.CreateCourse(id: 1, name: "Test Course", credits: 3);
            var courseSemester = CourseSemesterTestHelper.CreateCourseSemester(courseId: 1);
            courseSemester.Course = course;

            var associatedCourse = courseSemester.Course;

            Assert.AreEqual(1, associatedCourse.Id); 
            Assert.AreEqual("Test Course", associatedCourse.Name);
            Assert.AreEqual(3, associatedCourse.Credits);
        }

    }
}
