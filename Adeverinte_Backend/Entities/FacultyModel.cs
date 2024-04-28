namespace Adeverinte_Backend.Entities;

public class FacultyModel : Entity
{
    public string Name { get; private set; }
   
    public List<SpecialityModel> Specialities { get; private set; }

    public FacultyModel() {}
    
    public FacultyModel(string name, List<SpecialityModel> studentSpeciality)
    {
        Name = name;
        Specialities = studentSpeciality;
    }
}