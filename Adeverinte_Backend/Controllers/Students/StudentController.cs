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
    private readonly IStudentServices _studentServices;
    
    public StudentController(AppDbContext dbContext, IStudentServices studentServices)
    {
        _dbContext = dbContext;
        _studentServices = studentServices;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentResponse>>> GetStudents()
    {
        var students = await _studentServices.GetAll();

        return Ok(students.Select(Map));
    }

    [HttpGet("id")]
    public async Task<ActionResult<StudentResponse>> GetStudentById(string id)
    {
        var student = await _studentServices.FindById(id);
        
        if (student != null)
        {
            return Ok(Map(student));
        }

        return NotFound($"Student-ul cu id-ul {id} nu exista.");
    }
    
    [HttpPost]
    public async Task<ActionResult<StudentModel>> CreateStudent([FromBody] StudentRequest request)
    {
        
        var student = await _studentServices.CreateStudent(request);
        
        await _studentServices.Save();

        return Ok(Map(student));
    }
    
    [HttpDelete("id")]
    public async Task<ActionResult<StudentResponse>> DeleteStudent(string id)
    {
        var student = await _studentServices.FindById(id);

        if (student is null)
            return NotFound($"Student-ul cu id-ul {id} nu exista.");

        _dbContext.Remove(student);
        await _studentServices.Save();

        return Ok(Map(student));
    }
    
    private StudentResponse Map(StudentModel student)
    {
        return new StudentResponse(student.Id, student.Email, student.FirstName,
            student.LastName, student.Faculty, student.Speciality,
            student.Year, student.Role);
    }
}