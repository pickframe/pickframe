using Database.Repositories;
using Domain.Entities.Process;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Database;

[ExcludeFromCodeCoverage]
public static class DependenceInjection
{
    public static IServiceCollection AddInfraData(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DatabaseContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("Default");
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IProcessRepository, ProcessRepository>();

        return services;
    }
}
