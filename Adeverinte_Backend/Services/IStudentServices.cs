using Adeverinte_Backend.Controllers.Students;
using Adeverinte_Backend.Entities.Students;

namespace Adeverinte_Backend.Services;

public interface IStudentServices
{
    public Task<StudentModel> FindById(string id);
    public Task<List<StudentModel>> GetAll();
    public Task Save();
    public Task<StudentModel> CreateStudent(StudentRequest request);
}