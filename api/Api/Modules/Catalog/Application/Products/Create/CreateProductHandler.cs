using Api.Modules.Catalog.Application.Abstractions.Images;
using Api.Modules.Catalog.Application.Abstractions.Persistence;
using Api.Modules.Catalog.Application.Abstractions.Storage;
using Api.Modules.Catalog.Application.Products.Errors;
using Api.Modules.Catalog.Application.Services;
using Api.Modules.Catalog.Domain.Entities;
using Api.Shared.Domain;
using FluentValidation;

namespace Api.Modules.Catalog.Application.Products.Create
{
    public sealed class CreateProductHandler
    {
        private const int MaxSlugCreationAttempts = 3;
        private const long MaxTotalImageSize = 100 * 1024 * 1024;

        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly IValidator<CreateProductCommand> _validator;
        private readonly IProductImageProcessor _productImageProcessor;
        private readonly IObjectStorage _objectStorage;
        private readonly ILogger<CreateProductHandler> _logger;

        public CreateProductHandler(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IBrandRepository brandRepository,
            IValidator<CreateProductCommand> validator,
            IProductImageProcessor productImageProcessor,
            IObjectStorage objectStorage,
            ILogger<CreateProductHandler> logger)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _brandRepository = brandRepository;
            _validator = validator;
            _productImageProcessor = productImageProcessor;
            _objectStorage = objectStorage;
            _logger = logger;
        }

        public async Task<Result<Guid>> HandleAsync(
            CreateProductCommand command,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await _validator.ValidateAsync(
                command,
                cancellationToken);

            if (!validationResult.IsValid)
            {
                return Result.Failure<Guid>(
                    ProductApplicationErrors.ValidationFailed);
            }

            var totalImageSize = command.Images.Sum(image => image.Length);

            if (totalImageSize > MaxTotalImageSize)
            {
                return Result.Failure<Guid>(
                    ProductApplicationErrors.ImagesTooLarge);
            }

            uint? categoryId = null;
            uint? brandId = null;

            if (command.CategoryExternalId.HasValue)
            {
                var category = await _categoryRepository.GetByExternalIdAsync(
                    command.CategoryExternalId.Value,
                    cancellationToken);

                if (category is null)
                {
                    return Result.Failure<Guid>(
                        ProductApplicationErrors.CategoryNotFound);
                }

                if (!category.Active)
                {
                    return Result.Failure<Guid>(
                        ProductApplicationErrors.CategoryInactive);
                }

                categoryId = category.Id;
            }

            if (command.BrandExternalId.HasValue)
            {
                var brand = await _brandRepository.GetByExternalIdAsync(
                    command.BrandExternalId.Value,
                    cancellationToken);

                if (brand is null)
                {
                    return Result.Failure<Guid>(
                        ProductApplicationErrors.BrandNotFound);
                }

                if (!brand.Active)
                {
                    return Result.Failure<Guid>(
                        ProductApplicationErrors.BrandInactive);
                }

                brandId = brand.Id;
            }

            var sku = string.IsNullOrWhiteSpace(command.Sku)
                ? null
                : command.Sku.Trim().ToUpperInvariant();

            if (sku is not null &&
                await _productRepository.SkuExistsAsync(
                    sku,
                    cancellationToken))
            {
                return Result.Failure<Guid>(
                    ProductApplicationErrors.SkuAlreadyExists);
            }

            var baseSlug = SlugGenerator.Generate(command.Title);

            if (string.IsNullOrWhiteSpace(baseSlug))
            {
                return Result.Failure<Guid>(
                    ProductApplicationErrors.SlugGenerationFailed);
            }

            for (var attempt = 1; attempt <= MaxSlugCreationAttempts; attempt++)
            {
                var existingSlugs = await _productRepository
                    .GetSlugsByPrefixAsync(
                        baseSlug,
                        cancellationToken);

                var slug = SlugGenerator.GenerateUnique(
                    baseSlug,
                    existingSlugs);

                var productResult = Product.Create(
                    categoryId,
                    brandId,
                    command.Title,
                    slug,
                    command.Description,
                    sku,
                    command.Price,
                    command.DiscountPrice,
                    command.Stock,
                    command.Condition);

                if (productResult.IsFailure)
                {
                    return Result.Failure<Guid>(
                        productResult.Error);
                }

                var product = productResult.Value;

                var processedImages = new List<ProcessedProductImage>(
                    command.Images.Count);

                var uploadedImageKeys = new List<string>(
                    command.Images.Count);

                try
                {
                    for (var index = 0; index < command.Images.Count; index++)
                    {
                        var image = command.Images.ElementAt(index);

                        var imageResult = await _productImageProcessor.ProcessAsync(
                            image,
                            cancellationToken);

                        if (imageResult.IsFailure)
                        {
                            await DeleteUploadedImagesAsync(
                                uploadedImageKeys);

                            return Result.Failure<Guid>(
                                imageResult.Error);
                        }

                        var processedImage = imageResult.Value;
                        processedImages.Add(processedImage);

                        var imageExternalId = Guid.NewGuid();
                        var imageKey =
                            $"products/{product.ExternalId}/{imageExternalId}.webp";

                        var productImageResult = ProductImage.Create(
                            imageExternalId,
                            imageKey,
                            index == 0,
                            (uint)index);

                        if (productImageResult.IsFailure)
                        {
                            await DeleteUploadedImagesAsync(
                                uploadedImageKeys);

                            return Result.Failure<Guid>(
                                productImageResult.Error);
                        }

                        var addImageResult = product.AddImage(
                            productImageResult.Value);

                        if (addImageResult.IsFailure)
                        {
                            await DeleteUploadedImagesAsync(
                                uploadedImageKeys);

                            return Result.Failure<Guid>(
                                addImageResult.Error);
                        }

                        try
                        {
                            await _objectStorage.UploadAsync(
                                imageKey,
                                processedImage.Content,
                                "image/webp",
                                cancellationToken);

                            uploadedImageKeys.Add(imageKey);
                        }
                        catch
                        {
                            await DeleteUploadedImagesAsync(
                                uploadedImageKeys);

                            throw;
                        }
                    }

                    try
                    {
                        await _productRepository.CreateAsync(
                            product,
                            cancellationToken);

                        return Result.Success(
                            product.ExternalId);
                    }
                    catch (ProductPersistenceException exception)
                        when (exception.Failure ==
                              ProductPersistenceFailure.DuplicateSlug)
                    {
                        await DeleteUploadedImagesAsync(
                            uploadedImageKeys);

                        if (attempt == MaxSlugCreationAttempts)
                        {
                            return Result.Failure<Guid>(
                                ProductApplicationErrors.SlugConflict);
                        }

                        continue;
                    }
                    catch (ProductPersistenceException exception)
                        when (exception.Failure ==
                              ProductPersistenceFailure.DuplicateSku)
                    {
                        await DeleteUploadedImagesAsync(
                            uploadedImageKeys);

                        return Result.Failure<Guid>(
                            ProductApplicationErrors.SkuAlreadyExists);
                    }
                    catch
                    {
                        await DeleteUploadedImagesAsync(
                            uploadedImageKeys);

                        throw;
                    }
                }
                finally
                {
                    foreach (var processedImage in processedImages)
                        await processedImage.Content.DisposeAsync();
                }
            }

            return Result.Failure<Guid>(
                ProductApplicationErrors.SlugConflict);
        }

        private async Task DeleteUploadedImagesAsync(
            IReadOnlyCollection<string> imageKeys)
        {
            foreach (var imageKey in imageKeys)
            {
                try
                {
                    await _objectStorage.DeleteAsync(
                        imageKey,
                        CancellationToken.None);
                }
                catch (Exception exception)
                {
                    _logger.LogError(
                        exception,
                        "Failed to delete orphaned product image from object storage. ImageKey: {ImageKey}",
                        imageKey);
                }
            }
        }

    }
}