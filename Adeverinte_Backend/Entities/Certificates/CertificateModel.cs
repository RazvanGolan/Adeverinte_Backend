using Adeverinte_Backend.Entities.Enums;
using Adeverinte_Backend.Entities.Students;

namespace Adeverinte_Backend.Entities.Certificates;

public class CertificateModel : Entity
{
    public string Text { get; private set; }
    public bool OnEmail { get; private set; }
    public StudentModel Student { get; private set; }
    public TypeEnum Type { get; private set; }
    public string Motive { get; private set; }
    public StateEnum State { get; private set; }
    public DateTime Created { get; private set; }
    public DateTime? Accepted { get; private set; }
    public string? Number { get; private set; }
    public string ?RejectMsg { get; private set; }
    public PdfModel? Pdf { get; private set; }
    
    private CertificateModel()
    {
    }
    
}