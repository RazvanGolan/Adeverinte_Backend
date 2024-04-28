using Adeverinte_Backend.Entities.Enums;

namespace Adeverinte_Backend.Entities.Students;

public class StudentModel : Entity
{

    private StudentModel()
    {
        
    }
    public StudentModel(string firstName, string lastName, FacultyModel faculty, SpecialityModel speciality, string email, RoleEnum role, int year, string marca)
    {
        FirstName = firstName;
        LastName = lastName;
        Faculty = faculty;
        Speciality = speciality;
        Email = email;
        Role = role;
        Year = year;
        Marca = marca;
    }

    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public FacultyModel Faculty { get; private set; }
    public SpecialityModel Speciality { get; set; }
    public string Email { get; private set; }
    public RoleEnum Role { get; private set; }
    public int Year { get; private set; }
    public string Marca { get; private set; }
}
