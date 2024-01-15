namespace Api.Infrastructure.Services.Interfaces;

public interface ISpaceService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileStream"></param>
    /// <param name="keyName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>File Uri</returns>
    public ValueTask<string> UploadFileAsync(Stream fileStream, string keyName, CancellationToken cancellationToken);
}