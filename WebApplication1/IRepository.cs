namespace WebApplication1;

public interface IRepository
{
    Task<bool> GetAllListAsync();
}