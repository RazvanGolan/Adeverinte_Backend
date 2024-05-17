using Adeverinte_Backend.Controllers.Certificates;
using Adeverinte_Backend.Entities;
using Adeverinte_Backend.Entities.Certificates;
using Adeverinte_Backend.Entities.Enums;
using Adeverinte_Backend.Entities.Students;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;

namespace Adeverinte_Backend.Services;

public class CertificateService : ICertificateService
{
    private readonly AppDbContext _appDbContext;
    private readonly IStudentService _studentService;
    private readonly IPdfService _pdfService;

    public CertificateService(AppDbContext appDbContext, IStudentService studentService, IPdfService pdfService)
    {
        _appDbContext = appDbContext;
        _studentService = studentService;
        _pdfService = pdfService;
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
        IQueryable<CertificateModel> queryable = _appDbContext.Certificates
            .Include(c => c.Student)
            .Include(c => c.Student.Faculty)
            .Include(c => c.Student.Speciality);

        if (!string.IsNullOrWhiteSpace(faculty))
        {
            queryable = queryable.Where(c => c.Student.Faculty.Name == faculty);
        }

        if (!string.IsNullOrWhiteSpace(speciality))
        {
            queryable = queryable.Where(c => c.Student.Speciality.Name == speciality);
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

    public async Task<CertificateModel> CreatePdfAsync(string certificateId)
    {
        var certificate =  await _appDbContext.Certificates
            .Include(c=>c.Student)
            .Include(c=>c.Pdf)
            .FirstOrDefaultAsync(c=>c.Id == certificateId);

        if (certificate is null)
            throw new Exception($"Certificate with {certificateId} does not exist.");

        if (certificate.State == StateEnum.Rejected || certificate.State == StateEnum.Waiting)
            throw new Exception($"Certificate with id {certificateId} is in the wrong state {certificate.State}.");
        
        QuestPDF.Settings.License = LicenseType.Community;
        
        MemoryStream memoryStream = new MemoryStream();
        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(1, Unit.Centimetre);
                page.Size(PageSizes.A4);

                page.Header()
                    .Row(row =>
                    {
                        row.Spacing(10, Unit.Centimetre);

                        row.ConstantItem(5, Unit.Centimetre)
                            .AlignLeft()
                            .Stack(stack =>
                            {
                                stack.Item().Height(0.7f, Unit.Centimetre).Text("");
                                stack.Item().Image("Images/ACLogo.png");
                                stack.Item().Height(0.3f, Unit.Centimetre).Text("");
                                stack.Item()
                                    .Text($"Numar : {certificate.Number}",
                                        TextStyle.Default); //number din database 
                            });

                        row.RelativeItem()
                            .Height(3, Unit.Centimetre)
                            .Width(4, Unit.Centimetre)
                            .AlignRight()
                            .Image("Images/Logo-UPT.jpg");
                    });

                page.Content()
                    .AlignCenter()
                    .AlignTop()
                    .Stack(stack =>
                    {
                        stack.Item().Height(2, Unit.Centimetre).Text("");
                        stack.Item().Text("Adeverință").FontSize(30).AlignCenter();
                        stack.Spacing(0.5f, Unit.Centimetre);
                        stack.Item().Text($"Tip {certificate.Type}").FontSize(20).AlignCenter();
                        stack.Item().Height(2, Unit.Centimetre).Text("");
                        stack.Item().Text("     " + certificate.Text).FontSize(15).AlignLeft();
                        stack.Item().Text("     " + $"Adeverința se eliberează pentru motivul {certificate.Motive}.").AlignLeft().FontSize(15);
                        stack.Item().Height(12, Unit.Centimetre).Text("");
                        stack.Item().Text("     " + $"Data: {certificate.Accepted.Value:dd/MM/yyyy}").FontSize(15).AlignLeft();
                    });
            });
        }).GeneratePdf(memoryStream);
        memoryStream.Position = 0;

        var pdf = new PdfModel();
        pdf.EditFileName("Adeverință_" + certificate.Student.FirstName + "_" + certificate.Student.LastName);
        pdf.EditFileData(memoryStream.ToArray());
        pdf.EditFileCertificate(certificate);
        
        certificate.EditPdf(pdf);
        await _appDbContext.SaveChangesAsync();
        
        return certificate;
    }

    public async Task<CertificateModel> UploadSignedPdfAsync(string id, IFormFile? file)
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
        
        if (certificate.Pdf is not null) //daca cumva este deja un pdf, il sterge din baza lui de date
        {
            var pdf = certificate.Pdf;
            certificate.DeletePdf(); //pun attributul de pdf pe null

            _appDbContext.Remove(pdf); //il sterg din baza de pdf-uri
            await _appDbContext.SaveChangesAsync();
        }

        try
        {
            var pdfEntity = await _pdfService.UploadAsync(file, certificate);
            certificate.EditPdf(pdfEntity); //aici dau patch efectiv si salvez modificarile

            await _appDbContext.SaveChangesAsync();
            return certificate;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<byte[]> DownloadPdf(string id)
    {
        try
        {
            return await _pdfService.DownloadAsync(id);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
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