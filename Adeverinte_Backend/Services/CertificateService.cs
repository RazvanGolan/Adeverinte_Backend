using Adeverinte_Backend.Controllers.Certificates;
using Adeverinte_Backend.Entities.Certificates;
using Adeverinte_Backend.Entities.Enums;
using Adeverinte_Backend.Entities.Students;
using Microsoft.EntityFrameworkCore;

namespace Adeverinte_Backend.Services;

public class CertificateService : ICertificateService
{
    private readonly AppDbContext _appDbContext;
    private readonly IStudentService _studentService;

    public CertificateService(AppDbContext appDbContext, IStudentService studentService)
    {
        _appDbContext = appDbContext;
        _studentService = studentService;
    }


    public async Task<List<CertificateModel>> GetAllCertificatesAsync(CertificateParameters certificateParameters)
    {
        return await _appDbContext.Certificates
            .OrderBy(c => c.Number)
            .Skip((certificateParameters.PageNumber - 1) *  certificateParameters.PageSize)
            .Take(certificateParameters.PageSize)
            .Include(c=>c.Student)
            .Include(c=>c.Pdf)
            .ToListAsync();
    }

    public async Task<CertificateModel> GetCertificateByIdAsync(string id)
    {
        var certificate =  await _appDbContext.Certificates
            .Include(c=>c.Student)
            .Include(c=>c.Pdf)
            .FirstOrDefaultAsync(c=>c.Id == id);

        if (certificate is null)
            throw new Exception($"Certificate with {id} does not exist.");

        return certificate;
    }

    public async Task<List<CertificateModel>> GetCertificateByStudentEmailAsync(string email)
    {
        var certificates = await _appDbContext.Certificates
            .Include(c => c.Student)
            .Include(c => c.Pdf)
            .Where(c => c.Student.Email == email)
            .ToListAsync();
        
        return certificates;
    }

    public async Task<List<CertificateModel>> GetCertificateBigFilter(CertificateParameters certificateParameters, bool? today, 
        bool? week, bool? month, string? faculty, string? speciality, int? year, TypeEnum? type, StateEnum? state)
    {
        IQueryable<CertificateModel> queryable = _appDbContext.Certificates.Include(c => c.Student)
            .Include(c => c.Student.Faculty)
            .Include(c => c.Student.Speciality);

        if (!string.IsNullOrWhiteSpace(faculty))
        {
            queryable = queryable.Where(c => c.Student.Faculty.Id == faculty);
        }

        if (!string.IsNullOrWhiteSpace(speciality))
        {
            queryable = queryable.Where(c => c.Student.Speciality.Id == speciality);
        }

        if (year is not null)
        {
            queryable = queryable.Where(c => c.Student.Year == year);
        }

        if (today is not null && today == true)
        {
            queryable = queryable.Where(c => c.Created.Date == DateTime.UtcNow.Date);
        }
        else if(week is not null && week == true)
        {
            DateTime startWeek = DateTime.UtcNow.AddDays(-(int)DateTime.Today.DayOfWeek);
            DateTime endWeek = DateTime.UtcNow.AddDays(7).AddSeconds(-1);
            queryable = queryable.Where(c => c.Created.Date >= startWeek && c.Created.Date <= endWeek);
        }
        else if(month is not null && month == true)
        {
            DateTime currentMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            queryable = queryable.Where(c => c.Created.Date.Month == currentMonth.Month);
        }

        if (type is not null)
        {
            queryable = queryable.Where(c => c.Type == type);
        }

        if (state is not null)
        {
            queryable = queryable.Where(c => c.State == state);
        }

        queryable = queryable.OrderBy(c => c.Created)
             .Skip((certificateParameters.PageNumber - 1) * certificateParameters.PageSize)
             .Take(certificateParameters.PageSize);
        
        return await queryable.ToListAsync();
    }

    public async Task<CertificateModel> CreateCertificateAsync(CertificateRequest request)
    {
        StudentModel student;
        try
        { 
            student = await _studentService.FindById(request.StudentId);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }

        if (string.IsNullOrWhiteSpace(request.Motive))
            throw new Exception($"Null motive");

        var certificate = await CertificateModel.CreateAsync(student, request.Type, request.Motive, request.OnEmail);
        
        _appDbContext.Add(certificate);
        await _appDbContext.SaveChangesAsync();

        return certificate;
    }

    public async Task DeleteCertificateAsync(string id)
    {
        var certificate =  await _appDbContext.Certificates
            .FirstOrDefaultAsync(c=>c.Id == id);

        if (certificate is null)
            throw new Exception($"Certificate with {id} does not exist.");

        _appDbContext.Certificates.Remove(certificate);
        await _appDbContext.SaveChangesAsync();
    }

    public async Task<int> GetCountForTodayAsync()
    {
        //counts the number of certificates approved for the day
        
        return await _appDbContext.Certificates
            .Where(c => c.State == StateEnum.Approved)
            .CountAsync(c => c.Accepted != null && c.Accepted.Value.Date == DateTime.UtcNow.Date);    }

    public async Task<CertificateModel> UpdateToApproved(string id)
    {
        CertificateModel certificate;
        try
        {
            certificate = await GetCertificateByIdAsync(id);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }

        var countForToday = await GetCountForTodayAsync() + 1;
        
        certificate.EditNumber(countForToday);
        
        certificate.EditState(StateEnum.Approved);
        
        certificate.AcceptCertificate(DateTime.UtcNow);
        
        await _appDbContext.SaveChangesAsync();
        return certificate;
    }

    public async Task<CertificateModel> UpdateToRejected(string id, string rejectedMessage)
    {
        CertificateModel certificate;
        try
        {
            certificate = await GetCertificateByIdAsync(id);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }

        if (string.IsNullOrWhiteSpace(rejectedMessage))
            throw new Exception($"Bad reject message.");
        
        certificate.EditRejectedMessage(rejectedMessage);
        
        certificate.EditState(StateEnum.Rejected);
        
        certificate.EditNumber(null);
        
        certificate.AcceptCertificate(null);

        await _appDbContext.SaveChangesAsync();
        return certificate;
    }

    public async Task<CertificateModel> UpdateToSigned(string id)
    {
        CertificateModel certificate;
        try
        {
            certificate = await GetCertificateByIdAsync(id);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
        
        certificate.EditState(StateEnum.Signed);

        await _appDbContext.SaveChangesAsync();
        return certificate;
    }
    
    public async Task<CertificateModel> UpdateToWaiting(string id)
    {
        CertificateModel certificate;
        try
        {
            certificate = await GetCertificateByIdAsync(id);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
        
        certificate.EditState(StateEnum.Waiting);
        
        certificate.EditNumber(null);
        
        certificate.DeletePdf();
        
        certificate.AcceptCertificate(null);
        
        certificate.EditRejectedMessage(null);

        await _appDbContext.SaveChangesAsync();
        return certificate;
    }
}