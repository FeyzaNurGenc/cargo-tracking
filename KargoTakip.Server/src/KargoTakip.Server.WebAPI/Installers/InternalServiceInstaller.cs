
using KargoTakip.Server.Application;
using KargoTakip.Server.Infrastructure;

namespace KargoTakip.Server.WebAPI.Installers;

public static class InternalServiceInstaller
{
    public static IServiceCollection AddInternalServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplication();
        services.AddInfrastructure(configuration);
        services.AddCors();
        services.AddExceptionHandler<ExceptionHandler>().AddProblemDetails();
        //services.AddKeycloakWebApiAuthentication(builder.Configuration);
        //services.AddAuthorization().AddKeycloakAuthorization(builder.Configuration);
        return services;
    }
}