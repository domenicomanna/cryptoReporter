namespace Api.Services.Emailing;

public class EmailMessage
{
    public List<string> To { get; set; } = new List<string>();
    public string Subject { get; set; } = String.Empty;
    public string HtmlBody { get; set; } = string.Empty;
    public string TextBody { get; set; } = string.Empty;
}
