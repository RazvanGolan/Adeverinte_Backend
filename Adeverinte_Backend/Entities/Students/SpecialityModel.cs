namespace Adeverinte_Backend.Entities;

public class SpecialityModel : Entity
{
    public string Name { get; private set; }
    public FacultyModel Faculty { get; private set; }
}