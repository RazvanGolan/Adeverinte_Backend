using Adeverinte_Backend.Controllers.Students;
using Adeverinte_Backend.Entities;
using Adeverinte_Backend.Entities.Certificates;
using Adeverinte_Backend.Entities.Enums;
using Adeverinte_Backend.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Adeverinte_Backend.Controllers.Certificates;

[Route("Certificates")]
[ApiController]
public class CertificateController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly ICertificateService _certificateService;
    public CertificateController(AppDbContext dbContext, ICertificateService certificateService)
    {
        _dbContext = dbContext;
        _certificateService = certificateService;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CertificateResponse>>> GetCertificates(
        [FromQuery] CertificateParameters certificateParameters)
    {
        var certificates = await _certificateService.GetAllCertificatesAsync(certificateParameters);

        return Ok(certificates.Select(Map));
    }
    
    [HttpGet("id")]
    public async Task<ActionResult<CertificateResponse>> GetByIdAsync(string id)
    {
        try
        {
            var certificate = await _certificateService.GetCertificateByIdAsync(id);
            return Ok(Map(certificate));
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpGet("email")]
    public async Task<ActionResult<List<CertificateResponse>>> GetByStudentEmail(string email)
    {
        try
        {
            var certificates = await _certificateService.GetCertificateByStudentEmailAsync(email);
            return Ok(certificates.Select(Map));
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpGet("SortByAll")]
    public async Task<ActionResult<List<CertificateResponse>>> GetByBig([FromQuery]CertificateParameters certificateParameters,
        bool? today, bool? week, bool? month, string? facultyName, string? specialityName, int? year, TypeEnum? type, StateEnum? state)
    {
        var certificates = await _certificateService.GetCertificateBigFilter(certificateParameters, today, week, month,
            facultyName, specialityName, year, type, state);
        
        return Ok(certificates.Select(MapWithSpecialityAndFaculty));
    }
    
    [HttpGet("GetPdf/id")]
    public async Task<ActionResult> GetPdfByIdAsync(string id)
    {
        try
        {
            var pdfBytes =  await _certificateService.DownloadPdf(id);
            return Ok(pdfBytes);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPost]
    public async Task<ActionResult<CertificateResponse>> CreateCertificate(
        [FromBody] CertificateRequest certificateRequest)
    {
        try
        {
            var certificate = await _certificateService.CreateCertificateAsync(certificateRequest);
            return Ok(MapWithSpecialityAndFaculty(certificate));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPost("SentEmail/{id}")]
    public async Task<ActionResult<CertificateResponse>> SendEmail(string id)
    {
        try
        {
            var studentEmail = await _certificateService.SendEmailAsync(id);

            return Ok($"The email was successfully sent to the student address {studentEmail}");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPost("SentRejectedEmail/{id}")]
    public async Task<ActionResult<CertificateResponse>> SendRejectedEmail(string id)
    {
        try
        {
            var studentEmail = await _certificateService.SendRejectedEmailAsync(id);

            return Ok($"The email was successfully sent to the student address {studentEmail}");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPost("GeneratePdf")]
    public async Task<ActionResult<CertificateResponse>> CreatePdf(string certificateId)
    {
        try
        {
            var certificate = await _certificateService.CreatePdfAsync(certificateId);
            return Ok(Map(certificate));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpDelete("id")]
    public async Task<ActionResult> RemoveCertificate(string id)
    {
        try
        {
            await _certificateService.DeleteCertificateAsync(id);
            return Ok($"Certificate with id {id} was removed.");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPatch("PatchState/id")]
    public async Task<ActionResult<CertificateResponse>> UptdateState(string id,
        [FromQuery] StateEnum state, string? rejectMessage)
    {
        switch (state)
        {
            case StateEnum.Approved:
                try
                {
                    var certificate = await _certificateService.UpdateToApproved(id);
                    return Ok(Map(certificate));
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }

            case StateEnum.Rejected:
                try
                {
                    var certificate = await _certificateService.UpdateToRejected(id, rejectMessage);
                    return Ok(Map(certificate));
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }

            case StateEnum.Signed:
                try
                {
                    var certificate = await _certificateService.UpdateToSigned(id);
                    return Ok(Map(certificate));
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }

            case StateEnum.Waiting:
                try
                {
                    var certificate = await _certificateService.UpdateToWaiting(id);
                    return Ok(Map(certificate));
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            default:
                return BadRequest();
        }
    }
    
    [HttpPatch("UploadSignedPdf/id")]
    public async Task<ActionResult<CertificateResponse>> UploadPdf(string id, IFormFile? file)
    {
        try
        {
            var certificate = await _certificateService.UploadSignedPdfAsync(id, file);

            return Ok(Map(certificate));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    private CertificateResponse Map(CertificateModel certificateModel)
    {
        var pdfId = "";
        if (certificateModel.Pdf is null)
            pdfId = "null";
        else
        {
            pdfId = certificateModel.Pdf.Id;
        }
        
        return new CertificateResponse(certificateModel.Id,
            certificateModel.Text,
            certificateModel.OnEmail,
            certificateModel.Student,
            certificateModel.Type,
            certificateModel.Motive,
            certificateModel.Number,
            certificateModel.State,
            pdfId);
    }
    
    private CertificateResponseWithFacSpec MapWithSpecialityAndFaculty(CertificateModel certificateModel)
    {
        var pdfId = "";
        if (certificateModel.Pdf is null)
            pdfId = "null";
        else
        {
            pdfId = certificateModel.Pdf.Id;
        }
        
        
        var student = new StudentResponse(certificateModel.Student.Id, certificateModel.Student.Email, certificateModel.Student.FirstName,
            certificateModel.Student.LastName, certificateModel.Student.Faculty.Name, certificateModel.Student.Speciality.Name,
            certificateModel.Student.Year, certificateModel.Student.Role);
        
        return new CertificateResponseWithFacSpec(certificateModel.Id,
            certificateModel.Text,
            certificateModel.OnEmail,
            student,
            certificateModel.Created,
            certificateModel.Accepted,
            certificateModel.Type,
            certificateModel.Motive,
            certificateModel.Number,
            certificateModel.State,
            pdfId);
    }
}