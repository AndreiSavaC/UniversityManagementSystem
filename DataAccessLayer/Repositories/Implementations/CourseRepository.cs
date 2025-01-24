namespace DataAccessLayer.Repositories.Implementations
{
    using Microsoft.EntityFrameworkCore;
    using DomainModel.Entities;
    using DataAccessLayer.Context;
    using DataAccessLayer.Repositories.Interfaces;
    using log4net;
    public class CourseRepository(UniversityDbContext context) : ICourseRepository
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CourseRepository));

        private readonly UniversityDbContext _context = context;

        public async Task<Course?> GetByIdAsync(int id)
        {
            Logger.Info($"Fetching Course with ID = {id}.");
            return await _context.Courses
                .Include(c => c.Enrollments)
                .Include(c => c.Exams)
                .Include(c => c.CourseSemesters)
                .Include(c => c.Prerequisites)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Course>> GetAllAsync()
        {
            Logger.Info("Fetching all Courses.");
            return await _context.Courses
                .Include(c => c.Enrollments)
                .Include(c => c.Exams)
                .Include(c => c.CourseSemesters)
                .Include(c => c.Prerequisites)
                .ToListAsync();
        }

        public async Task AddAsync(Course course)
        {
            Logger.Info($"Adding a new Course: {course.Name}.");
            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Course course)
        {
            Logger.Info($"Updating Course with ID = {course.Id}.");
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            Logger.Info($"Deleting Course with ID = {id}.");
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }
            else
            {
                Logger.Warn($"Course with ID = {id} was not found.");
            }
        }
    }
}
