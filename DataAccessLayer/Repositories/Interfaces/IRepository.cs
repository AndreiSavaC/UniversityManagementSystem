namespace DataAccessLayer.Repositories.Interfaces
{
    /// <summary>
    /// Generic repository interface for CRUD operations.
    /// </summary>
    /// <typeparam name="T">The type of entity, must be a class.</typeparam>
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<List<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
    }
}
