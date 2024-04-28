namespace Adeverinte_Backend.Controllers.Students;

public class FacultyResponse
{
    public string  Id { get; set; }
    public string Name { get;  set; }
    public List<SpecialityResponse> Specialities { get;  set; }
}