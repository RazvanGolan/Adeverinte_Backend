using Adeverinte_Backend.Entities.Students;
using Adeverinte_Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Adeverinte_Backend.Controllers.Students;

[Route("Students")]
[ApiController]
public class StudentController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IStudentService _studentService;
    
    public StudentController(AppDbContext dbContext, IStudentService studentService)
    {
        _dbContext = dbContext;
        _studentService = studentService;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentResponse>>> GetStudents()
    {
        var students = await _studentService.GetAll();

        return Ok(students.Select(Map));
    }

    [HttpGet("id")]
    public async Task<ActionResult<StudentResponse>> GetStudentById(string id)
    {
        try
        {
            var student = await _studentService.FindById(id);
            return Ok(Map(student));
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }
    
    [HttpGet("email")]
    public async Task<ActionResult<StudentResponse>> GetStudentByEmail(string email)
    {
        try
        {
            var student = await _studentService.FindByEmail(email);
            return Ok(Map(student));
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }
    
    [HttpGet("/Faculties")]
    public async Task<ActionResult<IEnumerable<FacultyResponse>>> GetAllFaculties()
    {
        var faculties = await _dbContext.Faculties
            .Include(s => s.Specialities)
            .ToListAsync();

        var facultyResponses = faculties.Select(f => new FacultyResponse
        {
            Id = f.Id,
            Name = f.Name,
            Specialities = f.Specialities.Select(s => new SpecialityResponse
            {
                Name = s.Name,
                Id = s.Id,
                Faculty = f.Name
            }).ToList()
        }).ToList();

        return Ok(facultyResponses);
    }

    [HttpGet("/Specialities")]
    public async Task<ActionResult<IEnumerable<SpecialityResponse>>> GetAllSpecialties()
    {
        var specialties = await _dbContext.Specialities
            .Include(f => f.Faculty)
            .ToListAsync();

        var specialtyResponses = specialties.Select(s => new SpecialityResponse
        {
            Name = s.Name,
            Id = s.Id,
            Faculty = s.Faculty.Name
        }).ToList();
    
        return Ok(specialtyResponses);
    }
    
    [HttpPost]
    public async Task<ActionResult<StudentModel>> CreateStudent([FromBody] StudentRequest request)
    {
        try
        {
            var student = await _studentService.CreateStudent(request);
        
            await _studentService.Save();

            return Ok(Map(student));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpDelete("id")]
    public async Task<ActionResult> DeleteStudent(string id)
    {
        try
        {
            var student = await _studentService.FindById(id);
            _dbContext.Remove(student);
            await _studentService.Save();
            
            return Ok($"The student with id {id} was removed");
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }
    
    private StudentResponse Map(StudentModel student)
    {
        return new StudentResponse(student.Id, student.Email, student.FirstName,
            student.LastName, student.Faculty.Name, student.Speciality.Name,
            student.Year, student.Role);
    }
}