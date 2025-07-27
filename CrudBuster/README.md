# CrudBuster
Automatic CRUD Endpoint Generator for ASP.NET Core Minimal APIs (Beta)

**CrudBuster**, allows you to quickly and flexibly generate CRUD (Create, Read, Update, Delete) endpoints in your ASP.NET Core applications. You can define your endpoints without relying on any interface by integrating your own service classes through delegates.

## Features
- **Automatic CRUD Endpoint Generation**: Create GET, POST, PUT, and DELETE endpoints with a single method.
- **Flexible Service Integration**: Works with any service class thanks to its delegate-based structure.
- **Authorization Support**: You can add custom authorization policies to your endpoints.
- **Minimal API Support**: Optimized for .NET 6 and later.
- **Beta Release**: Currently in beta — we welcome your feedback!

## Installation
Add the NuGet package to your project:

## Feedback
Thank you for testing our beta version! You can share your issues or suggestions via GitHub Issues.


## Usage
```csharp
//ViewModels()
public record ProductCreateViewModel(Guid Id, string Name);
public record ProductUpdateViewModel(Guid Id, string Name);
public record ProductDeleteViewModel(Guid Id, string Name);
public record ProductGetViewModel(Guid Id, string Name);
public record ProductListViewModel(Guid Id, string Name);

...
    
public interface IRepository
{
    Task GetListAsync();
    Task GetByIdAsync();
    Task CreateAsync();
    Task UpdateAsync();
    Task DeleteAsync();
}

public async Task<Result<List<TListViewModel>>> GetListAsync()
{
    var response = new Result<List<TListViewModel>>();
    var data = await _repository.GetAllAsync(x=>x.Status == Status.Active);

    if (data is not null)
        response.Data = _mapper.Map<List<TListViewModel>>(data);
    else
        response.Message = "Something went wrong";

    return response;
}
... 
    
    public class Result<T>
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
    
...

     app.CrudBuster(options =>
{
    //It searches for entities in this layer.
    options.DomainLayerName = "Domain";
    
    //It searches for view models in this layer.
    options.ViewModelsLayerName = "Application";
    
    //It searches for repo in this layer.
    options.RepositoryLayerName = "Infrastructure";

    //In this field, you must assign the last part of the name you used in your view model classes. For example: ProductCreateViewModel, ProductCreateVM, ProductCreateDTO, or whatever naming convention you follow.
    options.ViewModelPattern = "ViewModel or DTO or VM etc...";
    
    options.AuthorizationPolicy = "Admin or NULL"; 
    
    options.RepositoryName = "IRepository"; //!!! It has to be the same as the name of the repository.".
    
    options.GetListService = "GetListAsync"; //!!! It must be the same as the method name in the repository.
    options.GetByIdService = "GetByIdAsync"; //!!! It must be the same as the method name in the repository.
    options.DeleteService = "DeleteAsync";   //!!! It must be the same as the method name in the repository.
    options.CreateService = "CreateAsync";   //!!! It must be the same as the method name in the repository.
    options.UpdateService = "UpdateAsync";   //!!! It must be the same as the method name in the repository.
    
    //Where will the obtained data be assigned? You should provide the name of the base response field in this area.
    options.ApiResulPropertyName = "Result";
    
    
    options.BaseEntityName="IEntity"; // !!!The database tables must have the same name as the base entity class they inherit from.
});
```

## Donate
Binance
- **BTC**: 13mzeg11nGAHwx5WtHye5dpNTQxbsb1T2v

- **ETH**: 0xadcdb8c83207821f86e9ff683cc74fa45e48ca30

- **SOL**: EdQCWcWmvsEJq9PxnVegviJipcExZUUgEz2ZZTBbhQVa

```bash
dotnet add package CrudBuster --version 1.0.0-beta1 --prerelease