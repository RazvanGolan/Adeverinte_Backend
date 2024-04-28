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
        var student = await _appDbContext.Students
            .Include(s=>s.Speciality)
            .Include(s=>s.Faculty)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (student is null)
            throw new Exception($"The student with id {id} does not exist.");

        return student;
    }

    public async Task<List<StudentModel>> GetAll()
    {
        return await _appDbContext.Students
            .Include(s=>s.Speciality)
            .Include(s=>s.Faculty)
            .ToListAsync();
    }

    public async Task Save()
    {
        await _appDbContext.SaveChangesAsync();
    }

    public async Task<StudentModel> CreateStudent(StudentRequest request)
    {
        var faculty = await _appDbContext.Faculties
            .FirstOrDefaultAsync(f => f.Id == request.FacultyId);
        var speciality = await _appDbContext.Specialities
            .FirstOrDefaultAsync(s => s.Id == request.SpecialityId);

        if (faculty is null)
            throw new Exception($"Faculty with id {request.FacultyId} does not exist");
        if (speciality is null)
            throw new Exception($"Speciality with id {request.SpecialityId} does not exist");
        
        var student = new StudentModel(request.FirstName, request.LastName, faculty, speciality, request.Email,
            request.Role, request.Year, request.Marca);

        _appDbContext.Students.Add(student);
        return student;
    }
}