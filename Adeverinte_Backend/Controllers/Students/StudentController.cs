using Adeverinte_Backend.Entities.Students;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Adeverinte_Backend.Controllers.Students;

[Route("Students")]
[ApiController]
public class StudentController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    
    public StudentController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentResponse>>> GetStudents()
    {
        var students = await _dbContext.Students.ToListAsync();

        return Ok(students.Select(Map));
    }
    
    private StudentResponse Map(StudentModel student)
    {
        return new StudentResponse(student.Id, student.Email, student.FirstName,
            student.LastName, student.Faculty, student.Speciality,
            student.Year, student.Role);
    }
}