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
    public class SemesterServiceTests
    {
        private Mock<ISemesterRepository> _mockRepo;
        private Mock<IValidator<Semester>> _mockValidator;
        private ISemesterService _semesterService;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<ISemesterRepository>();
            _mockValidator = new Mock<IValidator<Semester>>();
            _semesterService = new SemesterService(_mockRepo.Object, _mockValidator.Object);
        }

        #region CreateSemesterAsync Tests

        [TestMethod]
        public async Task CreateSemesterAsync_Should_Throw_ValidationException_When_Invalid()
        {
            var semester = new Semester
            {
                Number = 0, 
                MinCredits = -5
            };

            var validationResult = new ValidationResult(new List<ValidationFailure>
            {
                new("Number", "Semester number must be a positive integer."),
                new("MinCredits", "Minimum credits must be non-negative.")
            });

            _mockValidator
                .Setup(v => v.ValidateAsync(semester, It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(validationResult);

            await Assert.ThrowsExceptionAsync<ValidationException>(() => _semesterService.CreateSemesterAsync(semester));
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<Semester>()), Times.Never);
        }

        [TestMethod]
        public async Task CreateSemesterAsync_Should_Add_Semester_When_Valid()
        {
            var semester = new Semester
            {
                Number = 1,
                MinCredits = 20
            };

            var validationResult = new ValidationResult(); 
            _mockValidator
                .Setup(v => v.ValidateAsync(semester, It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(validationResult);

            _mockRepo
                .Setup(r => r.AddAsync(semester))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var result = await _semesterService.CreateSemesterAsync(semester);

            _mockRepo.Verify(r => r.AddAsync(semester), Times.Once);
            Assert.AreEqual(semester, result);
        }

        #endregion

        #region GetSemesterByIdAsync Tests

        [TestMethod]
        public async Task GetSemesterByIdAsync_Should_Return_Semester_When_Found()
        {
            var semesterId = 1;
            var semester = new Semester
            {
                Id = semesterId,
                Number = 2,
                MinCredits = 18
            };

            _mockRepo
                .Setup(r => r.GetByIdAsync(semesterId))
                .ReturnsAsync(semester);

            var result = await _semesterService.GetSemesterByIdAsync(semesterId);

            Assert.IsNotNull(result);
            Assert.AreEqual(semesterId, result.Id);
            Assert.AreEqual(2, result.Number);
            Assert.AreEqual(18, result.MinCredits);
        }

        [TestMethod]
        public async Task GetSemesterByIdAsync_Should_Return_Null_When_Not_Found()
        {
            var semesterId = 99;
            _mockRepo
                .Setup(r => r.GetByIdAsync(semesterId))
                .ReturnsAsync((Semester)null);

            var result = await _semesterService.GetSemesterByIdAsync(semesterId);

            Assert.IsNull(result);
        }

        #endregion

        #region GetAllSemestersAsync Tests

        [TestMethod]
        public async Task GetAllSemestersAsync_Should_Return_List_Of_Semesters()
        {
            var semesters = new List<Semester>
            {
                new() { Id = 1, Number = 1, MinCredits = 20 },
                new() { Id = 2, Number = 2, MinCredits = 18 }
            };

            _mockRepo
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(semesters);

            var result = await _semesterService.GetAllSemestersAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            CollectionAssert.Contains(result, semesters[0]);
            CollectionAssert.Contains(result, semesters[1]);
        }

        [TestMethod]
        public async Task GetAllSemestersAsync_Should_Return_Empty_List_When_No_Semesters()
        {
            var semesters = new List<Semester>();

            _mockRepo
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(semesters);

            var result = await _semesterService.GetAllSemestersAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        #endregion

        #region UpdateSemesterAsync Tests

        [TestMethod]
        public async Task UpdateSemesterAsync_Should_Throw_ValidationException_When_Invalid()
        {
            var semester = new Semester
            {
                Id = 1,
                Number = -1, 
                MinCredits = -10 
            };

            var validationResult = new ValidationResult(new List<ValidationFailure>
            {
                new("Number", "Semester number must be a positive integer."),
                new("MinCredits", "Minimum credits must be non-negative.")
            });

            _mockValidator
                .Setup(v => v.ValidateAsync(semester, It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(validationResult);

            await Assert.ThrowsExceptionAsync<ValidationException>(() => _semesterService.UpdateSemesterAsync(semester));
            _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Semester>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateSemesterAsync_Should_Update_Semester_When_Valid()
        {
            var semester = new Semester
            {
                Id = 1,
                Number = 3,
                MinCredits = 22
            };

            var validationResult = new ValidationResult(); 
            _mockValidator
                .Setup(v => v.ValidateAsync(semester, It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(validationResult);

            _mockRepo
                .Setup(r => r.UpdateAsync(semester))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _semesterService.UpdateSemesterAsync(semester);

            _mockRepo.Verify(r => r.UpdateAsync(semester), Times.Once);
        }

        #endregion

        #region DeleteSemesterAsync Tests

        [TestMethod]
        public async Task DeleteSemesterAsync_Should_Call_Delete_When_Semester_Exists()
        {
            var semesterId = 1;
            _mockRepo
                .Setup(r => r.DeleteAsync(semesterId))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _semesterService.DeleteSemesterAsync(semesterId);

            _mockRepo.Verify(r => r.DeleteAsync(semesterId), Times.Once);
        }

        [TestMethod]
        public async Task DeleteSemesterAsync_Should_Call_Delete_When_Semester_Does_Not_Exist()
        {
            var semesterId = 99; 
            _mockRepo
                .Setup(r => r.DeleteAsync(semesterId))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _semesterService.DeleteSemesterAsync(semesterId);

            _mockRepo.Verify(r => r.DeleteAsync(semesterId), Times.Once);
        }

        #endregion

     
    }
}
