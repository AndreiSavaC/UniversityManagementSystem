namespace TestServiceLayer.Services
{
    using Moq;
    using FluentValidation;
    using FluentValidation.Results;
    using DomainModel.Entities;
    using ServiceLayer.Services.Interfaces;
    using ServiceLayer.Services.Implementations;
    using DataAccessLayer.Repositories.Interfaces;
    [TestClass]
    public class CoursePrerequisiteServiceTests
    {
        private Mock<ICoursePrerequisiteRepository> _mockPrereqRepo;
        private Mock<ICourseRepository> _mockCourseRepo;
        private Mock<ICourseSemesterRepository> _mockCourseSemesterRepo;
        private Mock<IValidator<CoursePrerequisite>> _mockValidator;
        private ICoursePrerequisiteService _coursePrerequisiteService;

        [TestInitialize]
        public void Setup()
        {
            _mockPrereqRepo = new Mock<ICoursePrerequisiteRepository>();
            _mockCourseRepo = new Mock<ICourseRepository>();
            _mockCourseSemesterRepo = new Mock<ICourseSemesterRepository>();
            _mockValidator = new Mock<IValidator<CoursePrerequisite>>();

            _coursePrerequisiteService = new CoursePrerequisiteService(
                _mockPrereqRepo.Object,
                _mockCourseRepo.Object,
                _mockCourseSemesterRepo.Object,
                _mockValidator.Object
            );
        }

        #region GetCoursePrerequisiteByIdAsync Tests

        [TestMethod]
        public async Task GetCoursePrerequisiteByIdAsync_Should_Return_CoursePrerequisite_When_Found()
        {
            var cpId = 1;
            var cp = new CoursePrerequisite { Id = cpId, CourseId = 1, PrereqId = 2, MinGrade = 5 };
            _mockPrereqRepo.Setup(r => r.GetByIdAsync(cpId)).ReturnsAsync(cp);

            var result = await _coursePrerequisiteService.GetCoursePrerequisiteByIdAsync(cpId);

            Assert.IsNotNull(result);
            Assert.AreEqual(cpId, result.Id);
            Assert.AreEqual(1, result.CourseId);
            Assert.AreEqual(2, result.PrereqId);
            Assert.AreEqual(5, result.MinGrade);
        }

        [TestMethod]
        public async Task GetCoursePrerequisiteByIdAsync_Should_Return_Null_When_Not_Found()
        {
            var cpId = 99;

            _mockPrereqRepo.Setup(r => r.GetByIdAsync(cpId)).ReturnsAsync((CoursePrerequisite?)null);

            var result = await _coursePrerequisiteService.GetCoursePrerequisiteByIdAsync(cpId);

            Assert.IsNull(result);
        }

        #endregion

        #region GetAllCoursePrerequisitesAsync Tests

        [TestMethod]
        public async Task GetAllCoursePrerequisitesAsync_Should_Return_List_Of_CoursePrerequisites()
        {
            var cps = new List<CoursePrerequisite>
            {
                new() { Id = 1, CourseId = 1, PrereqId = 2, MinGrade = 5 },
                 new() { Id = 2, CourseId = 2, PrereqId = 3, MinGrade = 6 }
            };
            _mockPrereqRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(cps);

            var result = await _coursePrerequisiteService.GetAllCoursePrerequisitesAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            CollectionAssert.Contains(result, cps[0]);
            CollectionAssert.Contains(result, cps[1]);
        }

        [TestMethod]
        public async Task GetAllCoursePrerequisitesAsync_Should_Return_Empty_List_When_No_CoursePrerequisites()
        {
            var cps = new List<CoursePrerequisite>();
            _mockPrereqRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(cps);

            var result = await _coursePrerequisiteService.GetAllCoursePrerequisitesAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        #endregion

        #region CreateCoursePrerequisiteAsync Tests

        [TestMethod]
        public async Task CreateCoursePrerequisiteAsync_Should_Throw_ValidationException_When_Invalid()
        {
            var cp = new CoursePrerequisite { CourseId = 1, PrereqId = 2, MinGrade = 11 }; 
            var validationResult = new ValidationResult(new List<ValidationFailure>
            {
                new("MinGrade", "Minimum grade must be between 1 and 10.")
            });

            _mockValidator.Setup(v => v.ValidateAsync(cp, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(validationResult);

            await Assert.ThrowsExceptionAsync<ValidationException>(() => _coursePrerequisiteService.CreateCoursePrerequisiteAsync(cp));

            _mockPrereqRepo.Verify(r => r.AddAsync(It.IsAny<CoursePrerequisite>()), Times.Never);
        }

        [TestMethod]
        public async Task CreateCoursePrerequisiteAsync_Should_Add_CoursePrerequisite_When_Valid()
        {
            var cp = new CoursePrerequisite { CourseId = 1, PrereqId = 2, MinGrade = 5 };
            var validationResult = new ValidationResult(); 

            _mockValidator.Setup(v => v.ValidateAsync(cp, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(validationResult);

            _mockPrereqRepo.Setup(r => r.AddAsync(cp))
                           .Returns(Task.CompletedTask)
                           .Verifiable();
            
            var result = await _coursePrerequisiteService.CreateCoursePrerequisiteAsync(cp);

            _mockValidator.Verify(v => v.ValidateAsync(cp, It.IsAny<CancellationToken>()), Times.Once);
            _mockPrereqRepo.Verify(r => r.AddAsync(cp), Times.Once);
            Assert.AreEqual(cp, result);
        }

        #endregion

        #region UpdateCoursePrerequisiteAsync Tests

        [TestMethod]
        public async Task UpdateCoursePrerequisiteAsync_Should_Throw_ValidationException_When_Invalid()
        {
            var cp = new CoursePrerequisite { Id = 1, CourseId = 1, PrereqId = 1, MinGrade = 5 }; 
            var validationResult = new ValidationResult(new List<ValidationFailure>
            {
                new("", "Course cannot be a prerequisite of itself.")
            });

            _mockValidator.Setup(v => v.ValidateAsync(cp, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(validationResult);

            await Assert.ThrowsExceptionAsync<ValidationException>(() => _coursePrerequisiteService.UpdateCoursePrerequisiteAsync(cp));

            _mockPrereqRepo.Verify(r => r.UpdateAsync(It.IsAny<CoursePrerequisite>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateCoursePrerequisiteAsync_Should_Update_CoursePrerequisite_When_Valid()
        {
            var cp = new CoursePrerequisite { Id = 1, CourseId = 1, PrereqId = 2, MinGrade = 5 };
            var validationResult = new ValidationResult();

            _mockValidator.Setup(v => v.ValidateAsync(cp, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(validationResult);

            _mockPrereqRepo.Setup(r => r.UpdateAsync(cp))
                           .Returns(Task.CompletedTask)
                           .Verifiable();

            await _coursePrerequisiteService.UpdateCoursePrerequisiteAsync(cp);

            _mockValidator.Verify(v => v.ValidateAsync(cp, It.IsAny<CancellationToken>()), Times.Once);
            _mockPrereqRepo.Verify(r => r.UpdateAsync(cp), Times.Once);
        }

        #endregion

        #region DeleteCoursePrerequisiteAsync Tests

        [TestMethod]
        public async Task DeleteCoursePrerequisiteAsync_Should_Call_Delete_When_CoursePrerequisite_Exists()
        {
            var cpId = 1;
            _mockPrereqRepo.Setup(r => r.DeleteAsync(cpId))
                           .Returns(Task.CompletedTask)
                           .Verifiable();

            await _coursePrerequisiteService.DeleteCoursePrerequisiteAsync(cpId);

            _mockPrereqRepo.Verify(r => r.DeleteAsync(cpId), Times.Once);
        }

        [TestMethod]
        public async Task DeleteCoursePrerequisiteAsync_Should_Call_Delete_When_CoursePrerequisite_Does_Not_Exist()
        {
            var cpId = 99; 
            _mockPrereqRepo.Setup(r => r.DeleteAsync(cpId))
                           .Returns(Task.CompletedTask)
                           .Verifiable();

            await _coursePrerequisiteService.DeleteCoursePrerequisiteAsync(cpId);

            _mockPrereqRepo.Verify(r => r.DeleteAsync(cpId), Times.Once);
        }

        #endregion
    }
}
