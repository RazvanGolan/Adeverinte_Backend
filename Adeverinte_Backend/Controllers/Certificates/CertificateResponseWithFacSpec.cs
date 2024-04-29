using Adeverinte_Backend.Controllers.Students;
using Adeverinte_Backend.Entities.Enums;

namespace Adeverinte_Backend.Controllers.Certificates;

public record CertificateResponseWithFacSpec(
    string Id,
    string Text,
    bool OnEmail,
    StudentResponse student,
    DateTime created,
    DateTime? accepted,
    TypeEnum Type,
    string Motive,
    string Number,
    StateEnum state,
    string PdfId);