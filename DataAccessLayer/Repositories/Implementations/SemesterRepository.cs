namespace DataAccessLayer.Repositories.Implementations
{
    using Microsoft.EntityFrameworkCore;
    using DomainModel.Entities;
    using DataAccessLayer.Context;
    using DataAccessLayer.Repositories.Interfaces;
    using log4net;
    public class SemesterRepository(UniversityDbContext context) : ISemesterRepository
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SemesterRepository));

        private readonly UniversityDbContext _context = context;

        public async Task<Semester?> GetByIdAsync(int id)
        {
            Logger.Info($"Fetching Semester with ID = {id}.");
            return await _context.Semesters
                .Include(s => s.CourseSemesters)
                .Include(s => s.Enrollments)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Semester?> GetByNumberAsync(int number)
        {
            Logger.Info($"Fetching Semester with Number = {number}.");
            return await _context.Semesters
                .FirstOrDefaultAsync(s => s.Number == number);
        }

        public async Task<List<Semester>> GetAllAsync()
        {
            Logger.Info("Fetching all Semesters.");
            return await _context.Semesters
                .Include(s => s.CourseSemesters)
                .Include(s => s.Enrollments)
                .ToListAsync();
        }

        public async Task AddAsync(Semester semester)
        {
            Logger.Info($"Adding a new Semester: {semester.Number}.");
            await _context.Semesters.AddAsync(semester);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Semester semester)
        {
            Logger.Info($"Updating Semester with ID = {semester.Id}.");
            _context.Semesters.Update(semester);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            Logger.Info($"Deleting Semester with ID = {id}.");
            var semester = await _context.Semesters.FindAsync(id);
            if (semester != null)
            {
                _context.Semesters.Remove(semester);
                await _context.SaveChangesAsync();
            }
            else
            {
                Logger.Warn($"Semester with ID = {id} was not found.");
            }
        }
    }
}
