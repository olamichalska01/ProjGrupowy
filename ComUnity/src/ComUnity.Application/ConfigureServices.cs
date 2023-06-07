using ComUnity.Application.Common;
using ComUnity.Application.Common.Behaviours;
using ComUnity.Application.Database;
using ComUnity.Application.Features.Authentication.Settings;
using ComUnity.Application.Infrastructure.Services;
using ComUnity.Application.Infrastructure.Settings;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ComUnity.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddDbContext<ComUnityContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ComUnityContext).Assembly.FullName)
                    .UseNetTopologySuite()));

        services.Configure<AzureStorageSettings>(configuration.GetSection("AzureStorageSettings"));
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
        services.Configure<Links>(configuration.GetSection("Links"));

        services.AddScoped<IAzureStorageService, AzureStorageService>();
        services.AddScoped<IEmailSenderService, EmailSenderService>();
        services.AddScoped<IAuthenticatedUserProvider, AuthenticatedUserProvider>();
        services.AddScoped<IDomainEventService, DomainEventService>();

        return services;
    }
}
