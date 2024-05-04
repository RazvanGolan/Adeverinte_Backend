using Adeverinte_Backend.Entities;
using Adeverinte_Backend.Entities.Certificates;
using Microsoft.EntityFrameworkCore;

namespace Adeverinte_Backend.Services;

public class PdfService : IPdfService
{
    private readonly AppDbContext _dbContext;

    public PdfService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<PdfModel> UploadAsync(IFormFile? file, CertificateModel certificateModel)
    {
        if (file is null || file.Length == 0)
            throw new Exception($"No file uploaded.");
        
        if (!file.FileName.Contains(".pdf"))
        {
            throw new Exception($"Not a pdf file.");
        }

        var pdfEntity = new PdfModel(); //creez o noua entitate si ii setez numele
        pdfEntity.EditFileName(file.FileName);

        using (var memoryStream = new MemoryStream()) //apoi ii setez datele
        {
            await file.CopyToAsync(memoryStream);
            pdfEntity.EditFileData(memoryStream.ToArray());
        }
        
        pdfEntity.EditFileCertificate(certificateModel);

        return pdfEntity;
    }
    
    public async Task<byte[]> DownloadAsync(string id)
    {
        var certificate = await _dbContext.Certificates
            .Include(certificateModel => certificateModel.Pdf)
            .FirstOrDefaultAsync(c=>c.Id == id);
        
        if (certificate is null)
            throw new Exception($"Cannot find certificate with id {id}.");

        var pdfEntity = certificate.Pdf;

        if (pdfEntity is null)
            throw new Exception($"Certificate with id {id} does not have a pdf.");
        
        return pdfEntity.Data;
    }
    
}