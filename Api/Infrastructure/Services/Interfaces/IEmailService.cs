using Api.Infrastructure.Services.Email;

using Nudes.Retornator.Core;

namespace Api.Infrastructure.Services.Interfaces;

/// <summary>
/// 
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="To"></param>
    /// <param name="Subject"></param>
    /// <param name="Content"></param>
    /// <param name="HtmlContent"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Result> SendEmail(string To, string Subject, string Content, string HtmlContent, CancellationToken cancellationToken);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="mail"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Result> SendSimpleEmail(EmailMessage mail, CancellationToken cancellationToken);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="mail"></param>
    /// <param name="templateId"></param>
    /// <param name="templateData"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Result> SendTemplateEmail(EmailMessage mail, string templateId, object templateData, CancellationToken cancellationToken);
}