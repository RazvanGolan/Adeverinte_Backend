using Adeverinte_Backend.Entities.Enums;

namespace Adeverinte_Backend.Controllers.Students;

public class StudentResponse
{
    public StudentResponse(string id, string email, string firstName, string lastName, string faculty, string speciality, int year, RoleEnum role)
    {
        Id = id;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        Faculty = faculty;
        Speciality = speciality;
        Year = year;
        Role = role;
    }

    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Faculty { get; set; }
    public string? Speciality { get; set; }
    public string Email { get; set; }
    public int Year { get; set; }
    public RoleEnum Role { get; set; }
}