namespace DataAccessLayer.Repositories.Implementations
{
    using Microsoft.EntityFrameworkCore;
    using DomainModel.Entities;
    using DataAccessLayer.Context;
    using DataAccessLayer.Repositories.Interfaces;
    using log4net;
    public class CourseSemesterRepository(UniversityDbContext context) : ICourseSemesterRepository
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CourseSemesterRepository));

        private readonly UniversityDbContext _context = context;

        public async Task<CourseSemester?> GetByIdAsync(int id)
        {
            Logger.Info($"Fetching CourseSemester with ID = {id}.");
            return await _context.CourseSemesters
                .Include(cs => cs.Course)
                .Include(cs => cs.Semester)
                .FirstOrDefaultAsync(cs => cs.Id == id);
        }

        public async Task<List<CourseSemester>> GetAllAsync()
        {
            Logger.Info("Fetching all CourseSemesters.");
            return await _context.CourseSemesters
                .Include(cs => cs.Course)
                .Include(cs => cs.Semester)
                .ToListAsync();
        }

        public async Task AddAsync(CourseSemester courseSemester)
        {
            Logger.Info("Adding a new CourseSemester.");
            await _context.CourseSemesters.AddAsync(courseSemester);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CourseSemester courseSemester)
        {
            Logger.Info($"Updating CourseSemester with ID = {courseSemester.Id}.");
            _context.CourseSemesters.Update(courseSemester);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            Logger.Info($"Deleting CourseSemester with ID = {id}.");
            var entity = await _context.CourseSemesters.FindAsync(id);
            if (entity != null)
            {
                _context.CourseSemesters.Remove(entity);
                await _context.SaveChangesAsync();
            }
            else
            {
                Logger.Warn($"CourseSemester with ID = {id} was not found.");
            }
        }
    }
}
