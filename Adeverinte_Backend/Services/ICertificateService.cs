using Adeverinte_Backend.Controllers.Certificates;
using Adeverinte_Backend.Entities.Certificates;
using Adeverinte_Backend.Entities.Enums;

namespace Adeverinte_Backend.Services;

public interface ICertificateService
{
    public Task<List<CertificateModel>> GetAllCertificatesAsync(CertificateParameters certificateParameters);
    public Task<CertificateModel> GetCertificateByIdAsync(string id);
    public Task<List<CertificateModel>> GetCertificateByStudentEmailAsync(string email);
    public Task<List<CertificateModel>> GetCertificateBigFilter(CertificateParameters certificateParameters, bool? today, bool? week,
        bool? month, string? faculty, string? speciality, int? year, TypeEnum? type, StateEnum? state);
    Task<CertificateModel> CreateCertificateAsync(CertificateRequest certificate);
    Task DeleteCertificateAsync(string id);
    Task<int> GetCountForTodayAsync();
    Task<CertificateModel> UpdateToApproved(string id);
    Task<CertificateModel> UpdateToRejected(string id, string RejectedMessage);
    Task<CertificateModel> UpdateToSigned(string id);
    Task<CertificateModel> UpdateToWaiting(string id);



}