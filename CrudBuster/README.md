# CrudBuster
Automatic CRUD Endpoint Generator for ASP.NET Core Minimal APIs (Beta)

**CrudBuster**, allows you to quickly and flexibly generate CRUD (Create, Read, Update, Delete) endpoints in your ASP.NET Core applications. You can define your endpoints without relying on any interface by integrating your own service classes through delegates.

## Features
- **Automatic CRUD Endpoint Generation: Create GET, POST, PUT, and DELETE endpoints with a single method.
- **Flexible Service Integration: Works with any service class thanks to its delegate-based structure.
- **Authorization Support: You can add custom authorization policies to your endpoints.
- **Minimal API Support**: Optimized for .NET 6 and later.
- **Beta Release**: Currently in beta — we welcome your feedback! At this stage, it only supports generating listing (GET) endpoints. Support for other CRUD operations will be added in future releases.

## Installation
Add the NuGet package to your project:

## Feedback
Thank you for testing our beta version! You can share your issues or suggestions via GitHub Issues.

```bash
dotnet add package CrudBuster --version 1.0.0-beta1 --prerelease