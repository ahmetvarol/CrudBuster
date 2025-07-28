namespace WebApplication1.Repositories;

public class Repository<T> : IRepository<T>
where T : class
{
    public Task<List<T>> GetListAsync()
    {
        throw new NotImplementedException();
    }

    public Task<T> GetAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task CreateAsync(T entity)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(T entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}