namespace WebApplication1.Repo;

public class Repo<T> : IRepo<T> where T : class
{
    public Task Get()
    {
        throw new NotImplementedException();
    }

    public Task GetBy()
    {
        throw new NotImplementedException();
    }

    public Task Add()
    {
        throw new NotImplementedException();
    }

    public Task Update()
    {
        throw new NotImplementedException();
    }

    public Task Delete()
    {
        throw new NotImplementedException();
    }
}