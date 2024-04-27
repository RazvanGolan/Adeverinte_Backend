using Adeverinte_Backend.Entities.Enums;

namespace Adeverinte_Backend.Entities.Students;

public class StudentModel : Entity
{
    public StudentModel(string firstName, string lastName, string faculty, string speciality, string email, RoleEnum role, int year, string marca)
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
    public string Faculty { get; private set; }
    public string Speciality { get; set; }
    public string Email { get; private set; }
    public RoleEnum Role { get; private set; }
    public int Year { get; private set; }
    public string Marca { get; private set; }
}
