using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace CrudBuster;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

public static class CrudExtensions
{
    public static IEndpointRouteBuilder MapCrud<TRepo>(
        this IEndpointRouteBuilder endpoints, 
        string routePrefix, 
        Action<CrudOptions>? configure = null)
    where TRepo : class
    {
        
        var options = new CrudOptions();
        configure?.Invoke(options);
        
        var baseServiceType = typeof(TRepo).MakeGenericType();

        
        //GET: Listeleme
        endpoints.MapGet(routePrefix, async (HttpContext ctx) =>
        {
            var service = ctx.RequestServices.GetRequiredService(baseServiceType);
            var method = baseServiceType.GetMethod(options.GetListService);
            var task = (Task)method.Invoke(service, null);
            await task.ConfigureAwait(false);
            return task.GetType().GetProperty(options.ApiResulPropertyName)?.GetValue(task);
        })
        .WithTags(routePrefix)
        .RequireAuthorization(options.AuthorizationPolicy);
        
        
        
        return endpoints;
    }
    
    public class CrudOptions
    {
        public string? AuthorizationPolicy { get; set; }

        /// <summary>
        /// GetService name of Repository
        /// </summary>
        public string GetService { get; set; }
        public string GetListService { get; set; }
        public string CreateService { get; set; }
        public string UpdateService { get; set; }
        public string DeleteService { get; set; }
        public string ApiResulPropertyName { get; set; }
    }
}