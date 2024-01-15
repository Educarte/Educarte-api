using Api.Infrastructure.Options;
using Api.Infrastructure.Services.Email;
using Api.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Options;
using Nudes.Retornator.AspnetCore.Errors;
using Nudes.Retornator.Core;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Api.Infrastructure.Services;

/// <summary>
/// 
/// </summary>
public class EmailService : IEmailService
{
    private readonly EmailOptions options;
    private readonly IHostEnvironment hostEnvironment;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="optionsSnapshot"></param>
    public EmailService(IOptionsSnapshot<EmailOptions> optionsSnapshot, IHostEnvironment hostEnvironment)
    {
        options = optionsSnapshot.Value;
        this.hostEnvironment = hostEnvironment;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="To"></param>
    /// <param name="Subject"></param>
    /// <param name="Content"></param>
    /// <param name="HtmlContent"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Result> SendEmail(string To, string Subject, string Content, string HtmlContent, CancellationToken cancellationToken)
    {
        var msg = MailHelper.CreateSingleEmail(from: new EmailAddress(options.EmailSender),
                                               to: new EmailAddress(To),
                                               subject: Subject,
                                               plainTextContent: Content,
                                               htmlContent: HtmlContent);

        return await TrySendEmail(msg, cancellationToken);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="mail"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Result> SendSimpleEmail(EmailMessage mail, CancellationToken cancellationToken)
    {
        var msg = MailHelper.CreateSingleEmail(from: new EmailAddress(options.EmailSender),
                                               to: new EmailAddress(mail.To),
                                               subject: mail.Subject,
                                               plainTextContent: mail.Content,
                                               htmlContent: mail.HtmlContent);

        if (mail.Attachments != null)
        {
            var attachmentTasks = mail.Attachments.Select(attachment => msg.AddAttachmentAsync(attachment.Filename,
                                                                                               attachment.Attachment,
                                                                                               attachment.Type,
                                                                                               cancellationToken: cancellationToken));

            await Task.WhenAll(attachmentTasks);
        }

        return await TrySendEmail(msg, cancellationToken);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="mail"></param>
    /// <param name="templateId"></param>
    /// <param name="templateData"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Result> SendTemplateEmail(EmailMessage mail, string templateId, object templateData, CancellationToken cancellationToken)
    {
        var msg = MailHelper.CreateSingleTemplateEmail(from: new EmailAddress(options.EmailSender),
                                                       to: new EmailAddress(mail.To),
                                                       templateId: templateId,
                                                       dynamicTemplateData: templateData);

        if (mail.Attachments != null)
        {
            var attachmentTasks = mail.Attachments.Select(attachment => msg.AddAttachmentAsync(attachment.Filename,
                                                                                               attachment.Attachment,
                                                                                               attachment.Type,
                                                                                               cancellationToken: cancellationToken));
            await Task.WhenAll(attachmentTasks);
        }

        return await TrySendEmail(msg, cancellationToken);
    }


    private async Task<Result> TrySendEmail(SendGridMessage msg, CancellationToken cancellationToken)
    {
        try
        {
            var client = new SendGridClient(options.Key);

            var result = await client.SendEmailAsync(msg, cancellationToken);

            if (result.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                if (hostEnvironment.IsDevelopment())
                    return new InternalServerError($"{result.StatusCode} - '{await result.Body.ReadAsStringAsync(cancellationToken)}");

                return new BadRequestError();
            }
            return Result.Success;
        }
        catch (Exception ex)
        {
            return new InternalServerError(ex.Message);
        }
    }
}
