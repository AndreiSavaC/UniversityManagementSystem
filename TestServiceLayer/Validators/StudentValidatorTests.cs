namespace TestServiceLayer.Validators
{
    using FluentValidation.TestHelper;
    using ServiceLayer.Validators;
    using TestServiceLayer.Helpers;

    [TestClass]
    public class StudentValidatorTests
    {
        private StudentValidator _validator;

        [TestInitialize]
        public void Setup()
        {
            _validator = new StudentValidator();
        }

        [TestMethod]
        public void Should_Not_Have_Error_For_Valid_Student()
        {
            var student = StudentTestHelper.CreateStudent();

            var result = _validator.TestValidate(student);
            result.ShouldNotHaveAnyValidationErrors();
        }


        #region Id Tests

        [TestMethod]
        public void Should_Have_Error_When_Id_Is_Invalid()
        {
            var student = StudentTestHelper.CreateStudent(id: -1);

            var result = _validator.TestValidate(student);
            result.ShouldHaveValidationErrorFor(s => s.Id)
                  .WithErrorMessage("ID must be a positive number.");
        }

        #endregion

        #region FirstName Tests

        [TestMethod]
        public void Should_Have_Error_When_FirstName_Is_Empty()
        {
            var student = StudentTestHelper.CreateStudent(firstName: "");

            var result = _validator.TestValidate(student);
            result.ShouldHaveValidationErrorFor(s => s.FirstName)
                  .WithErrorMessage("First name is required.");
        }

        [TestMethod]
        public void Should_Have_Error_When_FirstName_Exceeds_MaxLength()
        {
            var student = StudentTestHelper.CreateStudent(firstName: new string('A', 51));

            var result = _validator.TestValidate(student);
            result.ShouldHaveValidationErrorFor(s => s.FirstName)
                  .WithErrorMessage("First name cannot exceed 50 characters.");
        }

        [TestMethod]
        public void Should_Not_Have_Error_When_FirstName_Is_Valid()
        {
            var student = StudentTestHelper.CreateStudent(firstName: "Marius");

            var result = _validator.TestValidate(student);
            result.ShouldNotHaveValidationErrorFor(s => s.FirstName);
        }

        #endregion

        #region LastName Tests

        [TestMethod]
        public void Should_Have_Error_When_LastName_Is_Empty()
        {
            var student = StudentTestHelper.CreateStudent(lastName: "");

            var result = _validator.TestValidate(student);
            result.ShouldHaveValidationErrorFor(s => s.LastName)
                  .WithErrorMessage("Last name is required.");
        }

        [TestMethod]
        public void Should_Have_Error_When_LastName_Exceeds_MaxLength()
        {
            var student = StudentTestHelper.CreateStudent(lastName: new string('B', 51));

            var result = _validator.TestValidate(student);
            result.ShouldHaveValidationErrorFor(s => s.LastName)
                  .WithErrorMessage("Last name cannot exceed 50 characters.");
        }

        [TestMethod]
        public void Should_Not_Have_Error_When_LastName_Is_Valid()
        {
            var student = StudentTestHelper.CreateStudent(lastName: "Bimes");

            var result = _validator.TestValidate(student);
            result.ShouldNotHaveValidationErrorFor(s => s.LastName);
        }

        #endregion

        #region CNP Tests

        [TestMethod]
        public void Should_Have_Error_When_CNP_Is_Empty()
        {

            var student = StudentTestHelper.CreateStudent(cnp: "");

            var result = _validator.TestValidate(student);
            result.ShouldHaveValidationErrorFor(s => s.CNP)
                  .WithErrorMessage("CNP is required.");
        }

        [TestMethod]
        public void Should_Have_Error_When_CNP_Is_Not_13_Characters()
        {
            var student = StudentTestHelper.CreateStudent(cnp: "1234567890  23");

            var result = _validator.TestValidate(student);
            result.ShouldHaveValidationErrorFor(s => s.CNP)
                  .WithErrorMessage("CNP must be exactly 13 characters.");
        }

        [TestMethod]
        public void Should_Have_Error_When_CNP_Is_Not_All_Digits()
        {
            var student = StudentTestHelper.CreateStudent(cnp: "12345678901AB");

            var result = _validator.TestValidate(student);
            result.ShouldHaveValidationErrorFor(s => s.CNP)
                  .WithErrorMessage("CNP must consist of exactly 13 digits.");
        }

        [TestMethod]
        public void Should_Not_Have_Error_When_CNP_Is_Valid()
        {
            var student = StudentTestHelper.CreateStudent(cnp: "1234567890123");

            var result = _validator.TestValidate(student);
            result.ShouldNotHaveValidationErrorFor(s => s.CNP);
        }

        #endregion

        #region Address Tests

        [TestMethod]
        public void Should_Have_Error_When_Address_Is_Empty()
        {
            var student = StudentTestHelper.CreateStudent(address: "");

            var result = _validator.TestValidate(student);
            result.ShouldHaveValidationErrorFor(s => s.Address)
                  .WithErrorMessage("Address is required.");
        }

        [TestMethod]
        public void Should_Not_Have_Error_When_Address_Is_Valid()
        {
            var student = StudentTestHelper.CreateStudent(address: "Str. Exemplu 1");

            var result = _validator.TestValidate(student);
            result.ShouldNotHaveValidationErrorFor(s => s.Address);
        }

        #endregion

        #region UnivCode Tests

        [TestMethod]
        public void Should_Have_Error_When_UnivCode_Is_Empty()
        {
            var student = StudentTestHelper.CreateStudent(univCode: "");

            var result = _validator.TestValidate(student);
            result.ShouldHaveValidationErrorFor(s => s.UnivCode)
                  .WithErrorMessage("University code is required.");
        }

        [TestMethod]
        public void Should_Have_Error_When_UnivCode_Exceeds_MaxLength()
        {
            var student = StudentTestHelper.CreateStudent(univCode: new string('U', 21));

            var result = _validator.TestValidate(student);
            result.ShouldHaveValidationErrorFor(s => s.UnivCode)
                  .WithErrorMessage("University code cannot exceed 20 characters.");
        }

        [TestMethod]
        public void Should_Not_Have_Error_When_UnivCode_Is_Valid()
        {
            var student = StudentTestHelper.CreateStudent(univCode: "UNI123");

            var result = _validator.TestValidate(student);
            result.ShouldNotHaveValidationErrorFor(s => s.UnivCode);
        }

        #endregion

        #region Emails Tests

        [TestMethod]
        public void Should_Have_Error_When_Email_Is_Invalid()
        {

            var student = StudentTestHelper.CreateStudent(emails: ["invalid-email"]);

            var result = _validator.TestValidate(student);
            result.ShouldHaveValidationErrorFor("Emails[0]")
                  .WithErrorMessage("Each email must be a valid email address.");
        }

        [TestMethod]
        public void Should_Not_Have_Error_When_Email_Is_Valid()
        {

            var student = StudentTestHelper.CreateStudent(emails: ["test@example.com"]);

            var result = _validator.TestValidate(student);
            result.ShouldNotHaveValidationErrorFor("Emails[0]");
        }

        [TestMethod]
        public void Should_Have_Errors_When_Multiple_Emails_Are_Invalid()
        {

            var student = StudentTestHelper.CreateStudent(emails: ["invalid-email@", "@domain.com", "valid@example.com"]);

            var result = _validator.TestValidate(student);

            result.ShouldHaveValidationErrorFor("Emails[0]")
                  .WithErrorMessage("Each email must be a valid email address.");
            result.ShouldHaveValidationErrorFor("Emails[1]")
                  .WithErrorMessage("Each email must be a valid email address.");
            result.ShouldNotHaveValidationErrorFor("Emails[2]");
        }

        #endregion

        #region PhoneNumbers Tests

        [TestMethod]
        public void Should_Have_Error_When_PhoneNumber_Is_Invalid_Format()
        {

            var student = StudentTestHelper.CreateStudent(phoneNumbers: ["1234567890"]);

            var result = _validator.TestValidate(student);

            result.ShouldHaveValidationErrorFor("PhoneNumbers[0]")
                  .WithErrorMessage("Each phone number must follow the Romanian format: 07 followed by 2 digits, optionally a space, and then 6 digits.");
        }

        [TestMethod]
        public void Should_Not_Have_Error_When_PhoneNumber_Is_Valid_With_Space()
        {

            var student = StudentTestHelper.CreateStudent(phoneNumbers: ["0712 345678"]);

            var result = _validator.TestValidate(student);
            result.ShouldNotHaveValidationErrorFor("PhoneNumbers[0]");
        }

        [TestMethod]
        public void Should_Not_Have_Error_When_PhoneNumber_Is_Valid_Without_Space()
        {

            var student = StudentTestHelper.CreateStudent(phoneNumbers: ["0712345678"]);

            var result = _validator.TestValidate(student);
            result.ShouldNotHaveValidationErrorFor("PhoneNumbers[0]");
        }

        [TestMethod]
        public void Should_Have_Errors_When_Multiple_PhoneNumbers_Are_Invalid()
        {

            var student = StudentTestHelper.CreateStudent(phoneNumbers: ["1234567890", "0456 789012", "07890123 45"]);

            var result = _validator.TestValidate(student);

            result.ShouldHaveValidationErrorFor("PhoneNumbers[0]")
                  .WithErrorMessage("Each phone number must follow the Romanian format: 07 followed by 2 digits, optionally a space, and then 6 digits.");
            result.ShouldHaveValidationErrorFor("PhoneNumbers[1]")
                  .WithErrorMessage("Each phone number must follow the Romanian format: 07 followed by 2 digits, optionally a space, and then 6 digits.");
            result.ShouldHaveValidationErrorFor("PhoneNumbers[2]")
                  .WithErrorMessage("Each phone number must follow the Romanian format: 07 followed by 2 digits, optionally a space, and then 6 digits.");
        }

        #endregion

        #region AtLeastOneContactInfo Tests

        [TestMethod]
        public void Should_Have_Error_When_No_Email_Or_PhoneNumbers()
        {

            var student = StudentTestHelper.CreateStudent(emails: [], phoneNumbers: []);


            var result = _validator.TestValidate(student);
            result.ShouldHaveValidationErrorFor(s => s)
                  .WithErrorMessage("At least one phone number or one email address must be provided.");
        }

        [TestMethod]
        public void Should_Not_Have_Error_When_Has_Email_Only()
        {

            var student = StudentTestHelper.CreateStudent(emails: ["test@example.com"], phoneNumbers: []);

            var result = _validator.TestValidate(student);
            result.ShouldNotHaveValidationErrorFor(s => s);
        }

        [TestMethod]
        public void Should_Not_Have_Error_When_Has_PhoneNumber_Only()
        {

            var student = StudentTestHelper.CreateStudent(emails: [], phoneNumbers: ["0712 345678"]);

            var result = _validator.TestValidate(student);
            result.ShouldNotHaveValidationErrorFor(s => s);
        }

        [TestMethod]
        public void Should_Not_Have_Error_When_Has_Both_Email_And_PhoneNumber()
        {

            var student = StudentTestHelper.CreateStudent(emails: ["test@example.com"], phoneNumbers: ["0712 345678"]);

            var result = _validator.TestValidate(student);
            result.ShouldNotHaveValidationErrorFor(s => s);
        }

        #endregion


        [TestMethod]
        public void Should_Access_Enrollments()
        {
            var enrollment = EnrollmentTestHelper.CreateEnrollment();

            var student = StudentTestHelper.CreateStudent();
            student.Enrollments = [enrollment];

            var studentEnrollments = student.Enrollments;

            Assert.AreEqual(1, studentEnrollments.Count);
            Assert.AreEqual(101, studentEnrollments.First().CourseId);
        }



        [TestMethod]
        public void Should_Access_Exams()
        {
            var exam = ExamTestHelper.CreateExam();

            var student = StudentTestHelper.CreateStudent();
            student.Exams = [ exam ];

            var studentExams = student.Exams;

            Assert.AreEqual(1, studentExams.Count);
            Assert.AreEqual(exam.CourseId, studentExams.First().CourseId);
        }

    }
}