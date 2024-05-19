using System.Text.RegularExpressions;
using Adeverinte_Backend.Controllers.Students;
using Adeverinte_Backend.Entities.Students;
using Microsoft.EntityFrameworkCore;

namespace Adeverinte_Backend.Services;

public class StudentService : IStudentService
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

    public async Task<StudentModel> FindByEmail(string email)
    {
        var student = await _appDbContext.Students
            .Include(s=>s.Speciality)
            .Include(s=>s.Faculty)
            .FirstOrDefaultAsync(s => s.Email == email);
        
        if(student is null)
            throw new Exception($"The student with email {email} does not exist.");

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

        if (!Regex.IsMatch(request.Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}(?:\.[a-zA-Z]{2,})?$"))
            throw new Exception($"The email {request.Email} is not a valid email");
        
        var student = new StudentModel(request.FirstName, request.LastName, faculty, speciality, request.Email,
            request.Role, request.Year, request.Marca);

        _appDbContext.Students.Add(student);
        return student;
    }
}