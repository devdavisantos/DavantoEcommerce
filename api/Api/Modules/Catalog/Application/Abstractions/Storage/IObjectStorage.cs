namespace Api.Modules.Catalog.Application.Abstractions.Storage;

public interface IObjectStorage
{
    Task UploadAsync(
        string key,
        Stream stream,
        string contentType,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        string key,
        CancellationToken cancellationToken = default);

    string GetPublicUrl(string key);
}