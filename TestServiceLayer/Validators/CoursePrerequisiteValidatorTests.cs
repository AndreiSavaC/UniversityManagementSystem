// TestServiceLayer.Validators/CoursePrerequisiteValidatorTests.cs
using FluentValidation.TestHelper;
using DomainModel.Entities;
using Moq;
using DataAccessLayer.Repositories.Interfaces;
using ServiceLayer.Validators;
using TestServiceLayer.Helpers;

namespace TestServiceLayer.Validators
{
    [TestClass]
    public class CoursePrerequisiteValidatorTests
    {
        private CoursePrerequisiteValidator _validator;
        private Mock<ICoursePrerequisiteRepository> _mockPrereqRepo;
        private Mock<ICourseRepository> _mockCourseRepo;

        [TestInitialize]
        public void Setup()
        {
            _mockPrereqRepo = new Mock<ICoursePrerequisiteRepository>();
            _mockCourseRepo = new Mock<ICourseRepository>();

            _validator = new CoursePrerequisiteValidator(_mockPrereqRepo.Object, _mockCourseRepo.Object);
        }

        [TestMethod]
        public async Task Should_Not_Have_Error_For_Valid_CoursePrerequisite()
        {

            var coursePrerequisite = CoursePrerequisiteTestHelper.CreateCoursePrerequisite();

            _mockCourseRepo.Setup(r => r.GetByIdAsync(coursePrerequisite.CourseId)).ReturnsAsync(new Course
            {
                Id = coursePrerequisite.CourseId,
                Name = "Course A",
                CourseSemesters =
                [
                    new CourseSemester { Semester = new Semester { Id = 2, Number = 2 } }
                ]
            });

            _mockCourseRepo.Setup(r => r.GetByIdAsync(coursePrerequisite.PrereqId)).ReturnsAsync(new Course
            {
                Id = coursePrerequisite.PrereqId,
                Name = "Course B",
                CourseSemesters =
                [
                    new CourseSemester { Semester = new Semester { Id = 1, Number = 1 } }
                ]
            });

            _mockPrereqRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

            var result = await _validator.TestValidateAsync(coursePrerequisite);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [TestMethod]
        public async Task Should_Have_Error_When_Id_Is_Invalid()
        {
            var cp = new CoursePrerequisite { Id = 0, CourseId = 1, PrereqId = 2, MinGrade = 5 };

            _mockCourseRepo.Setup(r => r.GetByIdAsync(cp.CourseId)).ReturnsAsync(new Course
            {
                Id = cp.CourseId,
                Name = "Course A",
                CourseSemesters = [
            new CourseSemester { Semester = new Semester { Id = 2, Number = 2 } }
        ]
            });

            _mockCourseRepo.Setup(r => r.GetByIdAsync(cp.PrereqId)).ReturnsAsync(new Course
            {
                Id = cp.PrereqId,
                Name = "Course B",
                CourseSemesters =
        [
            new CourseSemester { Semester = new Semester { Id = 1, Number = 1 } }
        ]
            });

            _mockPrereqRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

            var result = await _validator.TestValidateAsync(cp);

            result.ShouldHaveValidationErrorFor(cp => cp.Id)
                  .WithErrorMessage("ID is required.");
        }


        #region CourseId Tests

        [TestMethod]
        public async Task Should_Have_Error_When_CourseId_Is_Empty()
        {
            var coursePrerequisite = CoursePrerequisiteTestHelper.CreateCoursePrerequisite(courseId:0,prereqId:1,minGrade:5);

            _mockPrereqRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

            var result = await _validator.TestValidateAsync(coursePrerequisite);

            result.ShouldHaveValidationErrorFor(cp => cp.CourseId)
                  .WithErrorMessage("Course ID is required.");
        }

        [TestMethod]
        public async Task Should_Have_Error_When_CourseId_Less_Than_1()
        {
            var coursePrerequisite = CoursePrerequisiteTestHelper.CreateCoursePrerequisite(courseId: -1, prereqId: 1, minGrade: 5);

            _mockPrereqRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

            var result = await _validator.TestValidateAsync(coursePrerequisite);

            result.ShouldHaveValidationErrorFor(cp => cp.CourseId)
                  .WithErrorMessage("Course ID must be greater than 0.");
        }

        [TestMethod]
        public async Task Should_Have_Error_When_Course_Does_Not_Exist()
        {
            var coursePrerequisite = CoursePrerequisiteTestHelper.CreateCoursePrerequisite(courseId: 1, prereqId: 2, minGrade: 5);

            _mockCourseRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Course?)null);

            _mockPrereqRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

            var result = await _validator.TestValidateAsync(coursePrerequisite);

            result.ShouldHaveValidationErrorFor(cp => cp.CourseId)
                  .WithErrorMessage("Course must exist.");
        }

        [TestMethod]
        public async Task Should_Not_Have_Error_When_Course_Does_Exist()
        {
            var coursePrerequisite = CoursePrerequisiteTestHelper.CreateCoursePrerequisite(courseId: 1, prereqId: 2, minGrade: 5);

            _mockCourseRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Course
            {
                Id = 1,
                Name = "Course A",
                CourseSemesters = [
                    new CourseSemester { Semester = new Semester { Id = 2, Number = 2 } }
                ]
            });

            _mockPrereqRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

            var result = await _validator.TestValidateAsync(coursePrerequisite);

            result.ShouldNotHaveValidationErrorFor(cp => cp.CourseId);
        }

        #endregion

        #region PrereqId Tests

        [TestMethod]
        public async Task Should_Have_Error_When_PrereqId_Is_Empty()
        {
            var coursePrerequisite = CoursePrerequisiteTestHelper.CreateCoursePrerequisite(courseId: 1, prereqId: 0, minGrade: 5);

            _mockPrereqRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

            var result = await _validator.TestValidateAsync(coursePrerequisite);

            result.ShouldHaveValidationErrorFor(cp => cp.PrereqId)
                  .WithErrorMessage("Prerequisite Course ID is required.");
        }

        [TestMethod]
        public async Task Should_Have_Error_When_PrereqId_Less_Than_1()
        {
            var coursePrerequisite = CoursePrerequisiteTestHelper.CreateCoursePrerequisite(courseId: 1, prereqId: -1, minGrade: 5);

            _mockPrereqRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

            var result = await _validator.TestValidateAsync(coursePrerequisite);

            result.ShouldHaveValidationErrorFor(cp => cp.PrereqId)
                  .WithErrorMessage("Prerequisite Course ID must be greater than 0.");
        }

        [TestMethod]
        public async Task Should_Have_Error_When_PrerequisiteCourse_Does_Not_Exist()
        {
            var coursePrerequisite = CoursePrerequisiteTestHelper.CreateCoursePrerequisite(courseId: 1, prereqId: 2, minGrade: 5);

            _mockCourseRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync((Course?)null);

            _mockPrereqRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

            var result = await _validator.TestValidateAsync(coursePrerequisite);

            result.ShouldHaveValidationErrorFor(cp => cp.PrereqId)
                  .WithErrorMessage("Prerequisite Course must exist.");
        }

        [TestMethod]
        public async Task Should_Not_Have_Error_When_PrerequisiteCourse_Exists()
        {
            var coursePrerequisite = CoursePrerequisiteTestHelper.CreateCoursePrerequisite(courseId: 1, prereqId: 2, minGrade: 5);

            _mockCourseRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new Course
            {
                Id = 2,
                Name = "Course B",
                CourseSemesters =
                [
                    new CourseSemester { Semester = new Semester { Id = 1, Number = 1 } }
                ]
            });

            _mockPrereqRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

            var result = await _validator.TestValidateAsync(coursePrerequisite);

            result.ShouldNotHaveValidationErrorFor(cp => cp.PrereqId);
        }

        #endregion

        #region Same Course and Prereq Tests

        [TestMethod]
        public async Task Should_Have_Error_When_CourseId_Equals_PrereqId()
        {
            var coursePrerequisite = CoursePrerequisiteTestHelper.CreateCoursePrerequisite(courseId: 1, prereqId: 1, minGrade: 5);

            _mockCourseRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Course
            {
                Id = 1,
                Name = "Course A",
                CourseSemesters =
                [
                    new CourseSemester { Semester = new Semester { Id = 2, Number = 2 } }
                ]
            });

            _mockPrereqRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

            var result = await _validator.TestValidateAsync(coursePrerequisite);

            result.ShouldHaveValidationErrorFor(cp => cp)
                  .WithErrorMessage("Course cannot be a prerequisite of itself.");
        }

        [TestMethod]
        public async Task Should_Not_Have_Error_When_CourseId_Does_Not_Equals_PrereqId()
        {
            var coursePrerequisite = CoursePrerequisiteTestHelper.CreateCoursePrerequisite(courseId: 1, prereqId:2, minGrade: 5);

            _mockCourseRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Course
            {
                Id = 1,
                Name = "Course A",
                CourseSemesters =
                [
                    new CourseSemester { Semester = new Semester { Id = 2, Number = 2 } }
                ]
            });
            _mockCourseRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new Course
            {
                Id = 2,
                Name = "Course B",
                CourseSemesters =
                [
                    new CourseSemester { Semester = new Semester { Id = 1, Number = 1 } }
                ]
            });

            _mockPrereqRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

            var result = await _validator.TestValidateAsync(coursePrerequisite);

            result.ShouldNotHaveValidationErrorFor(cp => cp);
        }

        #endregion

        #region Semester Tests

        [TestMethod]
        public async Task Should_Have_Error_When_Course_Is_In_Semester_Less_Than_Two()
        {
            var coursePrerequisite = CoursePrerequisiteTestHelper.CreateCoursePrerequisite(courseId: 1, prereqId: 2, minGrade: 5);

            _mockCourseRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Course
            {
                Id = 1,
                Name = "Course A",
                CourseSemesters =
                [
                    new CourseSemester { Semester = new Semester { Id = 1, Number = 1 } }
                ]
            });

            _mockPrereqRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

            var result = await _validator.TestValidateAsync(coursePrerequisite);

            result.ShouldHaveValidationErrorFor(cp => cp)
                  .WithErrorMessage("Only courses from semester 2 or higher can have prerequisites.");
        }

        [TestMethod]
        public async Task Should_Not_Have_Error_When_Course_Is_In_Semester_At_Least_Two()
        {
            var coursePrerequisite = CoursePrerequisiteTestHelper.CreateCoursePrerequisite(courseId: 1, prereqId: 2, minGrade: 5);

            _mockCourseRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Course
            {
                Id = 1,
                Name = "Course A",
                CourseSemesters =
        [
            new CourseSemester { Semester = new Semester { Id = 2, Number = 2 } },
            new CourseSemester { Semester = new Semester { Id = 3, Number = 3 } }
        ]
            });

            _mockCourseRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new Course
            {
                Id = 2,
                Name = "Course B",
                CourseSemesters =
        [
            new CourseSemester { Semester = new Semester { Id = 1, Number = 1 } }
        ]
            });

            _mockPrereqRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

            var result = await _validator.TestValidateAsync(coursePrerequisite);

            result.ShouldNotHaveValidationErrorFor(cp => cp);
        }


        #endregion

        #region Prerequisite Semester Tests

        [TestMethod]
        public async Task Should_Have_Error_When_Prerequisite_Is_In_Same_Semester_As_Course()
        {
            var coursePrerequisite = CoursePrerequisiteTestHelper.CreateCoursePrerequisite(courseId: 1, prereqId: 2, minGrade: 5);

            _mockCourseRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Course
            {
                Id = 1,
                Name = "Course A",
                CourseSemesters =
                [
                    new CourseSemester { Semester = new Semester { Id = 2, Number = 2 } }
                ]
            });
            _mockCourseRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new Course
            {
                Id = 2,
                Name = "Course B",
                CourseSemesters =
                [
                    new CourseSemester { Semester = new Semester { Id = 2, Number = 2 } }
                ]
            });

            _mockPrereqRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

            var result = await _validator.TestValidateAsync(coursePrerequisite);

            result.ShouldHaveValidationErrorFor(cp => cp)
                  .WithErrorMessage("Prerequisite course must be in an earlier semester than the course.");
        }

        [TestMethod]
        public async Task Should_Have_Error_When_Prerequisite_Is_In_Later_Semester_Than_Course()
        {
            var coursePrerequisite = CoursePrerequisiteTestHelper.CreateCoursePrerequisite(courseId: 1, prereqId: 2, minGrade: 5);

            _mockCourseRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Course
            {
                Id = 1,
                Name = "Course A",
                CourseSemesters =
                [
                    new CourseSemester { Semester = new Semester { Id = 2, Number = 2 } }
                ]
            });
            _mockCourseRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new Course
            {
                Id = 2,
                Name = "Course B",
                CourseSemesters =
                [
                    new CourseSemester { Semester = new Semester { Id = 3, Number = 3 } }
                ]
            });

            _mockPrereqRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

            var result = await _validator.TestValidateAsync(coursePrerequisite);

            result.ShouldHaveValidationErrorFor(cp => cp)
                  .WithErrorMessage("Prerequisite course must be in an earlier semester than the course.");
        }

        [TestMethod]
        public async Task Should_Not_Have_Error_When_Prerequisite_Is_In_Earlier_Semester_Than_Course()
        {
            var coursePrerequisite = CoursePrerequisiteTestHelper.CreateCoursePrerequisite(courseId: 1, prereqId: 2, minGrade: 5);

            _mockCourseRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Course
            {
                Id = 1,
                Name = "Course A",
                CourseSemesters =
                [
                    new CourseSemester { Semester = new Semester { Id = 2, Number = 2 } }
                ]
            });
            _mockCourseRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new Course
            {
                Id = 2,
                Name = "Course B",
                CourseSemesters =
                [
                    new CourseSemester { Semester = new Semester { Id = 1, Number = 1 } }
                ]
            });

            _mockPrereqRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

            var result = await _validator.TestValidateAsync(coursePrerequisite);

            result.ShouldNotHaveValidationErrorFor(cp => cp);
        }

        #endregion

        #region Circular Dependency Tests

        [TestMethod]
        public async Task Should_Have_Error_When_Adding_Prerequisite_Creates_Circular_Dependency()
        {
            var coursePrerequisite = CoursePrerequisiteTestHelper.CreateCoursePrerequisite(courseId: 2, prereqId: 1, minGrade: 5);

            _mockCourseRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new Course
            {
                Id = 2,
                Name = "Course B",
                CourseSemesters =
                [
                    new CourseSemester { Semester = new Semester { Id = 2, Number = 2 } }
                ]
            });
            _mockCourseRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Course
            {
                Id = 1,
                Name = "Course A",
                CourseSemesters =
                [
                    new CourseSemester { Semester = new Semester { Id = 2, Number = 2 } }
                ]
            });

            var existingPrereqs = new List<CoursePrerequisite>
            {
                CoursePrerequisiteTestHelper.CreateCoursePrerequisite(id:1,courseId: 1, prereqId: 2, minGrade: 5)
            };
            _mockPrereqRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(existingPrereqs);

            var result = await _validator.TestValidateAsync(coursePrerequisite);

            result.ShouldHaveValidationErrorFor(cp => cp)
                  .WithErrorMessage("Adding this prerequisite would create a circular dependency.");
        }

        [TestMethod]
        public async Task Should_Not_Have_Error_When_Adding_Prerequisite_Does_Not_Create_Circular_Dependency()
        {
            var coursePrerequisite = CoursePrerequisiteTestHelper.CreateCoursePrerequisite(courseId: 1, prereqId: 3, minGrade: 5);

            _mockCourseRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Course
            {
                Id = 1,
                Name = "Course A",
                CourseSemesters =
                [
                    new CourseSemester { Semester = new Semester { Id = 2, Number = 2 } }
                ]
            });
            _mockCourseRepo.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(new Course
            {
                Id = 3,
                Name = "Course C",
                CourseSemesters =
                [
                    new CourseSemester { Semester = new Semester { Id = 1, Number = 1 } }
                ]
            });

            var existingPrereqs = new List<CoursePrerequisite>
            {
               CoursePrerequisiteTestHelper.CreateCoursePrerequisite(id:1,courseId: 1, prereqId: 2, minGrade: 5)
            };
            _mockPrereqRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(existingPrereqs);

            var result = await _validator.TestValidateAsync(coursePrerequisite);

            result.ShouldNotHaveValidationErrorFor(cp => cp);
        }

        #endregion

        [TestMethod]
        public void Should_Access_Course_From_CoursePrerequisite()
        {
            var course = CourseTestHelper.CreateCourse(id: 101, name: "Advanced Programming", credits: 4);
            var coursePrerequisite = CoursePrerequisiteTestHelper.CreateCoursePrerequisite(courseId: 101, prereqId: 201);
            coursePrerequisite.Course = course;

            var prerequisiteCourse = coursePrerequisite.Course;

            Assert.AreEqual(101, prerequisiteCourse.Id);
            Assert.AreEqual("Advanced Programming", prerequisiteCourse.Name);
            Assert.AreEqual(4, prerequisiteCourse.Credits);
        }

        [TestMethod]
        public void Should_Access_Prerequisite_Course_From_CoursePrerequisite()
        {
            var prereqCourse = CourseTestHelper.CreateCourse(id: 201, name: "Basic Programming", credits: 3);
            var coursePrerequisite = CoursePrerequisiteTestHelper.CreateCoursePrerequisite(courseId: 101, prereqId: 201);
            coursePrerequisite.PrerequisiteCourse = prereqCourse;

            var prerequisiteCourse = coursePrerequisite.PrerequisiteCourse;

            Assert.AreEqual(201, prerequisiteCourse.Id);
            Assert.AreEqual("Basic Programming", prerequisiteCourse.Name);
            Assert.AreEqual(3, prerequisiteCourse.Credits);
        }

    }
}
