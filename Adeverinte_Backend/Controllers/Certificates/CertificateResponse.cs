using Adeverinte_Backend.Entities.Enums;
using Adeverinte_Backend.Entities.Students;

namespace Adeverinte_Backend.Controllers.Certificates;

public record CertificateResponse(
    string Id,
    string Text,
    bool OnEmail,
    StudentModel student,
    TypeEnum Type,
    string Motive,
    string Number,
    StateEnum state,
    string PdfId);