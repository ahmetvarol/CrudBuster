namespace CrudBuster;

public static class CrudDelegateCache
{
    private static readonly Dictionary<string, Delegate> _cache = new();
    private static string CreateMethod { get; set; }
    private static string UpdateMethod { get; set; }
    private static string DeleteMethod { get; set; }
    private static string GetByMethod { get; set; }
    private static string GetListMethod { get; set; }

    public static void SetMethods(CrudOptions options)
    {
        CreateMethod = options.CreateService;
        UpdateMethod = options.UpdateService;
        DeleteMethod = options.DeleteService;
        GetListMethod = options.GetListService;
        GetByMethod = options.GetByIdService;
    }
    
    public static Func<object, Task<object>> GetListDelegate(Type serviceType)
    {
        var key = $"{serviceType.FullName}.{GetListMethod}";

        if (_cache.TryGetValue(key, out var cached))
            return (Func<object, Task<object>>)cached;

        var method = serviceType.GetMethod(GetListMethod);

        Func<object, Task<object>> del = async (service) =>
        {
            var task = (Task)method.Invoke(service, null);
            await task.ConfigureAwait(false);
            return task.GetType().GetProperty("Result")?.GetValue(task);
        };

        _cache[key] = del;
        return del;
    }

    public static Func<object, Guid, Task<object>> GetByIdDelegate(Type serviceType)
    {
        var key = $"{serviceType.FullName}.{GetByMethod}";

        if (_cache.TryGetValue(key, out var cached))
            return (Func<object, Guid, Task<object>>)cached;

        var method = serviceType.GetMethod(GetByMethod);

        Func<object, Guid, Task<object>> del = async (service, id) =>
        {
            var task = (Task)method.Invoke(service, new object[] { id });
            await task.ConfigureAwait(false);
            return task.GetType().GetProperty("Result")?.GetValue(task);
        };

        _cache[key] = del;
        return del;
    }
    
    public static Func<object, object, Task<object>> CreateDelegate(Type serviceType)
    {
        var key = $"{serviceType.FullName}.{CreateMethod}";

        if (_cache.TryGetValue(key, out var cached))
            return (Func<object, object, Task<object>>)cached;

        var method = serviceType.GetMethod(CreateMethod);

        Func<object, object, Task<object>> del = async (service, dto) =>
        {
            var task = (Task)method.Invoke(service, new object[] { dto });
            await task.ConfigureAwait(false);
            return task.GetType().GetProperty("Result")?.GetValue(task);
        };

        _cache[key] = del;
        return del;
    }
    
    public static Func<object, object, Task> UpdateDelegate(Type serviceType)
    {
        var key = $"{serviceType.FullName}.{UpdateMethod}";

        if (_cache.TryGetValue(key, out var cached))
            return (Func<object, object, Task>)cached;

        var method = serviceType.GetMethod(UpdateMethod);

        Func<object, object, Task> del = async (service, dto) =>
        {
            var task = (Task)method.Invoke(service, new object[] { dto });
            await task.ConfigureAwait(false);
        };

        _cache[key] = del;
        return del;
    }
    
    public static Func<object, object, Task> DeleteDelegate(Type serviceType)
    {
        var key = $"{serviceType.FullName}.{DeleteMethod}";

        if (_cache.TryGetValue(key, out var cached))
            return (Func<object, object, Task>)cached;

        var method = serviceType.GetMethod(DeleteMethod);

        Func<object, object, Task> del = async (service, id) =>
        {
            var task = (Task)method.Invoke(service, new object[] { id });
            await task.ConfigureAwait(false);
        };

        _cache[key] = del;
        return del;
    }

}