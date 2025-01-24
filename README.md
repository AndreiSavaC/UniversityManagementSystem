# University Management System

## Description

The **University Management System** is an application developed to manage information related to students, courses, enrollments, and exams within a university setting. The primary objective of this project was to learn and apply unit testing principles, ensuring high code coverage through comprehensive tests.

## Features

- **Student Management**: Add, update, and delete student information.
- **Course Administration**: Create and manage course details, including prerequisites and discounts.
- **Enrollments**: Enroll students in courses for specific semesters with rigorous validations.
- **Exams**: Manage exams, grades, and their relationships with students and courses.
- **Advanced Validations**: Utilize FluentValidation to ensure data integrity.
- **Detailed Logging**: Implement log4net for application monitoring and debugging.
- **Extensive Unit Testing**: Write tests using Moq and FluentValidation.TestHelper to ensure maximum code coverage.

## Technologies Used

- **Programming Language**: C#
- **Framework**: .NET Core
- **ORM**: Entity Framework Core
- **Database**: PostgreSQL
- **Validation**: FluentValidation
- **Logging**: log4net
- **Testing**: MSTest, Moq, FluentValidation.TestHelper

## Project Structure

- **DataAccessLayer**: Manages database connections and implements repositories.
- **DomainModel**: Defines entities and their relationships.
- **ServiceLayer**: Implements business logic and services.
- **Validators**: Validation rules for entities using FluentValidation.
- **TestServiceLayer**: Unit tests for services in the service layer.
- **TestServiceLayer.Validators**: Unit tests for validators.

## Project Purpose

This project was created for educational purposes to:

- **Learn Unit Testing**: Apply unit tests to ensure the correct functionality of components.
- **Ensure High Code Coverage**: Write comprehensive tests that cover most possible scenarios.
- **Implement Best Practices**: Use SOLID principles, design patterns, and layered architecture to create clean and maintainable code.
