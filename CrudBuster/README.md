# CrudBuster
An automatic CRUD endpoint generator for ASP.NET Core Minimal APIs that scans your project's entities and automatically generates corresponding CRUD (Create, Read, Update, Delete) service endpoints based on those entities, streamlining API development and reducing boilerplate code."

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


# Changelog
## [1.0.9] - 2025-07-29
### Added
- The error in the PropertyType.Name value has been resolved.
- Critical bug fixed

## [1.0.8] - 2025-07-28
### Added
- Delegate-based architecture was employed for CRUD services, enabling dynamic method binding and enhancing execution efficiency.
- Core infrastructure has been refactored to support modularity and performance.

## [1.0.7] - 2025-07-28
- The view model folder structure is created based on the dynamic entity name.
- Performance improvements have been made.
- If any of the view models (CreateViewModel, UpdateViewModel, DeleteViewModel, GetViewModel, GetListViewModel) are missing, the system will automatically generate the missing ones. If the entity has no view models at all, it will generate all of them.
- Critical bug fixed




## Usage
```csharp
app.CrudBuster(opt => opt
    //!!! It must be the same as the method name in the repository.
    .WithGetByIdService("GetAsync")

    //!!! It must be the same as the method name in the repository.
    .WithGetListService("GetListAsync")

    //!!! It must be the same as the method name in the repository.
    .WithCreateService("CreateAsync")

    //!!! It must be the same as the method name in the repository.
    .WithUpdateService("UpdateAsync")

    //!!! It must be the same as the method name in the repository.
    .WithDeleteService("DeleteAsync")
    
    //It searches for entities in this layer.
    .WithDomainLayer("DOmain")
    
    //It searches for view models in this layer.
    .WithViewModelLayer("Application")
    
    //It searches for repo in this layer.
    .WithRepositoryLayer("Infrastructure")
    
    //In this field, you must assign the last part of the name you used in your view model classes. For example: ProductCreate*ViewModel*, ProductCreate*VM*, ProductCreate*DTO*, or whatever naming convention you follow.
    .WithViewModelPattern("VM")

    //You must provide your own view model path."
    .WithViewModelOutput(Directory.GetCurrentDirectory()+"/ViewModels")
    
    // !!!The database tables must have the same name as the base entity class they inherit from.
    .WithBaseEntityName("IBaseEntity")
    
    //If required authenticate
    .WithIsAuthenticateRequired(false)

     //This field specifies which permissions can access the controller. You can set it to null
    .WithAuthorizationPolicy("Admin, User, SuperAdmin")

    //!!! It has to be the same as the name of the repository.".
    .WithRepositoryName("IRepository"));

```


## Donate
Binance
- **BTC**: 13mzeg11nGAHwx5WtHye5dpNTQxbsb1T2v

- **ETH**: 0xadcdb8c83207821f86e9ff683cc74fa45e48ca30

- **SOL**: EdQCWcWmvsEJq9PxnVegviJipcExZUUgEz2ZZTBbhQVa