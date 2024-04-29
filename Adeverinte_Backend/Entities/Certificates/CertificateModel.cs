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
    
    public static async Task<CertificateModel> CreateAsync(
        StudentModel student,
        TypeEnum type,
        string motive,
        bool onEmail
    )
    {
        var text = type + " " + motive;
        
        return new CertificateModel
        {
            Text = text,
            Student = student,
            Type = type,
            Motive = motive,
            Created = DateTime.UtcNow,
            State = 0,
            OnEmail = onEmail
        };
    }
    
    public void EditNumber(int? order)
    {
        if (order is null)
            Number = null;
        else
        {
            var number = $"{order.ToString()}/{DateTime.Today:dd.MM}";
            Number = number;
        }
    }
    
    public void EditState(StateEnum state)
    {
        State = state;
    }

    public void AcceptCertificate(DateTime? acceptTime)
    {
        Accepted = acceptTime;
    }
    
    public void EditRejectedMessage(string? text)
    {
        RejectMsg = text;
    }
    
    public void EditPdf(PdfModel pdfModel)
    {
        Pdf = pdfModel;
    }

    public void DeletePdf()
    {
        Pdf = null;
    }
    
}