namespace TestServiceLayer.Helpers
{
    using DomainModel.Entities;
    using System.Collections.Generic;

    public static class StudentTestHelper
    {
        public static Student CreateStudent(
            int? id = null,
            string? firstName = null,
            string? lastName = null,
            string? cnp = null,
            string? address = null,
            string? univCode = null,
            List<string>? emails = null,
            List<string>? phoneNumbers = null)
        {

            return new Student
            {
                Id = id ?? 1,
                FirstName = firstName ?? "Ion",
                LastName = lastName ?? "Popescu",
                CNP = cnp ?? "1234567892312",
                Address = address ?? "Strada Exemplu, Nr. 1, Bucuresti",
                UnivCode = univCode ?? "UNI12345",
                Emails = emails ?? ["ion.popescu@example.com"],
                PhoneNumbers = phoneNumbers ?? ["0712345678"]
            };
        }
    }
}
