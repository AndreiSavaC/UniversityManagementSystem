namespace DataAccessLayer.Repositories.Implementations
{
    using Microsoft.EntityFrameworkCore;
    using DomainModel.Entities;
    using DataAccessLayer.Context;
    using DataAccessLayer.Repositories.Interfaces;
    using log4net;
    public class ExamRepository(UniversityDbContext context) : IExamRepository
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ExamRepository));

        private readonly UniversityDbContext _context = context;

        public async Task<Exam?> GetByIdAsync(int id)
        {
            Logger.Info($"Fetching Exam with ID = {id}.");
            return await _context.Exams
                .Include(e => e.Student)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<Exam>> GetAllAsync()
        {
            Logger.Info("Fetching all Exams.");
            return await _context.Exams
                .Include(e => e.Student)
                .Include(e => e.Course)
                .ToListAsync();
        }

        public async Task AddAsync(Exam exam)
        {
            Logger.Info("Adding a new Exam.");
            await _context.Exams.AddAsync(exam);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Exam exam)
        {
            Logger.Info($"Updating Exam with ID = {exam.Id}.");
            _context.Exams.Update(exam);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            Logger.Info($"Deleting Exam with ID = {id}.");
            var entity = await _context.Exams.FindAsync(id);
            if (entity != null)
            {
                _context.Exams.Remove(entity);
                await _context.SaveChangesAsync();
            }
            else
            {
                Logger.Warn($"Exam with ID = {id} was not found.");
            }
        }
    }
}
