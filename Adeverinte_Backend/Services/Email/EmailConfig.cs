namespace Adeverinte_Backend.Services.Email;

public class EmailConfig
{
    public string From { get; set; }
    public string EmailHost { get; set; }
    
    public int Port { get; set; }
    
    public string EmailUserName { get; set; }
    
    public string EmailPassword { get; set; }
}