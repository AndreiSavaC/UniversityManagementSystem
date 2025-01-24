namespace DataAccessLayer.Repositories.Implementations
{
    using Microsoft.EntityFrameworkCore;
    using DataAccessLayer.Repositories.Interfaces;
    using DataAccessLayer.Context;
    using DomainModel.Entities;
    using log4net;
    public class CourseDiscountRepository(UniversityDbContext context) : ICourseDiscountRepository
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CourseDiscountRepository));

        private readonly UniversityDbContext _context = context;

        public async Task<CourseDiscount?> GetByIdAsync(int id)
        {
            Logger.Info($"Fetching CourseDiscount with ID = {id}.");
            return await _context.CourseDiscounts
                .Include(cd => cd.Course)
                .FirstOrDefaultAsync(cd => cd.Id == id);
        }

        public async Task<List<CourseDiscount>> GetAllAsync()
        {
            Logger.Info("Fetching all CourseDiscounts.");
            return await _context.CourseDiscounts
                .Include(cd => cd.Course)
                .ToListAsync();
        }

        public async Task<bool> ExistsGroupIdAsync(int groupId)
        {
            return await _context.CourseDiscounts.AnyAsync(cd => cd.GroupId == groupId);
        }

        public async Task<List<int>> GetCourseIdsByGroupIdAsync(int groupId)
        {
            return await _context.CourseDiscounts
                .Where(cd => cd.GroupId == groupId)
                .Select(cd => cd.CourseId)
                .ToListAsync();
        }

        public async Task AddAsync(CourseDiscount discount)
        {
            Logger.Info("Adding a new CourseDiscount.");
            await _context.CourseDiscounts.AddAsync(discount);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CourseDiscount discount)
        {
            Logger.Info($"Updating CourseDiscount with ID = {discount.Id}.");
            _context.CourseDiscounts.Update(discount);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            Logger.Info($"Deleting CourseDiscount with ID = {id}.");
            var entity = await _context.CourseDiscounts.FindAsync(id);
            if (entity != null)
            {
                _context.CourseDiscounts.Remove(entity);
                await _context.SaveChangesAsync();
                Logger.Info($"CourseDiscount with ID {id} deleted successfully.");
            }
            else
            {
                Logger.Warn($"CourseDiscount with ID = {id} was not found.");
            }
        }
    }
}
