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
    public class StudentServiceTests
    {
        private Mock<IStudentRepository> _mockRepo;
        private Mock<IValidator<Student>> _mockValidator;
        private Mock<IEnrollmentRepository> _mockEnrollmentRepo;
        private Mock<IExamRepository> _mockExamRepo;
        private Mock<ISemesterRepository> _mockSemesterRepo;
        private IStudentService _studentService;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<IStudentRepository>();
            _mockValidator = new Mock<IValidator<Student>>();
            _mockEnrollmentRepo = new Mock<IEnrollmentRepository>();
            _mockExamRepo = new Mock<IExamRepository>();
            _mockSemesterRepo = new Mock<ISemesterRepository>();

            _studentService = new StudentService(
                _mockRepo.Object,
                _mockEnrollmentRepo.Object,
                _mockExamRepo.Object,
                _mockSemesterRepo.Object,
                _mockValidator.Object
            );
        }

        #region CreateStudentAsync Tests

        [TestMethod]
        public async Task CreateStudentAsync_Should_Throw_ValidationException_When_Invalid()
        {
            var student = new Student
            {
                FirstName = "John",
                LastName = "Doe",
                CNP = "123456789012",
                Address = "123 Main Street",
                UnivCode = "UNIV123",
                Emails = ["john.doe@example.com"],
                PhoneNumbers = [ "0123 456789" ]
            };

            var validationResult = new ValidationResult(new List<ValidationFailure>
            {
                new("CNP", "CNP must be exactly 13 characters.")
            });

            _mockValidator
                .Setup(v => v.ValidateAsync(student, It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(validationResult);

            await Assert.ThrowsExceptionAsync<ValidationException>(() => _studentService.CreateStudentAsync(student));
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<Student>()), Times.Never);
        }

        [TestMethod]
        public async Task CreateStudentAsync_Should_Add_Student_When_Valid()
        {
            var student = new Student
            {
                FirstName = "Jane",
                LastName = "Smith",
                CNP = "1234567890123",
                Address = "456 Elm Street",
                UnivCode = "UNIV456",
                Emails = [  "jane.smith@example.com" ],
                PhoneNumbers = [ "0456 123456" ]
            };

            var validationResult = new ValidationResult();
            _mockValidator
                .Setup(v => v.ValidateAsync(student, It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(validationResult);

            _mockRepo
                .Setup(r => r.AddAsync(student))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var result = await _studentService.CreateStudentAsync(student);

            _mockRepo.Verify(r => r.AddAsync(student), Times.Once);
            Assert.AreEqual(student, result);
        }

        #endregion

        #region GetStudentByIdAsync Tests

        [TestMethod]
        public async Task GetStudentByIdAsync_Should_Return_Student_When_Found()
        {
            var studentId = 1;
            var student = new Student
            {
                Id = studentId,
                FirstName = "Alice",
                LastName = "Johnson",
                CNP = "1234567890123",
                Address = "789 Oak Avenue",
                UnivCode = "UNIV789",
                Emails = [ "alice.johnson@example.com" ],
                PhoneNumbers = [ "0678 901234" ]
            };

            _mockRepo
                .Setup(r => r.GetByIdAsync(studentId))
                .ReturnsAsync(student);

            var result = await _studentService.GetStudentByIdAsync(studentId);

            Assert.IsNotNull(result);
            Assert.AreEqual(studentId, result.Id);
            Assert.AreEqual("Alice", result.FirstName);
            Assert.AreEqual("Johnson", result.LastName);
        }

        [TestMethod]
        public async Task GetStudentByIdAsync_Should_Return_Null_When_Not_Found()
        {
            var studentId = 99; 
            _mockRepo
                .Setup(r => r.GetByIdAsync(studentId))
                .ReturnsAsync((Student)null);

            var result = await _studentService.GetStudentByIdAsync(studentId);

            Assert.IsNull(result);
        }

        #endregion

        #region GetAllStudentsAsync Tests

        [TestMethod]
        public async Task GetAllStudentsAsync_Should_Return_List_Of_Students()
        {
            var students = new List<Student>
            {
                new() {
                    Id = 1,
                    FirstName = "Bob",
                    LastName = "Williams",
                    CNP = "1234567890123",
                    Address = "321 Pine Road",
                    UnivCode = "UNIV321",
                    Emails = [ "bob.williams@example.com" ],
                    PhoneNumbers = [ "0789 012345" ]
                },
                new()
                {
                    Id = 2,
                    FirstName = "Carol",
                    LastName = "Brown",
                    CNP = "9876543210987",
                    Address = "654 Maple Street",
                    UnivCode = "UNIV654",
                    Emails = [ "carol.brown@example.com" ],
                    PhoneNumbers = [ "0890 123456" ]
                }
            };

            _mockRepo
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(students);

            var result = await _studentService.GetAllStudentsAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            CollectionAssert.Contains(result, students[0]);
            CollectionAssert.Contains(result, students[1]);
        }

        [TestMethod]
        public async Task GetAllStudentsAsync_Should_Return_Empty_List_When_No_Students()
        {
            var students = new List<Student>();

            _mockRepo
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(students);

            var result = await _studentService.GetAllStudentsAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        #endregion

        #region UpdateStudentAsync Tests

        [TestMethod]
        public async Task UpdateStudentAsync_Should_Throw_ValidationException_When_Invalid()
        {
            var student = new Student
            {
                Id = 1,
                FirstName = "Invalid",
                LastName = "Update",
                CNP = "123456789012", 
                Address = "999 Invalid Street",
                UnivCode = "UNIV999",
                Emails = [ "invalid.update@example.com" ],
                PhoneNumbers = [ "0123 456789" ]
            };

            var validationResult = new ValidationResult(new List<ValidationFailure>
            {
                new("CNP", "CNP must be exactly 13 characters.")
            });

            _mockValidator
                .Setup(v => v.ValidateAsync(student, It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(validationResult);

            await Assert.ThrowsExceptionAsync<ValidationException>(() => _studentService.UpdateStudentAsync(student));
            _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Student>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateStudentAsync_Should_Update_Student_When_Valid()
        {
            var student = new Student
            {
                Id = 1,
                FirstName = "Emily",
                LastName = "Davis",
                CNP = "1234567890123",
                Address = "111 Cedar Lane",
                UnivCode = "UNIV111",
                Emails = [ "emily.davis@example.com" ],
                PhoneNumbers = [ "0456 789012" ]
            };

            var validationResult = new ValidationResult();
            _mockValidator
                .Setup(v => v.ValidateAsync(student, It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(validationResult);

            _mockRepo
                .Setup(r => r.UpdateAsync(student))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _studentService.UpdateStudentAsync(student);

            _mockRepo.Verify(r => r.UpdateAsync(student), Times.Once);
        }

        #endregion

        #region DeleteStudentAsync Tests

        [TestMethod]
        public async Task DeleteStudentAsync_Should_Call_Delete_When_Student_Exists()
        {
            var studentId = 1;
            _mockRepo
                .Setup(r => r.DeleteAsync(studentId))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _studentService.DeleteStudentAsync(studentId);

            _mockRepo.Verify(r => r.DeleteAsync(studentId), Times.Once);
        }

        [TestMethod]
        public async Task DeleteStudentAsync_Should_Call_Delete_When_Student_Does_Not_Exist()
        {
            var studentId = 99;
            _mockRepo
                .Setup(r => r.DeleteAsync(studentId))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _studentService.DeleteStudentAsync(studentId);

            _mockRepo.Verify(r => r.DeleteAsync(studentId), Times.Once);
        }

        #endregion

        #region Additional Tests


        [TestMethod]
        public async Task CreateStudentAsync_Should_Throw_ValidationException_When_No_Email_Or_Phone()
        {
            var student = new Student
            {
                FirstName = "Tom",
                LastName = "Harris",
                CNP = "1234567890123",
                Address = "222 Birch Street",
                UnivCode = "UNIV222",
                Emails = [], 
                PhoneNumbers = []
            };

            var validationResult = new ValidationResult(new List<ValidationFailure>
            {
                new("", "At least one phone number or one email address must be provided.")
            });

            _mockValidator
                .Setup(v => v.ValidateAsync(student, It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(validationResult);

            await Assert.ThrowsExceptionAsync<ValidationException>(() => _studentService.CreateStudentAsync(student));
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<Student>()), Times.Never);
        }

        [TestMethod]
        public async Task CreateStudentAsync_Should_Throw_ValidationException_When_Invalid_Email_Format()
        {
            var student = new Student
            {
                FirstName = "Lucy",
                LastName = "Martinez",
                CNP = "1234567890123",
                Address = "333 Pine Street",
                UnivCode = "UNIV333",
                Emails = [], 
                PhoneNumbers = []
            };

            var validationResult = new ValidationResult(new List<ValidationFailure>
            {
                new("Emails[0]", "Each email must be a valid email address.")
            });

            _mockValidator
                .Setup(v => v.ValidateAsync(student, It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(validationResult);

            await Assert.ThrowsExceptionAsync<ValidationException>(() => _studentService.CreateStudentAsync(student));
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<Student>()), Times.Never);
        }

        [TestMethod]
        public async Task CreateStudentAsync_Should_Throw_ValidationException_When_Invalid_Phone_Format()
        {
            var student = new Student
            {
                FirstName = "Mark",
                LastName = "Lee",
                CNP = "1234567890123",
                Address = "444 Walnut Avenue",
                UnivCode = "UNIV444",
                Emails = [],
                PhoneNumbers = [] 
            };

            var validationResult = new ValidationResult(new List<ValidationFailure>
            {
                new("PhoneNumbers[0]", "Each phone number must follow the Romanian format: 4 digits prefix and 6 digits number, optionally separated by a space.")
            });

            _mockValidator
                .Setup(v => v.ValidateAsync(student, It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(validationResult);

            await Assert.ThrowsExceptionAsync<ValidationException>(() => _studentService.CreateStudentAsync(student));
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<Student>()), Times.Never);
        }

        #endregion

        #region PromoteStudentAsync Tests

        [TestMethod]
        public async Task PromoteStudentAsync_Should_Promote_Student_When_Credits_Are_Sufficient()
        {
            var studentId = 1;
            var currentSemester = new Semester { Id = 1, Number = 1, MinCredits = 30 };
            var nextSemester = new Semester { Id = 2, Number = 2, MinCredits = 40 };

            var course1 = new Course { Id = 101, Credits = 15 };
            var course2 = new Course { Id = 102, Credits = 15 };

            var enrollments = new List<Enrollment>
    {
        new() { StudentId = studentId, SemesterId = 1, Course = course1, CourseId = 101 },
        new() { StudentId = studentId, SemesterId = 1, Course = course2, CourseId = 102 }
    };

            var exams = new List<Exam>
    {
        new() { StudentId = studentId, CourseId = 101, Grade = 7 }, 
        new() { StudentId = studentId, CourseId = 102, Grade = 6 }  
    };

            _mockRepo.Setup(r => r.GetByIdAsync(studentId)).ReturnsAsync(new Student { Id = studentId });
            _mockEnrollmentRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(enrollments);
            _mockExamRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(exams);
            _mockSemesterRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(currentSemester);
            _mockSemesterRepo.Setup(r => r.GetByNumberAsync(2)).ReturnsAsync(nextSemester);

            await _studentService.PromoteStudentAsync(studentId);

            _mockEnrollmentRepo.Verify(r => r.UpdateAsync(It.Is<Enrollment>(e => e.SemesterId == 2)), Times.Exactly(2));
        }


        [TestMethod]
        public async Task PromoteStudentAsync_Should_Throw_Exception_When_Student_Has_No_Enrollments()
        {
            var studentId = 1;
            _mockRepo.Setup(r => r.GetByIdAsync(studentId)).ReturnsAsync(new Student { Id = studentId });
            _mockEnrollmentRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]); 

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _studentService.PromoteStudentAsync(studentId));

            _mockEnrollmentRepo.Verify(r => r.UpdateAsync(It.IsAny<Enrollment>()), Times.Never);
        }

        [TestMethod]
        public async Task PromoteStudentAsync_Should_Throw_Exception_When_Student_Has_Insufficient_Credits()
        {
            var studentId = 1;
            var currentSemester = new Semester { Id = 1, Number = 1, MinCredits = 30 };

            var enrollments = new List<Enrollment>
    {
        new() { StudentId = studentId, SemesterId = 1, Course = new Course { Credits = 20 } }
    };

            var exams = new List<Exam>
    {
        new() { StudentId = studentId, CourseId = 1, Grade = 7 }
    };

            _mockRepo.Setup(r => r.GetByIdAsync(studentId)).ReturnsAsync(new Student { Id = studentId });
            _mockEnrollmentRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(enrollments);
            _mockExamRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(exams);
            _mockSemesterRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(currentSemester);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _studentService.PromoteStudentAsync(studentId));

            _mockEnrollmentRepo.Verify(r => r.UpdateAsync(It.IsAny<Enrollment>()), Times.Never);
        }

        [TestMethod]
        public async Task PromoteStudentAsync_Should_Throw_Exception_When_Next_Semester_Does_Not_Exist()
        {
            var studentId = 1;
            var currentSemester = new Semester { Id = 1, Number = 1, MinCredits = 30 };

            var enrollments = new List<Enrollment>
    {
        new() { StudentId = studentId, SemesterId = 1, Course = new Course { Credits = 30 } }
    };

            var exams = new List<Exam>
    {
        new() { StudentId = studentId, CourseId = 1, Grade = 7 } 
    };

            _mockRepo.Setup(r => r.GetByIdAsync(studentId)).ReturnsAsync(new Student { Id = studentId });
            _mockEnrollmentRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(enrollments);
            _mockExamRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(exams);
            _mockSemesterRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(currentSemester);
            _mockSemesterRepo.Setup(r => r.GetByNumberAsync(2)).ReturnsAsync((Semester?)null); 

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _studentService.PromoteStudentAsync(studentId));

            _mockEnrollmentRepo.Verify(r => r.UpdateAsync(It.IsAny<Enrollment>()), Times.Never);
        }

        [TestMethod]
        public async Task PromoteStudentAsync_Should_Throw_Exception_When_Student_Does_Not_Exist()
        {
            var studentId = 1;
            _mockRepo.Setup(r => r.GetByIdAsync(studentId)).ReturnsAsync((Student?)null); 

            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() =>
                _studentService.PromoteStudentAsync(studentId));

            _mockEnrollmentRepo.Verify(r => r.UpdateAsync(It.IsAny<Enrollment>()), Times.Never);
        }

        #endregion

        [TestMethod]
        public async Task GetStudentCreditReportAsync_Should_Return_Correct_Credits()
        {
            var studentId = 1;
            var semesters = new List<Semester>
    {
        new() { Id = 1, Number = 1 },
        new() { Id = 2, Number = 2 } 
    };

            var enrollments = new List<Enrollment>
    {
        new() { StudentId = studentId, SemesterId = 1, CourseId = 101, Course = new Course { Credits = 10 } },
        new() { StudentId = studentId, SemesterId = 1, CourseId = 102, Course = new Course { Credits = 15 } },
        new() { StudentId = studentId, SemesterId = 2, CourseId = 103, Course = new Course { Credits = 20 } }
    };

            var exams = new List<Exam>
    {
        new() { StudentId = studentId, CourseId = 101, Grade = 6 }, 
        new() { StudentId = studentId, CourseId = 102, Grade = 7 }, 
        new() { StudentId = studentId, CourseId = 103, Grade = 5 }  
    };

            _mockRepo.Setup(r => r.GetByIdAsync(studentId)).ReturnsAsync(new Student { Id = studentId });
            _mockEnrollmentRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(enrollments);
            _mockExamRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(exams);

            _mockSemesterRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                             .ReturnsAsync((int id) => semesters.FirstOrDefault(s => s.Id == id));

            var result = await _studentService.GetStudentCreditReportAsync(studentId);

            Assert.AreEqual(2, result.Count); 
            Assert.AreEqual(25, result[1]);  
            Assert.AreEqual(20, result[2]);  
        }



    }
}
