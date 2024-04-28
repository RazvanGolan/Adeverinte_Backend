using Adeverinte_Backend.Controllers.Students;
using Adeverinte_Backend.Entities.Students;
using Microsoft.EntityFrameworkCore;

namespace Adeverinte_Backend.Services;

public class StudentService : IStudentServices
{
    protected readonly AppDbContext _appDbContext;
    
    public StudentService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }
    
    public async Task<StudentModel> FindById(string id)
    {
        return await _appDbContext.Students.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<StudentModel>> GetAll()
    {
        return await _appDbContext.Students
            .ToListAsync();
    }

    public async Task Save()
    {
        await _appDbContext.SaveChangesAsync();
    }

    public async Task<StudentModel> CreateStudent(StudentRequest request)
    {
        var student = new StudentModel(request.FirstName, request.LastName, request.Faculty, request.Speciality, request.Email,
            request.Role, request.Year, request.Marca);

        _appDbContext.Students.Add(student);
        return student;
    }
}