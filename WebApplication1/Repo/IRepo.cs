using WebApplication1.Entities;

namespace WebApplication1.Repo;

public interface IRepo<T> where T : class
{
    Task Get();
    Task GetBy();
    Task Add();
    Task Update();
    Task Delete();
}