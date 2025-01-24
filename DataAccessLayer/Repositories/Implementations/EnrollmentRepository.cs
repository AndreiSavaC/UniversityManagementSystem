namespace DataAccessLayer.Repositories.Implementations
{
    using Microsoft.EntityFrameworkCore;
    using DomainModel.Entities;
    using DataAccessLayer.Context;
    using DataAccessLayer.Repositories.Interfaces;
    using log4net;
    public class EnrollmentRepository(UniversityDbContext context) : IEnrollmentRepository
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(EnrollmentRepository));

        private readonly UniversityDbContext _context = context;

        public async Task<Enrollment?> GetByIdAsync(int id)
        {
            Logger.Info($"Fetching Enrollment with ID = {id}.");
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Include(e => e.Semester)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<Enrollment>> GetAllAsync()
        {
            Logger.Info("Fetching all Enrollments.");
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Include(e => e.Semester)
                .ToListAsync();
        }

        public async Task AddAsync(Enrollment enrollment)
        {
            Logger.Info("Adding a new Enrollment.");
            await _context.Enrollments.AddAsync(enrollment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Enrollment enrollment)
        {
            Logger.Info($"Updating Enrollment with ID = {enrollment.Id}.");
            _context.Enrollments.Update(enrollment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            Logger.Info($"Deleting Enrollment with ID = {id}.");
            var entity = await _context.Enrollments.FindAsync(id);
            if (entity != null)
            {
                _context.Enrollments.Remove(entity);
                await _context.SaveChangesAsync();
            }
            else
            {
                Logger.Warn($"Enrollment with ID = {id} was not found.");
            }
        }
    }
}
