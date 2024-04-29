using Adeverinte_Backend.Entities.Enums;

namespace Adeverinte_Backend.Controllers.Certificates;

public record CertificateRequest(bool OnEmail, string StudentId, TypeEnum Type, string Motive);