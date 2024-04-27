using Adeverinte_Backend.Entities.Certificates;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Adeverinte_Backend.Controllers.Certificates;

[Route("Certificates")]
[ApiController]
public class CertificateController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    
    public CertificateController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CertificateResponse>>> GetCertificates()
    {
        var certificates =  await _dbContext.Certificates
            .Include(c=>c.Student)
            .Include(c=>c.Pdf)
            .ToListAsync();

        return Ok(certificates.Select(Map));
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
}