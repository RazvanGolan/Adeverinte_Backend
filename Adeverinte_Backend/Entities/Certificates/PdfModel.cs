using Adeverinte_Backend.Entities.Certificates;

namespace Adeverinte_Backend.Entities;

public class PdfModel : Entity
{
    public string FileName { get; private set; }
    public byte[] Data { get; private set; }
    public CertificateModel Certificate { get; private set; }
    public string CertificateId { get; private set; }

    public void EditFileName(string name)
    {
        FileName = name;
    }

    public void EditFileData(byte[] data)
    {
        Data = data;
    }
    
    public void EditFileCertificate(CertificateModel certificateModel)
    {
        Certificate = certificateModel;
        CertificateId = certificateModel.Id;
    }
}