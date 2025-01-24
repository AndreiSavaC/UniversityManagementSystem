using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DomainModel.Entities;
using DataAccessLayer.Context;
using DataAccessLayer.Repositories.Interfaces;
using log4net;

namespace DataAccessLayer.Repositories.Implementations
{
    public class CoursePrerequisiteRepository(UniversityDbContext context) : ICoursePrerequisiteRepository
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CoursePrerequisiteRepository));

        private readonly UniversityDbContext _context = context;

        public async Task<CoursePrerequisite?> GetByIdAsync(int id)
        {
            Logger.Info($"Fetching CoursePrerequisite with ID = {id}.");
            return await _context.CoursePrerequisites
                .Include(cp => cp.Course)
                .Include(cp => cp.PrerequisiteCourse)
                .FirstOrDefaultAsync(cp => cp.Id == id);
        }

        public async Task<List<CoursePrerequisite>> GetAllAsync()
        {
            Logger.Info("Fetching all CoursePrerequisites.");
            return await _context.CoursePrerequisites
                .Include(cp => cp.Course)
                .Include(cp => cp.PrerequisiteCourse)
                .ToListAsync();
        }

        public async Task AddAsync(CoursePrerequisite cp)
        {
            Logger.Info("Adding a new CoursePrerequisite.");
            await _context.CoursePrerequisites.AddAsync(cp);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CoursePrerequisite cp)
        {
            Logger.Info($"Updating CoursePrerequisite with ID = {cp.Id}.");
            _context.CoursePrerequisites.Update(cp);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            Logger.Info($"Deleting CoursePrerequisite with ID = {id}.");
            var entity = await _context.CoursePrerequisites.FindAsync(id);
            if (entity != null)
            {
                _context.CoursePrerequisites.Remove(entity);
                await _context.SaveChangesAsync();
            }
            else
            {
                Logger.Warn($"CoursePrerequisite with ID = {id} was not found.");
            }
        }
    }
}
