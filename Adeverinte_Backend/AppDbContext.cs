using Adeverinte_Backend.Entities;
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
    public DbSet<FacultyModel> Faculties { get; set; }
    public DbSet<SpecialityModel> Specialities { get; set; }
    
}