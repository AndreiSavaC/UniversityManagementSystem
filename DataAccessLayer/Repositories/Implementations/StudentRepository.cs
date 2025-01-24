namespace DataAccessLayer.Repositories.Implementations
{
    using Microsoft.EntityFrameworkCore;
    using DomainModel.Entities;
    using DataAccessLayer.Context;
    using DataAccessLayer.Repositories.Interfaces;
    using log4net;
    public class StudentRepository(UniversityDbContext context) : IStudentRepository
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(StudentRepository));

        private readonly UniversityDbContext _context = context;

        public async Task<Student?> GetByIdAsync(int id)
        {
            Logger.Info($"Fetching Student with ID = {id}.");
            return await _context.Students
                .Include(s => s.Enrollments)
                .Include(s => s.Exams)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<Student>> GetAllAsync()
        {
            Logger.Info("Fetching all Students.");
            return await _context.Students
                .Include(s => s.Enrollments)
                .Include(s => s.Exams)
                .ToListAsync();
        }

        public async Task AddAsync(Student student)
        {
            Logger.Info($"Adding a new Student: {student.FirstName} {student.LastName}.");
            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Student student)
        {
            Logger.Info($"Updating Student with ID = {student.Id}.");
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            Logger.Info($"Deleting Student with ID = {id}.");
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }
            else
            {
                Logger.Warn($"Student with ID = {id} was not found.");
            }
        }
    }
}
