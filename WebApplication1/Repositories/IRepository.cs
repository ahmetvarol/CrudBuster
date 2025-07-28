namespace WebApplication1.Repositories;

public interface IRepository<T>
    where T  : class
{
    Task<List<T>> GetListAsync();
    Task<T> GetAsync(Guid id);
    Task CreateAsync(T entity);
    Task UpdateAsync(T entity);
    Task<bool> DeleteAsync(Guid id);
}