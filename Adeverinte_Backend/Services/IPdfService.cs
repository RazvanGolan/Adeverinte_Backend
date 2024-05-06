using Adeverinte_Backend.Entities;
using Adeverinte_Backend.Entities.Certificates;

namespace Adeverinte_Backend.Services;

public interface IPdfService
{
    public Task<PdfModel> UploadAsync(IFormFile? file, CertificateModel certificateModel);
    public Task<byte[]> DownloadAsync(string id);
}