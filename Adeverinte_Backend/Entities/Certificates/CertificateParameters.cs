namespace Adeverinte_Backend.Entities.Certificates;

public class CertificateParameters
{
    const int maxPageSize = 50;
    public int PageNumber { get; set; } = 1;
    private int _pageSize = 100;
    public int PageSize
    {
        get
        {
            return _pageSize;
        }
        set
        {
            _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }
    }
}