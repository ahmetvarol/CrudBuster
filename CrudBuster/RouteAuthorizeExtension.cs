using Microsoft.AspNetCore.Builder;

namespace CrudBuster;

public static class RouteAuthorizeExtension
{
    public static void ApplyAuthorization(this RouteHandlerBuilder route, CrudOptions options)
    {
        if (options.IsAuthenticateRequired)
        {
            if (string.IsNullOrEmpty(options.AuthorizationPolicy))
            {
                route.RequireAuthorization();
            }
            else
            {
                route.RequireAuthorization(options.AuthorizationPolicy);
            }
        }
     
    }
}