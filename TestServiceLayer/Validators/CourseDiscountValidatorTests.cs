namespace TestServiceLayer.Validators
{
    using FluentValidation.TestHelper;
    using ServiceLayer.Validators;
    using DomainModel.Entities;
    using Moq;
    using DataAccessLayer.Repositories.Interfaces;
    using TestServiceLayer.Helpers;
    using System.Text.RegularExpressions;

    [TestClass]
    public class CourseDiscountValidatorTests
    {
        private CourseDiscountValidator _validator;
        private Mock<ICourseDiscountRepository> _mockDiscountRepo;
        private Mock<ICourseRepository> _mockCourseRepo;

        [TestInitialize]
        public void Setup()
        {
            _mockDiscountRepo = new Mock<ICourseDiscountRepository>();
            _mockCourseRepo = new Mock<ICourseRepository>();

            _validator = new CourseDiscountValidator(_mockDiscountRepo.Object, _mockCourseRepo.Object);
        }

        [TestMethod]
        public async Task Should_Not_Have_Error_For_Valid_CourseDiscount()
        {
            var discount = CourseDiscountTestHelper.CreateCourseDiscount();

            _mockCourseRepo.Setup(repo => repo.GetByIdAsync(101))
                           .ReturnsAsync(CourseTestHelper.CreateCourse());

            var result = await _validator.TestValidateAsync(discount);

            result.ShouldNotHaveAnyValidationErrors();
        }

        #region Id Tests

        public void Should_Have_Error_When_Id_Is_Invalid()
        {
            var discount = CourseDiscountTestHelper.CreateCourseDiscount(id: -1);

            var result = _validator.TestValidate(discount);
            result.ShouldHaveValidationErrorFor(s => s.Id)
                  .WithErrorMessage("ID must be a positive number.");
        }

        #endregion

        #region GroupId Tests

        [TestMethod]
        public async Task Should_Have_Error_When_GroupId_Is_Empty()
        {
            var discount = CourseDiscountTestHelper.CreateCourseDiscount(groupId: 0);

            var result = await _validator.TestValidateAsync(discount);

            result.ShouldHaveValidationErrorFor(cd => cd.GroupId)
                  .WithErrorMessage("Group ID is required.");
        }

        [TestMethod]
        public async Task Should_Have_Error_When_GroupId_Is_Not_Unique_On_Create()
        {
            var discount = CourseDiscountTestHelper.CreateCourseDiscount(groupId: 1);
            _mockDiscountRepo.Setup(r => r.ExistsGroupIdAsync(1)).ReturnsAsync(true);

            var result = await _validator.TestValidateAsync(discount);

            result.ShouldHaveValidationErrorFor(cd => cd.GroupId)
                  .WithErrorMessage("Group ID must be unique.");
        }

        [TestMethod]
        public async Task Should_Not_Have_Error_When_GroupId_Is_Unique_On_Create()
        {
            var discount = CourseDiscountTestHelper.CreateCourseDiscount(groupId: 2);
            _mockDiscountRepo.Setup(r => r.ExistsGroupIdAsync(2)).ReturnsAsync(false);

            var result = await _validator.TestValidateAsync(discount);

            result.ShouldNotHaveValidationErrorFor(cd => cd.GroupId);
        }

        #endregion

        #region CourseId Tests

        [TestMethod]
        public async Task Should_Have_Error_When_CourseId_Is_Empty()
        {
            var discount = CourseDiscountTestHelper.CreateCourseDiscount(courseId: 0);

            var result = await _validator.TestValidateAsync(discount);

            result.ShouldHaveValidationErrorFor(cd => cd.CourseId)
                  .WithErrorMessage("Course ID is required.");
        }

        [TestMethod]
        public async Task Should_Have_Error_When_CourseId_Does_Not_Exist()
        {
            var discount = CourseDiscountTestHelper.CreateCourseDiscount(courseId: 1);
            _mockCourseRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Course)null);

            var result = await _validator.TestValidateAsync(discount);

            result.ShouldHaveValidationErrorFor(cd => cd.CourseId)
                  .WithErrorMessage("Course must exist.");
        }

        [TestMethod]
        public async Task Should_Not_Have_Error_When_CourseId_Exists()
        {
            var discount = CourseDiscountTestHelper.CreateCourseDiscount(courseId: 2);
            _mockCourseRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new Course { Id = 2, Name = "Course B" });

            var result = await _validator.TestValidateAsync(discount);

            result.ShouldNotHaveValidationErrorFor(cd => cd.CourseId);
        }

        #endregion

        #region DiscountPercentage Tests

        [TestMethod]
        public async Task Should_Have_Error_When_DiscountPercentage_Is_Less_Than_0()
        {
            var discount = CourseDiscountTestHelper.CreateCourseDiscount(discountPercentage: -1);

            var result = await _validator.TestValidateAsync(discount);

            result.ShouldHaveValidationErrorFor(cd => cd.DiscountPercentage)
                  .WithErrorMessage("Discount percentage must be between 0 and 100.");
        }

        [TestMethod]
        public async Task Should_Have_Error_When_DiscountPercentage_Is_Greater_Than_100()
        {
            var discount = CourseDiscountTestHelper.CreateCourseDiscount(discountPercentage: 101);

            var result = await _validator.TestValidateAsync(discount);

            result.ShouldHaveValidationErrorFor(cd => cd.DiscountPercentage)
                  .WithErrorMessage("Discount percentage must be between 0 and 100.");
        }

        [TestMethod]
        public async Task Should_Not_Have_Error_When_DiscountPercentage_Is_Within_Range()
        {
            var discount = CourseDiscountTestHelper.CreateCourseDiscount(discountPercentage: 60);
            
            var result = await _validator.TestValidateAsync(discount);

            result.ShouldNotHaveValidationErrorFor(cd => cd.DiscountPercentage);
        }

        #endregion

        [TestMethod]
        public void Should_Access_Course()
        {
            var course = CourseTestHelper.CreateCourse(id: 101, name: "Test Course");
            var discount = CourseDiscountTestHelper.CreateCourseDiscount(courseId: 101);
            discount.Course = course;

            var courseFromDiscount = discount.Course;

            Assert.AreEqual(101, courseFromDiscount.Id); 
            Assert.AreEqual("Test Course", courseFromDiscount.Name); 
        }


    }
}
