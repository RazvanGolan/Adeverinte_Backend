using Adeverinte_Backend.Entities.Certificates;
using Adeverinte_Backend.Entities.Students;
using Microsoft.EntityFrameworkCore;

namespace Adeverinte_Backend;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
        
    }
    
    public DbSet<CertificateModel> Certificates { get; set; }
    public DbSet<StudentModel> Students { get; set; }
}