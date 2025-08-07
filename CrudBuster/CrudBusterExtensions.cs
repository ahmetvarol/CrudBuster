using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

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
        
        
        var repositoryInterface = repositoryAssembly.GetType(options.RepositoryName)
                                  ?? repositoryAssembly.GetExportedTypes()
                                      .FirstOrDefault(t =>
                                          t.Name == options.RepositoryName || 
                                          (t.IsGenericTypeDefinition && t.Name.StartsWith(options.RepositoryName + "`")));

        
        if (repositoryInterface == null)
            throw new Exception($"{options.RepositoryName} not found.");
        
        var sourceCodes = new List<string>();
        foreach (var entity in entities)
        {
            string routePrefix = entity.Name;
            var baseServiceType = repositoryInterface;
            
            var viewModels = viewModelsAssembly.GetExportedTypes().Where(t => t.IsClass && !t.IsAbstract && t.Name.StartsWith(entity.Name) && t.Name.EndsWith(options.ViewModelPattern)).ToList();
           
            if (!string.IsNullOrEmpty(options.ViewModelPattern))
            {
                if (!viewModels.Any(x => x.Name == $"{entity.Name}Create{options.ViewModelPattern}"))
                {
                    bool status = CrudBusterViewModelGenerator.GenerateDtoFromEntity(entity,$"{entity.Name}Create{options.ViewModelPattern}",options.ViewModelOutputPath,entity.Name,options.ViewModelPattern,"Create",options.DomainLayerName);
                    if(status)
                        sourceCodes.Add($"{options.ViewModelOutputPath}/{entity.Name}Create{options.ViewModelPattern}.cs");
                }

                if (!viewModels.Any(x => x.Name == $"{entity.Name}Update{options.ViewModelPattern}"))
                {
                    bool status =  CrudBusterViewModelGenerator.GenerateDtoFromEntity(entity,$"{entity.Name}Update{options.ViewModelPattern}",options.ViewModelOutputPath,entity.Name,options.ViewModelPattern,"Update",options.DomainLayerName);
                    if (status)
                        sourceCodes.Add($"{options.ViewModelOutputPath}/{entity.Name}Update{options.ViewModelPattern}.cs");
                }

                if (!viewModels.Any(x => x.Name == $"{entity.Name}Delete{options.ViewModelPattern}"))
                {
                    bool status = CrudBusterViewModelGenerator.GenerateDtoFromEntity(entity,$"{entity.Name}Delete{options.ViewModelPattern}",options.ViewModelOutputPath,entity.Name,options.ViewModelPattern,"Delete",options.DomainLayerName);
                    if (status)
                        sourceCodes.Add($"{options.ViewModelOutputPath}/{entity.Name}Delete{options.ViewModelPattern}.cs");
                }

                if (!viewModels.Any(x => x.Name == $"{entity.Name}Get{options.ViewModelPattern}"))
                {
                    bool status = CrudBusterViewModelGenerator.GenerateDtoFromEntity(entity,$"{entity.Name}Get{options.ViewModelPattern}",options.ViewModelOutputPath,entity.Name,options.ViewModelPattern,"Get",options.DomainLayerName);
                    if (status)
                        sourceCodes.Add($"{options.ViewModelOutputPath}/{entity.Name}Get{options.ViewModelPattern}.cs");
                }

                if (!viewModels.Any(x => x.Name == $"{entity.Name}List{options.ViewModelPattern}"))
                {
                    bool status = CrudBusterViewModelGenerator.GenerateDtoFromEntity(entity,$"{entity.Name}List{options.ViewModelPattern}",options.ViewModelOutputPath,entity.Name,options.ViewModelPattern,"List",options.DomainLayerName);
                    if (status)
                        sourceCodes.Add($"{options.ViewModelOutputPath}/{entity.Name}List{options.ViewModelPattern}.cs");
                }

                if (sourceCodes.Any())
                {
                    var assembly = MemoryCompile.CompileToAssembly(options.ViewModelOutputPath);
                    viewModels = assembly.GetExportedTypes().Where(t => t.IsClass && !t.IsAbstract && t.Name.StartsWith(entity.Name) && t.Name.EndsWith(options.ViewModelPattern)).ToList();
                }
                
            }

            if (options.ProccessType == ProccessType.MinimalAPI)
            {
                 var createViewModel = viewModels.First(x=>x.Name.EndsWith("Create"+options.ViewModelPattern));
                    var updateViewModel = viewModels.First(x=>x.Name.EndsWith("Update"+options.ViewModelPattern));
                    var deleteViewModel = viewModels.First(x=>x.Name.EndsWith("Delete"+options.ViewModelPattern));
                        
                        
                    CrudDelegateCache.SetMethods(options);
                    
                    var createRoute = endpoints.MapPost(routePrefix + "/CreateAsync", async (HttpContext ctx) =>
                        {
                            var service = ctx.RequestServices.GetRequiredService(baseServiceType);
                            var dto = await ctx.Request.ReadFromJsonAsync(createViewModel);
                            var serviceDelegate = CrudDelegateCache.CreateDelegate(baseServiceType);
                            await serviceDelegate(service, dto);
                            return Results.Ok();
                        })
                        .Accepts(createViewModel, "application/json")
                        .WithTags(routePrefix);
                    
                    var updateRoute = endpoints.MapPut(routePrefix + "/UpdateAsync", async (HttpContext ctx) =>
                        {
                            var service = ctx.RequestServices.GetRequiredService(baseServiceType);
                            var dto = await ctx.Request.ReadFromJsonAsync(updateViewModel);
                            var serviceDelegate = CrudDelegateCache.UpdateDelegate(baseServiceType);
                            await serviceDelegate(service, dto);
                            return Results.Ok();
                        })
                        .Accepts(updateViewModel, "application/json")
                        .WithTags(routePrefix);
                    
                    var deleteRoute = endpoints.MapDelete(routePrefix + "/DeleteAsync", async (HttpContext ctx) =>
                        {
                            var service = ctx.RequestServices.GetRequiredService(baseServiceType);
                            var dto = await ctx.Request.ReadFromJsonAsync(deleteViewModel);
                            var serviceDelegate = CrudDelegateCache.DeleteDelegate(baseServiceType);
                            await serviceDelegate(service, dto);
                            return Results.Ok();
                         
                        })
                        .Accepts(deleteViewModel, "application/json")
                        .WithTags(routePrefix);
                    
                    var getAllRoute = endpoints.MapGet(routePrefix + "/GetListAsync", async (HttpContext ctx) =>
                        {
                            var service = ctx.RequestServices.GetRequiredService(baseServiceType);
                            var serviceDelegate = CrudDelegateCache.GetListDelegate(baseServiceType);
                            var result = await serviceDelegate(service);
                            return Results.Ok(result);
                        })
                        .WithTags(routePrefix);

                    var getRoute = endpoints.MapGet(routePrefix + "/GetAsync/{{Id}}", async (Guid Id,HttpContext ctx) =>
                        {
                            var service = ctx.RequestServices.GetRequiredService(baseServiceType);
                            var serviceDelegate = CrudDelegateCache.GetByIdDelegate(baseServiceType);
                            var result = await serviceDelegate(service,Id);
                            return Results.Ok(result);
                        })
                        .WithTags(routePrefix);
                    
                    
                    getAllRoute.ApplyAuthorization(options);
                    getRoute.ApplyAuthorization(options);
                    createRoute.ApplyAuthorization(options);
                    updateRoute.ApplyAuthorization(options);
                    deleteRoute.ApplyAuthorization(options);
            }
            else if (options.ProccessType == ProccessType.Controller)
            {
                
            }
           
        }
        
        
        return endpoints;
    }
    
   
}