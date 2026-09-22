using Amazon.S3;
using Amazon.S3.Model;
using Api.Modules.Catalog.Application.Abstractions.Storage;
using Microsoft.Extensions.Options;

namespace Api.Integrations.Cloudflare.R2;

public sealed class CloudflareR2ObjectStorage : IObjectStorage
{
    private readonly IAmazonS3 _s3Client;
    private readonly R2Settings _settings;

    public CloudflareR2ObjectStorage(
        IAmazonS3 s3Client,
        IOptions<R2Settings> options)
    {
        _s3Client = s3Client;
        _settings = options.Value;
    }

    public async Task UploadAsync(
        string key,
        Stream stream,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var request = new PutObjectRequest
        {
            BucketName = _settings.BucketName,
            Key = key,
            InputStream = stream,
            ContentType = contentType,
            DisablePayloadSigning = true,
            DisableDefaultChecksumValidation = true
        };

        await _s3Client.PutObjectAsync(request, cancellationToken);
    }

    public async Task DeleteAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        var request = new DeleteObjectRequest
        {
            BucketName = _settings.BucketName,
            Key = key
        };

        await _s3Client.DeleteObjectAsync(request, cancellationToken);
    }
}