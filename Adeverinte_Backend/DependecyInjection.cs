using Microsoft.EntityFrameworkCore;

namespace Adeverinte_Backend;

public static class DependecyInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("ConnectionString"));
        });
        
        return services;
    }
}