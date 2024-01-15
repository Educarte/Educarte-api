using Amazon.Runtime.Internal;
using Amazon.S3;
using Amazon.S3.Transfer;
using Api.Infrastructure.Options;
using Api.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Api.Infrastructure.Services;

public class SpaceService : IDisposable, ISpaceService
{
    private const int TIMEOUT = 5;
    private readonly IAmazonS3 s3Client;
    private readonly SpaceOptions spaceOptions;


    public SpaceService(IOptionsSnapshot<SpaceOptions> spaceOptions)
    {

        s3Client = new AmazonS3Client(spaceOptions.Value.AccessKey, spaceOptions.Value.AwsSecretAccessKey, new AmazonS3Config
        {
            ForcePathStyle = false,
            ServiceURL = spaceOptions.Value.ServiceURL,
            Timeout = TimeSpan.FromSeconds(TIMEOUT),
            MaxErrorRetry = 1
        });

        this.spaceOptions = spaceOptions.Value;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileStream"></param>
    /// <param name="keyName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async ValueTask<string> UploadFileAsync(Stream fileStream, string keyName, CancellationToken cancellationToken)
    {
        var fileTransferUtility = new TransferUtility(s3Client);

        try
        {
            await fileTransferUtility.UploadAsync(fileStream, spaceOptions.SpaceName, keyName, cancellationToken);
            await s3Client.PutACLAsync(new Amazon.S3.Model.PutACLRequest
            {
                BucketName = spaceOptions.SpaceName,
                Key = keyName,
                CannedACL = S3CannedACL.PublicRead
            }, cancellationToken);

            return $"{spaceOptions.SpaceLink}{"/"}{keyName}";

        }
        catch (AmazonS3Exception e)
        {
            throw e;
        }
        catch (HttpErrorResponseException e)
        {
            throw e;
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            Dispose();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {

    }
}
