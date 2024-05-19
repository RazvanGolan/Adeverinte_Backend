using Adeverinte_Backend.Controllers.Students;
using Adeverinte_Backend.Entities.Students;

namespace Adeverinte_Backend.Services;

public interface IStudentService
{
    public Task<StudentModel> FindById(string id);
    public Task<StudentModel> FindByEmail(string email);
    public Task<List<StudentModel>> GetAll();
    public Task Save();
    public Task<StudentModel> CreateStudent(StudentRequest request);
}