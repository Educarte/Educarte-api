namespace Api.Infrastructure.Services.Email;

/// <summary>
/// 
/// </summary>
public class EmailMessage
{
    public string To { get; set; }

    public string Subject { get; set; }

    public string Content { get; set; }

    public string HtmlContent { get; set; }

    public IEnumerable<EmailAttachment> Attachments { get; set; }

    public class EmailAttachment
    {
        public string Filename { get; set; }
        public Stream Attachment { get; set; }
        public string Type { get; set; }
    }
}
