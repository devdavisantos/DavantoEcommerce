using Api.Modules.Catalog.Application.Abstractions.Images;
using Api.Modules.Catalog.Application.Products.Create;
using Api.Modules.Catalog.Application.Products.Errors;
using Api.Shared.Domain;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace Api.Modules.Catalog.Infrastructure.Images
{
    public sealed class ProductImageProcessor : IProductImageProcessor
    {
        private const long MaxFileSize = 50 * 1024 * 1024;
        private const int MinDimension = 300;
        private const int MaxInputDimension = 10000;
        private const long MaxPixels = 50_000_000;
        private const int MaxOutputDimension = 3000;
        private const int WebpQuality = 90;

        private static readonly HashSet<string> AllowedFormats =
            new(StringComparer.OrdinalIgnoreCase)
            {
                "JPEG",
                "PNG",
                "WEBP"
            };

        public async Task<Result<ProcessedProductImage>> ProcessAsync(
            ProductImageInput image,
            CancellationToken cancellationToken = default)
        {
            if (image.Length <= 0)
            {
                return Result.Failure<ProcessedProductImage>(
                    ProductApplicationErrors.ImageEmpty);
            }

            if (image.Length > MaxFileSize)
            {
                return Result.Failure<ProcessedProductImage>(
                    ProductApplicationErrors.ImageTooLarge);
            }

            if (!image.Content.CanRead)
            {
                return Result.Failure<ProcessedProductImage>(
                    ProductApplicationErrors.InvalidImageFormat);
            }

            try
            {
                if (image.Content.CanSeek)
                    image.Content.Position = 0;

                var imageInfo = await Image.IdentifyAsync(
                    image.Content,
                    cancellationToken);

                if (imageInfo is null ||
                    imageInfo.Metadata.DecodedImageFormat is null ||
                    !AllowedFormats.Contains(
                        imageInfo.Metadata.DecodedImageFormat.Name))
                {
                    return Result.Failure<ProcessedProductImage>(
                        ProductApplicationErrors.InvalidImageFormat);
                }

                if (imageInfo.Width < MinDimension ||
                    imageInfo.Height < MinDimension ||
                    imageInfo.Width > MaxInputDimension ||
                    imageInfo.Height > MaxInputDimension ||
                    (long)imageInfo.Width * imageInfo.Height > MaxPixels)
                {
                    return Result.Failure<ProcessedProductImage>(
                        ProductApplicationErrors.InvalidImageDimensions);
                }

                if (image.Content.CanSeek)
                    image.Content.Position = 0;

                using var decodedImage = await Image.LoadAsync(
                    image.Content,
                    cancellationToken);

                decodedImage.Mutate(context => context.AutoOrient());

                if (decodedImage.Width > MaxOutputDimension ||
                    decodedImage.Height > MaxOutputDimension)
                {
                    decodedImage.Mutate(context =>
                        context.Resize(new ResizeOptions
                        {
                            Size = new Size(
                                MaxOutputDimension,
                                MaxOutputDimension),
                            Mode = ResizeMode.Max
                        }));
                }

                decodedImage.Metadata.ExifProfile = null;
                decodedImage.Metadata.IccProfile = null;
                decodedImage.Metadata.XmpProfile = null;

                var output = await EncodeWebpAsync(
                    decodedImage,
                    cancellationToken);

                return Result.Success(
                    new ProcessedProductImage(
                        output,
                        output.Length));
            }
            catch (UnknownImageFormatException)
            {
                return Result.Failure<ProcessedProductImage>(
                    ProductApplicationErrors.InvalidImageFormat);
            }
            catch (InvalidImageContentException)
            {
                return Result.Failure<ProcessedProductImage>(
                    ProductApplicationErrors.InvalidImageFormat);
            }
            catch (Exception) when (!cancellationToken.IsCancellationRequested)
            {
                return Result.Failure<ProcessedProductImage>(
                    ProductApplicationErrors.ImageProcessingFailed);
            }
        }

        private static async Task<MemoryStream> EncodeWebpAsync(
            Image image,
            CancellationToken cancellationToken)
        {
            var output = new MemoryStream();

            try
            {
                var encoder = new WebpEncoder
                {
                    Quality = WebpQuality
                };

                await image.SaveAsWebpAsync(
                    output,
                    encoder,
                    cancellationToken);

                output.Position = 0;

                return output;
            }
            catch
            {
                await output.DisposeAsync();
                throw;
            }
        }
    }
}