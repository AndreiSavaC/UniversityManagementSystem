namespace TestServiceLayer.Validators
{
    using FluentValidation.TestHelper;
    using ServiceLayer.Validators;
    using DomainModel.Entities;
    using TestServiceLayer.Helpers;

    [TestClass]
    public class CourseValidatorTests
    {
        private CourseValidator _validator;

        [TestInitialize]
        public void Setup()
        {
            _validator = new CourseValidator();
        }

        [TestMethod]
        public void Should_Not_Have_Error_For_Valid_Course()
        {
            var course = CourseTestHelper.CreateCourse();


            var result = _validator.TestValidate(course);
            result.ShouldNotHaveAnyValidationErrors();
        }

        #region Id Tests

        [TestMethod]
        public void Should_Have_Error_When_Id_Is_Invalid()
        {
            var course = CourseTestHelper.CreateCourse(id: -1);

            var result = _validator.TestValidate(course);
            result.ShouldHaveValidationErrorFor(s => s.Id)
                  .WithErrorMessage("ID must be a positive number.");
        }

        #endregion

        #region Name Tests

        [TestMethod]
        public void Should_Have_Error_When_Name_Is_Empty()
        {
            var course = CourseTestHelper.CreateCourse(name: "");

            var result = _validator.TestValidate(course);
            result.ShouldHaveValidationErrorFor(c => c.Name)
                  .WithErrorMessage("Course name is required.");
        }

        [TestMethod]
        public void Should_Have_Error_When_Name_Exceeds_MaxLength()
        {
            var course = CourseTestHelper.CreateCourse(name: new string('A', 101));

            var result = _validator.TestValidate(course);
            result.ShouldHaveValidationErrorFor(c => c.Name)
                  .WithErrorMessage("Course name cannot exceed 100 characters.");
        }

        [TestMethod]
        public void Should_Not_Have_Error_When_Name_Is_Valid()
        {
            var course = CourseTestHelper.CreateCourse(name: "Mathematics");

            var result = _validator.TestValidate(course);
            result.ShouldNotHaveValidationErrorFor(c => c.Name);
        }

        #endregion

        #region Description Tests

        [TestMethod]
        public void Should_Have_Error_When_Description_Is_Empty()
        {
            var course = CourseTestHelper.CreateCourse(description: "");

            var result = _validator.TestValidate(course);
            result.ShouldHaveValidationErrorFor(c => c.Description)
                  .WithErrorMessage("Course description is required.");
        }

        [TestMethod]
        public void Should_Have_Error_When_Description_Exceeds_MaxLength()
        {
            var course = CourseTestHelper.CreateCourse(description: new string('D', 501));

            var result = _validator.TestValidate(course);
            result.ShouldHaveValidationErrorFor(c => c.Description)
                  .WithErrorMessage("Course description cannot exceed 500 characters.");
        }

        [TestMethod]
        public void Should_Not_Have_Error_When_Description_Is_Valid()
        {
            var course = CourseTestHelper.CreateCourse(description: "An advanced course in mathematics.");

            var result = _validator.TestValidate(course);
            result.ShouldNotHaveValidationErrorFor(c => c.Description);
        }

        #endregion

        #region Credits Tests

        [TestMethod]
        public void Should_Have_Error_When_Credits_Less_Than_Or_Equal_To_Zero()
        {
            var course = CourseTestHelper.CreateCourse(credits: 0);

            var result = _validator.TestValidate(course);
            result.ShouldHaveValidationErrorFor(c => c.Credits)
                  .WithErrorMessage("Credits must be a positive integer.");
        }

        [TestMethod]
        public void Should_Not_Have_Error_When_Credits_Is_Positive()
        {
            var course = CourseTestHelper.CreateCourse(credits: 3);

            var result = _validator.TestValidate(course);
            result.ShouldNotHaveValidationErrorFor(c => c.Credits);
        }

        #endregion

        #region Cost Tests

        [TestMethod]
        public void Should_Have_Error_When_Cost_Is_Less_Than_Or_Equal_To_Zero()
        {
            var course = CourseTestHelper.CreateCourse(cost: 0m);

            var result = _validator.TestValidate(course);
            result.ShouldHaveValidationErrorFor(c => c.Cost)
                  .WithErrorMessage("Cost per credit must be positive.");
        }
        [TestMethod]
        public void Should_Not_Have_Error_When_Cost_Is_Positive()
        {
            var course = CourseTestHelper.CreateCourse(
                minCostPerCredit: 80m,
                maxCostPerCredit: 120m,
                credits: 3,
                cost: 200m
                );


            var result = _validator.TestValidate(course);
            result.ShouldHaveValidationErrorFor(c => c.Cost)
                  .WithErrorMessage("Cost must be in range [MinCostPerCredit * Credits, MaxCostPerCredit * Credits].");
        }

        #endregion

        #region MinCostPerCredit and MaxCostPerCredit Tests

        [TestMethod]
        public void Should_Have_Error_When_MinCostPerCredit_Greater_Than_MaxCostPerCredit()
        {
            var course = CourseTestHelper.CreateCourse(
                minCostPerCredit: 150m,
                maxCostPerCredit: 100m
                );

            var result = _validator.TestValidate(course);
            result.ShouldHaveValidationErrorFor(c => c)
                  .WithErrorMessage("Minimum cost per credit must be less than or equal to maximum cost per credit.");
        }

        [TestMethod]
        public void Should_Not_Have_Error_When_MinCostPerCredit_Less_Than_Or_Equal_To_MaxCostPerCredit()
        {
            // Arrange
            var course1 = new Course
            {
                MinCostPerCredit = 80m,
                MaxCostPerCredit = 120m
            };

            var course2 = new Course
            {
                MinCostPerCredit = 100m,
                MaxCostPerCredit = 100m
            };

            // Act & Assert
            var result1 = _validator.TestValidate(course1);
            result1.ShouldNotHaveValidationErrorFor(c => c);

            var result2 = _validator.TestValidate(course2);
            result2.ShouldNotHaveValidationErrorFor(c => c);
        }

        #endregion

        #region Cost Range Tests

        [TestMethod]
        public void Should_Have_Error_When_Cost_Less_Than_MinCostPerCredit_Multiplied_By_Credits()
        {
            var course = CourseTestHelper.CreateCourse(
                credits: 3,
                minCostPerCredit: 80m,
                maxCostPerCredit: 120m,
                cost: 30m
                );

            var result = _validator.TestValidate(course);
            result.ShouldHaveValidationErrorFor(c => c.Cost)
                  .WithErrorMessage("Cost must be in range [MinCostPerCredit * Credits, MaxCostPerCredit * Credits].");
        }

        [TestMethod]
        public void Should_Have_Error_When_Cost_Greater_Than_MaxCostPerCredit_Multiplied_By_Credits()
        {
            var course = CourseTestHelper.CreateCourse(
                credits: 3,
                minCostPerCredit: 80m,
                maxCostPerCredit: 120m,
                cost: 400m
                );

            var result = _validator.TestValidate(course);
            result.ShouldHaveValidationErrorFor(c => c.Cost)
                  .WithErrorMessage("Cost must be in range [MinCostPerCredit * Credits, MaxCostPerCredit * Credits].");
        }

        [TestMethod]
        public void Should_Not_Have_Error_When_Cost_Is_Within_Range()
        {
            var course1 = CourseTestHelper.CreateCourse(
                credits: 3,
                minCostPerCredit: 80m,
                maxCostPerCredit: 120m,
                cost: 240m
                );

            var course2 = CourseTestHelper.CreateCourse(
                credits: 3,
                minCostPerCredit: 80m,
                maxCostPerCredit: 120m,
                cost: 300m
                );

            var course3 = CourseTestHelper.CreateCourse(
                credits: 3,
                minCostPerCredit: 80m,
                maxCostPerCredit: 120m,
                cost: 360m
                );
           
            var result1 = _validator.TestValidate(course1);
            result1.ShouldNotHaveValidationErrorFor(c => c.Cost);

            var result2 = _validator.TestValidate(course2);
            result2.ShouldNotHaveValidationErrorFor(c => c.Cost);

            var result3 = _validator.TestValidate(course3);
            result3.ShouldNotHaveValidationErrorFor(c => c.Cost);
        }

        #endregion

        #region Combined Validations Tests

        [TestMethod]
        public void Should_Have_Errors_For_Multiple_Invalid_Fields()
        {

            var course = CourseTestHelper.CreateCourse(
                name: "",
                description: new string('D', 501), 
                credits: -1,
                cost: 100m,
                minCostPerCredit: 150m,
                maxCostPerCredit: 100m 
            );

            var result = _validator.TestValidate(course);
            result.ShouldHaveValidationErrorFor(c => c.Name)
                  .WithErrorMessage("Course name is required.");
            result.ShouldHaveValidationErrorFor(c => c.Description)
                  .WithErrorMessage("Course description cannot exceed 500 characters.");
            result.ShouldHaveValidationErrorFor(c => c.Credits)
                  .WithErrorMessage("Credits must be a positive integer.");
            result.ShouldHaveValidationErrorFor(c => c)
                  .WithErrorMessage("Minimum cost per credit must be less than or equal to maximum cost per credit.");
            result.ShouldHaveValidationErrorFor(c => c.Cost)
                  .WithErrorMessage("Cost must be in range [MinCostPerCredit * Credits, MaxCostPerCredit * Credits].");
        }

        #endregion

        [TestMethod]
        public void Should_Access_Exams()
        {
            var exam = ExamTestHelper.CreateExam();
            var course = CourseTestHelper.CreateCourse();
            course.Exams = [exam];

            var courseExams = course.Exams;

            var result = _validator.TestValidate(course);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [TestMethod]
        public void Should_Access_Enrollments()
        {
            var enrollment = EnrollmentTestHelper.CreateEnrollment();

            var course = CourseTestHelper.CreateCourse();
            course.Enrollments = [enrollment];

            var courseEnrollments = course.Enrollments;

            Assert.AreEqual(1, courseEnrollments.Count);
            Assert.AreEqual(101, courseEnrollments.First().CourseId);
        }

        [TestMethod]
        public void Should_Access_CoursePrerequisite()
        {
            var coursePrerequisite = CoursePrerequisiteTestHelper.CreateCoursePrerequisite();

            var course = CourseTestHelper.CreateCourse();
            course.Prerequisites =  [coursePrerequisite ];

            var dependentCourse = new List<CoursePrerequisite> { coursePrerequisite };
            course.DependentCourses = dependentCourse;

            var coursePrerequisites = course.Prerequisites;
            var dependentCourses = course.DependentCourses;

            Assert.AreEqual(1, coursePrerequisites.Count);
            Assert.AreEqual(101, coursePrerequisites.First().CourseId);

            Assert.AreEqual(1, dependentCourses.Count);
            Assert.AreEqual(101, dependentCourses.First().CourseId);
        }

        [TestMethod]
        public void Should_Access_CourseDiscount()
        {
            var courseDiscount = CourseDiscountTestHelper.CreateCourseDiscount();

            var course = CourseTestHelper.CreateCourse();
            course.CourseDiscounts = [courseDiscount];

            var courseDiscounts = course.CourseDiscounts;

            Assert.AreEqual(1, courseDiscounts.Count);
            Assert.AreEqual(101, courseDiscounts.First().CourseId);
        }

    }
}
