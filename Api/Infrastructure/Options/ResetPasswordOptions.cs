namespace Api.Infrastructure.Options;

public class ResetPasswordOptions
{
    public int CodeExpirationTime { get; set; }
    public string ResetUri { get; set; }
    public string CompanyName { get; set; }

    //With Template Settings
    public string DesktopResetPasswordTemplateId { get; set; }
    public string MobileResetPasswordTemplateId { get; set; }
    public string TempPasswordTemplateId { get; set; }

    //Non Template Settings
    public string EmailSubject { get; set; }
    public string EmailContent { get; set; }
    public string EmailHtmlContent { get; set; }
}
