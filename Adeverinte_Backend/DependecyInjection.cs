using Adeverinte_Backend.Services;
using Adeverinte_Backend.Services.Email;
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

        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<ICertificateService, CertificateService>();
        services.AddScoped<IPdfService, PdfService>();
        services.AddScoped<IEmailService, EmailService>();
        
        services.Configure<EmailConfig>(configuration.GetSection("EmailConfig"));
        return services;
    }
}