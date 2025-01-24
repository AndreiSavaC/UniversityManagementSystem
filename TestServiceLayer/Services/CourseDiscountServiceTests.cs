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
    public class CourseDiscountServiceTests
    {
        private Mock<ICourseRepository> _mockCourseRepo;
        private Mock<ICourseDiscountRepository> _mockDiscountRepo;
        private Mock<IValidator<CourseDiscount>> _mockValidator;
        private ICourseDiscountService _courseDiscountService;

        [TestInitialize]
        public void Setup()
        {
            _mockCourseRepo = new Mock<ICourseRepository>();
            _mockDiscountRepo = new Mock<ICourseDiscountRepository>();
            _mockValidator = new Mock<IValidator<CourseDiscount>>();

            _courseDiscountService = new CourseDiscountService(
                _mockCourseRepo.Object,
                _mockDiscountRepo.Object,
                _mockValidator.Object
            );
        }

        #region CRUD Operations Tests

        [TestMethod]
        public async Task GetCourseDiscountByIdAsync_Should_Return_CourseDiscount_When_Found()
        {
            var discountId = 1;
            var discount = new CourseDiscount { Id = discountId, GroupId = 1, CourseId = 1, DiscountPercentage = 10 };
            _mockDiscountRepo.Setup(r => r.GetByIdAsync(discountId)).ReturnsAsync(discount);

            var result = await _courseDiscountService.GetCourseDiscountByIdAsync(discountId);

            Assert.IsNotNull(result);
            Assert.AreEqual(discountId, result.Id);
            Assert.AreEqual(1, result.GroupId);
            Assert.AreEqual(1, result.CourseId);
            Assert.AreEqual(10, result.DiscountPercentage);
        }

        [TestMethod]
        public async Task GetCourseDiscountByIdAsync_Should_Return_Null_When_Not_Found()
        {
            var discountId = 99;
            _mockDiscountRepo.Setup(r => r.GetByIdAsync(discountId)).ReturnsAsync((CourseDiscount?)null);

            var result = await _courseDiscountService.GetCourseDiscountByIdAsync(discountId);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetAllCourseDiscountsAsync_Should_Return_List_Of_CourseDiscounts()
        {
            var discounts = new List<CourseDiscount>
            {
                new() { Id = 1, GroupId = 1, CourseId = 1, DiscountPercentage = 10 },
                new() { Id = 2, GroupId = 2, CourseId = 2, DiscountPercentage = 20 }
            };
            _mockDiscountRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(discounts);

            var result = await _courseDiscountService.GetAllCourseDiscountsAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            CollectionAssert.Contains(result, discounts[0]);
            CollectionAssert.Contains(result, discounts[1]);
        }

        [TestMethod]
        public async Task CreateCourseDiscountAsync_Should_Throw_ValidationException_When_Invalid()
        {
            var discount = new CourseDiscount { GroupId = 1, CourseId = 1, DiscountPercentage = 110 };
            var validationResult = new ValidationResult(new List<ValidationFailure>
            {
                new("DiscountPercentage", "Discount percentage must be between 0 and 100.")
            });

            _mockValidator.Setup(v => v.ValidateAsync(discount, It.IsAny<System.Threading.CancellationToken>()))
                          .ReturnsAsync(validationResult);

            await Assert.ThrowsExceptionAsync<ValidationException>(() => _courseDiscountService.CreateCourseDiscountAsync(discount));
            _mockDiscountRepo.Verify(r => r.AddAsync(It.IsAny<CourseDiscount>()), Times.Never);
        }

        [TestMethod]
        public async Task CreateCourseDiscountAsync_Should_Add_CourseDiscount_When_Valid()
        {
            var discount = new CourseDiscount { GroupId = 2, CourseId = 2, DiscountPercentage = 15 };
            var validationResult = new ValidationResult();

            _mockValidator.Setup(v => v.ValidateAsync(discount, It.IsAny<System.Threading.CancellationToken>()))
                          .ReturnsAsync(validationResult);

            _mockDiscountRepo.Setup(r => r.AddAsync(discount))
                             .Returns(Task.CompletedTask)
                             .Verifiable();

            var result = await _courseDiscountService.CreateCourseDiscountAsync(discount);

            _mockValidator.Verify(v => v.ValidateAsync(discount, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
            _mockDiscountRepo.Verify(r => r.AddAsync(discount), Times.Once);
            Assert.AreEqual(discount, result);
        }

        [TestMethod]
        public async Task UpdateCourseDiscountAsync_Should_Throw_ValidationException_When_Invalid()
        {
            var discount = new CourseDiscount { Id = 1, GroupId = 1, CourseId = 1, DiscountPercentage = -5 };
            var validationResult = new ValidationResult(new List<ValidationFailure>
            {
                new("DiscountPercentage", "Discount percentage must be between 0 and 100.")
            });

            _mockValidator.Setup(v => v.ValidateAsync(discount, It.IsAny<System.Threading.CancellationToken>()))
                          .ReturnsAsync(validationResult);

            await Assert.ThrowsExceptionAsync<ValidationException>(() => _courseDiscountService.UpdateCourseDiscountAsync(discount));
            _mockDiscountRepo.Verify(r => r.UpdateAsync(It.IsAny<CourseDiscount>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateCourseDiscountAsync_Should_Update_CourseDiscount_When_Valid()
        {
            var discount = new CourseDiscount { Id = 1, GroupId = 1, CourseId = 1, DiscountPercentage = 25 };
            var validationResult = new ValidationResult(); 

            _mockValidator.Setup(v => v.ValidateAsync(discount, It.IsAny<System.Threading.CancellationToken>()))
                          .ReturnsAsync(validationResult);

            _mockDiscountRepo.Setup(r => r.UpdateAsync(discount))
                             .Returns(Task.CompletedTask)
                             .Verifiable();

            await _courseDiscountService.UpdateCourseDiscountAsync(discount);

            _mockValidator.Verify(v => v.ValidateAsync(discount, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
            _mockDiscountRepo.Verify(r => r.UpdateAsync(discount), Times.Once);
        }

        [TestMethod]
        public async Task DeleteCourseDiscountAsync_Should_Call_Delete_When_CourseDiscount_Exists()
        {
            var discountId = 1;
            _mockDiscountRepo.Setup(r => r.DeleteAsync(discountId))
                             .Returns(Task.CompletedTask)
                             .Verifiable();

            await _courseDiscountService.DeleteCourseDiscountAsync(discountId);

            _mockDiscountRepo.Verify(r => r.DeleteAsync(discountId), Times.Once);
        }

        [TestMethod]
        public async Task DeleteCourseDiscountAsync_Should_Call_Delete_When_CourseDiscount_Does_Not_Exist()
        {
            var discountId = 99;
            _mockDiscountRepo.Setup(r => r.DeleteAsync(discountId))
                             .Returns(Task.CompletedTask)
                             .Verifiable();

            await _courseDiscountService.DeleteCourseDiscountAsync(discountId);

            _mockDiscountRepo.Verify(r => r.DeleteAsync(discountId), Times.Once);
        }

        #endregion

        #region ApplyDiscountsAsync Tests

        [TestMethod]
        public async Task ApplyDiscountsAsync_Should_Apply_Max_Discount_When_Multiple_Discounts_Are_Available()
        {
            var student = new Student
            {
                Id = 1,
                LastName = "John Doe",
                Enrollments =
        [
            new() { CourseId = 1 },
            new() { CourseId = 2 },
            new() { CourseId = 3 }
        ]
            };

            var discounts = new List<CourseDiscount>
    {
        new() { Id = 1, GroupId = 1, CourseId = 1, DiscountPercentage = 10 },
        new() { Id = 2, GroupId = 2, CourseId = 2, DiscountPercentage = 20 },
        new() { Id = 3, GroupId = 3, CourseId = 3, DiscountPercentage = 15 }
    };

            _mockDiscountRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(discounts);
            _mockDiscountRepo.Setup(r => r.GetCourseIdsByGroupIdAsync(1)).ReturnsAsync(new List<int> { 1 });
            _mockDiscountRepo.Setup(r => r.GetCourseIdsByGroupIdAsync(2)).ReturnsAsync(new List<int> { 2 });
            _mockDiscountRepo.Setup(r => r.GetCourseIdsByGroupIdAsync(3)).ReturnsAsync(new List<int> { 3 });

            var courses = new List<Course>
    {
        new() { Id = 1, Cost = 100m },
        new() { Id = 2, Cost = 200m },
        new() { Id = 3, Cost = 300m }
    };

            _mockCourseRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(courses[0]);
            _mockCourseRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(courses[1]);
            _mockCourseRepo.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(courses[2]);

            _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<CourseDiscount>(), It.IsAny<System.Threading.CancellationToken>()))
                          .ReturnsAsync(new ValidationResult());

            await _courseDiscountService.ApplyDiscountsAsync(student);

            _mockCourseRepo.Verify(r => r.UpdateAsync(It.Is<Course>(c => c.Id == 1 && c.Cost == 90m)), Times.Once);
            _mockCourseRepo.Verify(r => r.UpdateAsync(It.Is<Course>(c => c.Id == 2 && c.Cost == 160m)), Times.Once);
            _mockCourseRepo.Verify(r => r.UpdateAsync(It.Is<Course>(c => c.Id == 3 && c.Cost == 255m)), Times.Once);

            _mockValidator.Verify(v => v.ValidateAsync(It.IsAny<CourseDiscount>(), It.IsAny<System.Threading.CancellationToken>()), Times.Exactly(3));
        }

        [TestMethod]
        public async Task ApplyDiscountsAsync_Should_Not_Apply_Discount_When_Validation_Fails()
        {
            var student = new Student
            {
                Id = 1,
                LastName = "Jane Doe",
                Enrollments = new List<Enrollment>
        {
            new() { CourseId = 1 },
            new() { CourseId = 2 }
        }
            };

            var discounts = new List<CourseDiscount>
    {
        new() { Id = 1, GroupId = 1, CourseId = 1, DiscountPercentage = 10 },
        new() { Id = 2, GroupId = 2, CourseId = 2, DiscountPercentage = 20 }
    };

            var courses = new List<Course>
    {
        new() { Id = 1, Cost = 100m },
        new() { Id = 2, Cost = 200m }
    };

            _mockDiscountRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(discounts);
            _mockDiscountRepo.Setup(r => r.GetCourseIdsByGroupIdAsync(1)).ReturnsAsync(new List<int> { 1 });
            _mockDiscountRepo.Setup(r => r.GetCourseIdsByGroupIdAsync(2)).ReturnsAsync(new List<int> { 2 });

            _mockCourseRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(courses[0]);
            _mockCourseRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(courses[1]);

            _mockValidator.Setup(v => v.ValidateAsync(It.Is<CourseDiscount>(cd => cd.GroupId == 1), It.IsAny<System.Threading.CancellationToken>()))
                          .ReturnsAsync(new ValidationResult(new List<ValidationFailure>
                          {
                      new("GroupId", "Group ID must be unique.") 
                          }));

            _mockValidator.Setup(v => v.ValidateAsync(It.Is<CourseDiscount>(cd => cd.GroupId == 2), It.IsAny<System.Threading.CancellationToken>()))
                          .ReturnsAsync(new ValidationResult()); 

            await _courseDiscountService.ApplyDiscountsAsync(student);

            _mockCourseRepo.Verify(r => r.UpdateAsync(It.Is<Course>(c => c.Id == 1)), Times.Never);

            _mockCourseRepo.Verify(r => r.UpdateAsync(It.Is<Course>(c => c.Id == 2 && c.Cost == 160m)), Times.Once);

            _mockValidator.Verify(v => v.ValidateAsync(It.IsAny<CourseDiscount>(), It.IsAny<System.Threading.CancellationToken>()), Times.Exactly(2));
        }


        [TestMethod]
        public async Task ApplyDiscountsAsync_Should_Handle_No_Discounts_Available()
        {
            var student = new Student
            {
                Id = 1,
                LastName = "Alice",
                Enrollments = new List<Enrollment>
        {
            new() { CourseId = 1 },
            new() { CourseId = 2 }
        }
            };

            _mockDiscountRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

            await _courseDiscountService.ApplyDiscountsAsync(student);

            _mockCourseRepo.Verify(r => r.UpdateAsync(It.IsAny<Course>()), Times.Never);
        }

        [TestMethod]
        public async Task ApplyDiscountsAsync_Should_Not_Apply_Discount_When_Not_All_Courses_In_Group_Are_Selected()
        {
            var student = new Student
            {
                Id = 1,
                LastName = "Bob",
                Enrollments = new List<Enrollment>
        {
            new() { CourseId = 1 },
            new() { CourseId = 2 }
        }
            };

            var discounts = new List<CourseDiscount>
    {
        new() { Id = 1, GroupId = 1, CourseId = 1, DiscountPercentage = 10 }
    };

            _mockDiscountRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(discounts);
            _mockDiscountRepo.Setup(r => r.GetCourseIdsByGroupIdAsync(1)).ReturnsAsync(new List<int> { 1, 2, 3 });

            var courses = new List<Course>
    {
        new() { Id = 1, Cost = 100m },
        new() { Id = 2, Cost = 200m },
        new() { Id = 3, Cost = 300m }
    };

            _mockCourseRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(courses[0]);
            _mockCourseRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(courses[1]);
            _mockCourseRepo.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(courses[2]);

            _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<CourseDiscount>(), It.IsAny<System.Threading.CancellationToken>()))
                          .ReturnsAsync(new ValidationResult());

            await _courseDiscountService.ApplyDiscountsAsync(student);

            _mockCourseRepo.Verify(r => r.UpdateAsync(It.IsAny<Course>()), Times.Never);
        }

        #endregion

    }
}
