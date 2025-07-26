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

        var entityInterface = domainAssembly.GetExportedTypes().FirstOrDefault(t => t.IsInterface && t.Name == options.BaseEntityName);
        
        var entities = domainAssembly.GetExportedTypes().Where(t => t.IsClass && !t.IsAbstract && entityInterface.IsAssignableFrom(t)).ToList();
        if(!entities.Any())
            throw new Exception($"Entities not found in assembly {options.DomainLayerName}");
        
        
        var repositoryInterface = repositoryAssembly.GetType(options.RepositoryName) ?? repositoryAssembly.GetExportedTypes().FirstOrDefault(t => t.Name == options.RepositoryName);
      
        if (repositoryInterface == null)
            throw new Exception($"{options.RepositoryName} not found.");
        

        foreach (var entity in entities)
        {
            
            var viewModels = viewModelsAssembly.GetExportedTypes().Where(t => t.IsClass && !t.IsAbstract && t.Name.StartsWith(entity.Name) && t.Name.EndsWith(options.ViewModelPattern)).ToList();

            if (!viewModels.Any(x => x.Name == $"{entity.Name}Create{options.ViewModelPattern}"))
                throw new Exception($"The view model for the create {entity.Name} was not found ");
            if (!viewModels.Any(x => x.Name == $"{entity.Name}Update{options.ViewModelPattern}"))
                throw new Exception($"The view model for the update {entity.Name} was not found ");
            if (!viewModels.Any(x => x.Name == $"{entity.Name}Delete{options.ViewModelPattern}"))
                throw new Exception($"The view model for the delete {entity.Name} was not found ");
            
            string routePrefix = entity.Name;
            var baseServiceType = repositoryInterface;
            
            var createViewModel = viewModels.First(x=>x.Name.EndsWith("Create"+options.ViewModelPattern));
            var updateViewModel = viewModels.First(x=>x.Name.EndsWith("Update"+options.ViewModelPattern));
            var deleteViewModel = viewModels.First(x=>x.Name.EndsWith("Delete"+options.ViewModelPattern));
                
                
            var createRoute = endpoints.MapPost(routePrefix + "/AddAsync", async (HttpContext ctx) =>
                {
                    var service = ctx.RequestServices.GetRequiredService(baseServiceType);
                    var method = baseServiceType.GetMethod(options.CreateService);
                    var dto = await ctx.Request.ReadFromJsonAsync(createViewModel);
                    var task = (Task)method.Invoke(service, new object[] { dto });
                    await task.ConfigureAwait(false);
                    return task.GetType().GetProperty(options.ApiResulPropertyName)?.GetValue(task);
                })
                .Accepts(createViewModel, "application/json")
                .WithTags(routePrefix);
            
            var updateRoute = endpoints.MapPut(routePrefix + "/UpdateAsync", async (HttpContext ctx) =>
                {
                    var service = ctx.RequestServices.GetRequiredService(baseServiceType);
                    var method = baseServiceType.GetMethod(options.CreateService);
                    var dto = await ctx.Request.ReadFromJsonAsync(updateViewModel);
                    var task = (Task)method.Invoke(service, new object[] { dto });
                    await task.ConfigureAwait(false);
                    return task.GetType().GetProperty(options.ApiResulPropertyName)?.GetValue(task);
                })
                .Accepts(updateViewModel, "application/json")
                .WithTags(routePrefix);
            
            var deleteRoute = endpoints.MapDelete(routePrefix + "/DeleteAsync", async (Guid Id,HttpContext ctx) =>
                {
                    var service = ctx.RequestServices.GetRequiredService(baseServiceType);
                    var method = baseServiceType.GetMethod(options.DeleteService);
                    var dto = await ctx.Request.ReadFromJsonAsync(deleteViewModel);
                    var task = (Task)method.Invoke(service, new object[] { dto });
                    await task.ConfigureAwait(false);
                    return Results.Ok();
                })
                .Accepts(deleteViewModel, "application/json")
                .WithTags(routePrefix);
            
            var getAllRoute = endpoints.MapGet(routePrefix + "/GetListAsync", async (HttpContext ctx) =>
                {
                    var service = ctx.RequestServices.GetRequiredService(baseServiceType);
                    var method = baseServiceType.GetMethod(options.GetListService);
                    var task = (Task)method.Invoke(service, null);
                    await task.ConfigureAwait(false);
                    return task.GetType().GetProperty(options.ApiResulPropertyName)?.GetValue(task);
                })
                .WithTags(routePrefix);

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
            {
                getAllRoute.RequireAuthorization(options.AuthorizationPolicy);
                getRoute.RequireAuthorization(options.AuthorizationPolicy);
                createRoute.RequireAuthorization(options.AuthorizationPolicy);
                updateRoute.RequireAuthorization(options.AuthorizationPolicy);
                deleteRoute.RequireAuthorization(options.AuthorizationPolicy);
            }
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
        /// Your base entity name. It's used to retrieve the classes that inherit from this class.
        /// </summary>
        public string BaseEntityName { get; set; }
        
        /// <summary>
        /// It is used to get the classes that inherit from this class.
        /// </summary>
        public string GetListService { get; set; }
        
        /// <summary>
        /// It is used to get the classes that inherit from this class.
        /// </summary>
        public string GetByIdService { get; set; }
        
        /// <summary>
        /// It is used to get the classes that inherit from this class.
        /// </summary>
        public string CreateService { get; set; }
        
        /// <summary>
        /// It is used to get the classes that inherit from this class.
        /// </summary>
        public string UpdateService { get; set; }
        
        /// <summary>
        /// It is used to get the classes that inherit from this class.
        /// </summary>
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