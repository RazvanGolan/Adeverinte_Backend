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