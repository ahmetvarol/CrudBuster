using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace CrudBuster;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

public static class CrudBusterExtensions
{
    public static IEndpointRouteBuilder CrudBuster(
        this IEndpointRouteBuilder endpoints, 
        Action<CrudOptions>? configure = null)
    {
        var options = new CrudOptions();
        configure?.Invoke(options); 
        
        var domainAssembly = Assembly.Load(options.DomainLayerName);
        var viewModelsAssembly = Assembly.Load(options.ViewModelsLayerName);
        var repositoryAssembly = Assembly.Load(options.RepositoryLayerName);
        
        var serviceTypes = viewModelsAssembly.GetExportedTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith(options.ViewModelPattern))
            .ToList();

        
        var repositoryInterface = repositoryAssembly.GetType(options.RepositoryName) ?? repositoryAssembly.GetExportedTypes().FirstOrDefault(t => t.Name == options.RepositoryName);
        if (repositoryInterface == null)
            throw new Exception($"{options.RepositoryName} not found.");
        
        foreach (var dtoType in serviceTypes)
        {
            var entityName = dtoType.Name.Replace(options.ViewModelPattern, "");
            var entityType = domainAssembly.GetExportedTypes().FirstOrDefault(t => t.Name == entityName);
            
            //Entity isn't if exist to go to next entity 
            if (entityType is null)
                continue;
            
            var baseServiceType = repositoryInterface;

            string routePrefix = entityName;
            
            //GET: GetList
            var getAllRoute = endpoints.MapGet(routePrefix + "/GetAllListAsync", async (HttpContext ctx) =>
                {
                    var service = ctx.RequestServices.GetRequiredService(baseServiceType);
                    var method = baseServiceType.GetMethod(options.GetListService);
                    var task = (Task)method.Invoke(service, null);
                    await task.ConfigureAwait(false);
                    return task.GetType().GetProperty(options.ApiResulPropertyName)?.GetValue(task);
                })
                .WithTags(routePrefix);

            if (options.IsAuthenticateRequired)
                getAllRoute.RequireAuthorization(options.AuthorizationPolicy);
            
            
            var getRoute = endpoints.MapGet(routePrefix + "/GetAsync/{{Id}}", async (Guid Id,HttpContext ctx) =>
                {
                    var service = ctx.RequestServices.GetRequiredService(baseServiceType);
                    var method = baseServiceType.GetMethod(options.GetByIdService);
                    var task = (Task)method.Invoke(service, new object[] { Id });
                    await task.ConfigureAwait(false);
                    return task.GetType().GetProperty(options.ApiResulPropertyName)?.GetValue(task);
                })
                .WithTags(routePrefix);

            if (options.IsAuthenticateRequired)
                getRoute.RequireAuthorization(options.AuthorizationPolicy);
            
            var createRoute = endpoints.MapPost(routePrefix + "/AddAsync", async (HttpContext ctx) =>
                {
                    var service = ctx.RequestServices.GetRequiredService(baseServiceType);
                    var method = baseServiceType.GetMethod(options.CreateService);
                    
                    var dto = await ctx.Request.ReadFromJsonAsync(dtoType);
                    var task = (Task)method.Invoke(service, new object[] { dto });
                    await task.ConfigureAwait(false);
                    return task.GetType().GetProperty(options.ApiResulPropertyName)?.GetValue(task);
                })
                .WithTags(routePrefix);

            if (options.IsAuthenticateRequired)
                createRoute.RequireAuthorization(options.AuthorizationPolicy);
            
            var updateRoute = endpoints.MapPut(routePrefix + "/UpdateAsync", async (HttpContext ctx) =>
                {
                    var service = ctx.RequestServices.GetRequiredService(baseServiceType);
                    var method = baseServiceType.GetMethod(options.CreateService);
                    var dto = await ctx.Request.ReadFromJsonAsync(dtoType);
                    var task = (Task)method.Invoke(service, new object[] { dto });
                    await task.ConfigureAwait(false);
                    return task.GetType().GetProperty(options.ApiResulPropertyName)?.GetValue(task);
                })
                .WithTags(routePrefix);

            if (options.IsAuthenticateRequired)
                updateRoute.RequireAuthorization(options.AuthorizationPolicy);
            
            
            var deleteRoute = endpoints.MapDelete(routePrefix + "/DeleteAsync/{{Id}}", async (Guid Id,HttpContext ctx) =>
                {
                    var service = ctx.RequestServices.GetRequiredService(baseServiceType);
                    var method = baseServiceType.GetMethod(options.DeleteService);
                    var task = (Task)method.Invoke(service, new object[] { Id });
                    await task.ConfigureAwait(false);
                    return Results.Ok();
                })
                .WithTags(routePrefix);

            if (options.IsAuthenticateRequired)
                deleteRoute.RequireAuthorization(options.AuthorizationPolicy);
            
        }
        
        return endpoints;
    }
    
    /// <summary>
    /// Configuration for CRUD operation
    /// </summary>
    public class CrudOptions
    {
        /// <summary>
        /// Your project in domain layer name.
        /// If you enter the name of the domain layer here, it will retrieve the classes marked with BaseEntity or anything within that layer and generate minimal APIs specifically for CRUD operations related to those domain entities.
        /// </summary>
        public string DomainLayerName { get; set; }

        /// <summary>
        /// The layer where the view models are located
        /// </summary>
        public string ViewModelsLayerName { get; set; }

        /// <summary>
        /// The layer where the repository are located
        /// </summary>
        public string RepositoryLayerName { get; set; }
        
        /// <summary>
        /// Your repository name
        /// Ex: IRepository, IRepositoryService, IGenericService, etc...
        /// </summary>
        public string RepositoryName { get; set; }

        /// <summary>
        /// Is authorization mandatory on a controller?
        /// </summary>
        public bool IsAuthenticateRequired { get; set; }
        /// <summary>
        /// Your view model of entity name pattern
        /// Ex: Product*ViewModel*, Product*VM*, Product*DTO* etc...
        /// </summary>
        public string ViewModelPattern { get; set; }
        
        /// <summary>
        /// Admin, User, SuperAdmin etc...
        /// </summary>
        public string? AuthorizationPolicy { get; set; }

        
        /// <summary>
        /// GetListService name of your repository in get list method name
        /// </summary>
        public string GetListService { get; set; }
        public string GetByIdService { get; set; }
        public string CreateService { get; set; }
        public string UpdateService { get; set; }
        public string DeleteService { get; set; }
        
        
        /// <summary>
        /// View model name of common API base response model
        /// Ex:
        /// public class Result(Class name doesn't matter) -> This
        /// {
        ///     public bool Status { get; set; }
        ///     public string Message { get; set; }
        ///     public T Data { get; set; }
        /// }
        /// </summary>
        public string ApiResulPropertyName { get; set; }
    }
}