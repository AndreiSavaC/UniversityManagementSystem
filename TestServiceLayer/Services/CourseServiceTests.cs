namespace TestServiceLayer.Services
{
    using Moq;
    using FluentValidation;
    using FluentValidation.Results;
    using DomainModel.Entities;
    using DataAccessLayer.Repositories.Interfaces;
    using ServiceLayer.Services.Interfaces;
    using ServiceLayer.Services.Implementations;
    [TestClass]
    public class CourseServiceTests
    {
        private Mock<ICourseRepository> _mockRepo;
        private Mock<IValidator<Course>> _mockValidator;
        private ICourseService _courseService;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<ICourseRepository>();
            _mockValidator = new Mock<IValidator<Course>>();
            _courseService = new CourseService(_mockRepo.Object, _mockValidator.Object);
        }

        #region CreateCourseAsync Tests

        [TestMethod]
        public async Task CreateCourseAsync_Should_Throw_ValidationException_When_Invalid()
        {
            var course = new Course
            {
                Name = "InvalidCost",
                Credits = 3,
                Cost = 200m, 
                MinCostPerCredit = 80m,
                MaxCostPerCredit = 120m,
                Description = "A valid description."
            };

            var validationResult = new ValidationResult(new List<ValidationFailure>
            {
                new("Cost", "Cost must be in range [MinCostPerCredit * Credits, MaxCostPerCredit * Credits].")
            });

            _mockValidator
                .Setup(v => v.ValidateAsync(course, It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(validationResult);

            await Assert.ThrowsExceptionAsync<ValidationException>(() => _courseService.CreateCourseAsync(course));
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<Course>()), Times.Never);
        }

        [TestMethod]
        public async Task CreateCourseAsync_Should_Add_Course_When_Valid()
        {
            var course = new Course
            {
                Name = "Mathematics",
                Description = "Advanced math course",
                Credits = 3,
                Cost = 300m, 
                MinCostPerCredit = 80m,
                MaxCostPerCredit = 120m,
            };

            var validationResult = new ValidationResult();
            _mockValidator
                .Setup(v => v.ValidateAsync(course, It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(validationResult);

            _mockRepo
                .Setup(r => r.AddAsync(course))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var result = await _courseService.CreateCourseAsync(course);

            _mockRepo.Verify(r => r.AddAsync(course), Times.Once);
            Assert.AreEqual(course, result);
        }

        #endregion

        #region GetCourseByIdAsync Tests

        [TestMethod]
        public async Task GetCourseByIdAsync_Should_Return_Course_When_Found()
        {
            var courseId = 1;
            var course = new Course
            {
                Id = courseId,
                Name = "Physics",
                Description = "Physics course",
                Credits = 4,
                Cost = 400m,
                MinCostPerCredit = 80m,
                MaxCostPerCredit = 120m,
            };

            _mockRepo
                .Setup(r => r.GetByIdAsync(courseId))
                .ReturnsAsync(course);

            var result = await _courseService.GetCourseByIdAsync(courseId);

            Assert.IsNotNull(result);
            Assert.AreEqual(courseId, result.Id);
            Assert.AreEqual("Physics", result.Name);
        }

        [TestMethod]
        public async Task GetCourseByIdAsync_Should_Return_Null_When_Not_Found()
        {
            var courseId = 99; 
            _mockRepo
                .Setup(r => r.GetByIdAsync(courseId))
                .ReturnsAsync((Course?)null);

            var result = await _courseService.GetCourseByIdAsync(courseId);

            Assert.IsNull(result);
        }

        #endregion

        #region GetAllCoursesAsync Tests

        [TestMethod]
        public async Task GetAllCoursesAsync_Should_Return_List_Of_Courses()
        {
            var courses = new List<Course>
            {
                new() {
                    Id = 1,
                    Name = "Chemistry",
                    Description = "Chemistry course",
                    Credits = 3,
                    Cost = 300m,
                    MinCostPerCredit = 80m,
                    MaxCostPerCredit = 120m,
                },
                new()
                {
                    Id = 2,
                    Name = "Biology",
                    Description = "Biology course",
                    Credits = 4,
                    Cost = 400m,
                    MinCostPerCredit = 80m,
                    MaxCostPerCredit = 120m,
                }
            };

            _mockRepo
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(courses);

            var result = await _courseService.GetAllCoursesAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            CollectionAssert.Contains(result, courses[0]);
            CollectionAssert.Contains(result, courses[1]);
        }

        [TestMethod]
        public async Task GetAllCoursesAsync_Should_Return_Empty_List_When_No_Courses()
        {
            var courses = new List<Course>();

            _mockRepo
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(courses);

            var result = await _courseService.GetAllCoursesAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        #endregion

        #region UpdateCourseAsync Tests

        [TestMethod]
        public async Task UpdateCourseAsync_Should_Throw_ValidationException_When_Invalid()
        {
            var course = new Course
            {
                Id = 1,
                Name = "InvalidUpdate",
                Credits = 3,
                Cost = 200m, 
                MinCostPerCredit = 80m,
                MaxCostPerCredit = 120m,
                Description = "Updated description."
            };

            var validationResult = new ValidationResult(new List<ValidationFailure>
            {
                new("Cost", "Cost must be in range [MinCostPerCredit * Credits, MaxCostPerCredit * Credits].")
            });

            _mockValidator
                .Setup(v => v.ValidateAsync(course, It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(validationResult);

            await Assert.ThrowsExceptionAsync<ValidationException>(() => _courseService.UpdateCourseAsync(course));
            _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Course>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateCourseAsync_Should_Update_Course_When_Valid()
        {
            var course = new Course
            {
                Id = 1,
                Name = "Advanced Mathematics",
                Description = "Advanced math course description.",
                Credits = 3,
                Cost = 300m,
                MinCostPerCredit = 80m,
                MaxCostPerCredit = 120m,
            };

            var validationResult = new ValidationResult(); 
            _mockValidator
                .Setup(v => v.ValidateAsync(course, It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(validationResult);

            _mockRepo
                .Setup(r => r.UpdateAsync(course))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _courseService.UpdateCourseAsync(course);

            _mockRepo.Verify(r => r.UpdateAsync(course), Times.Once);
        }



        [TestMethod]
        public async Task DeleteCourseAsync_Should_Call_Delete_When_Course_Exists()
        {
            var courseId = 1;
            _mockRepo
                .Setup(r => r.DeleteAsync(courseId))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _courseService.DeleteCourseAsync(courseId);

            _mockRepo.Verify(r => r.DeleteAsync(courseId), Times.Once);
        }

        [TestMethod]
        public async Task DeleteCourseAsync_Should_Call_Delete_When_Course_Does_Not_Exist()
        {
            var courseId = 99; 
            _mockRepo
                .Setup(r => r.DeleteAsync(courseId))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _courseService.DeleteCourseAsync(courseId);

            _mockRepo.Verify(r => r.DeleteAsync(courseId), Times.Once);
        }

        #endregion


    }
}
