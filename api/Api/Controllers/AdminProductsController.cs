using Api.Controllers.Requests.Admin.Products;
using Api.Controllers.Responses;
using Api.Modules.Catalog.Application.Products.Create;
using Api.Modules.Catalog.Application.Products.Errors;
using Api.Modules.Catalog.Application.Products.GetAll;
using Api.Modules.Catalog.Application.Products.GetById;
using Api.Modules.Catalog.Domain.Enums;
using Api.Shared.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/admin/products")]
    [Authorize(Policy = "AdminOnly")]
    public sealed class AdminProductsController : ControllerBase
    {
        private readonly CreateProductHandler _createProductHandler;
        private readonly GetProductsHandler _getProductsHandler;
        private readonly GetProductByIdHandler _getProductByIdHandler;

        public AdminProductsController(
            CreateProductHandler createProductHandler,
            GetProductsHandler getProductsHandler,
            GetProductByIdHandler getProductByIdHandler)
        {
            _createProductHandler = createProductHandler;
            _getProductsHandler = getProductsHandler;
            _getProductByIdHandler = getProductByIdHandler;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(110 * 1024 * 1024)]
        [RequestFormLimits(MultipartBodyLengthLimit = 110 * 1024 * 1024)]
        public async Task<IActionResult> CreateAsync(
            [FromForm] CreateProductRequest request,
            CancellationToken cancellationToken)
        {
            var images = request.Images
                .Select(image => new ProductImageInput(
                    image.OpenReadStream(),
                    image.Length))
                .ToArray();

            var command = new CreateProductCommand(
                request.CategoryExternalId,
                request.BrandExternalId,
                request.Title,
                request.Description,
                request.Sku,
                request.Price,
                request.DiscountPrice,
                request.Stock,
                request.Condition,
                images);

            var result = await _createProductHandler.HandleAsync(
                command,
                cancellationToken);

            if (result.IsFailure)
                return MapError(result.Error);

            return StatusCode(
                StatusCodes.Status201Created,
                new
                {
                    ExternalId = result.Value
                });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null,
            [FromQuery] ProductStatus? status = null,
            CancellationToken cancellationToken = default)
        {
            var query = new GetProductsQuery(
                page,
                pageSize,
                search,
                status);

            var result = await _getProductsHandler.HandleAsync(
                query,
                cancellationToken);

            if (result.IsFailure)
                return MapError(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("{externalId:guid}")]
        public async Task<IActionResult> GetByIdAsync(
            Guid externalId,
            CancellationToken cancellationToken)
        {
            var query = new GetProductByIdQuery(
                externalId);

            var result = await _getProductByIdHandler.HandleAsync(
                query,
                cancellationToken);

            if (result.IsFailure)
                return MapError(result.Error);

            return Ok(result.Value);
        }

        private IActionResult MapError(Error error)
        {
            var response = new ApiErrorResponse(
                error.Code,
                error.Message);

            if (error == ProductApplicationErrors.CategoryNotFound ||
                error == ProductApplicationErrors.BrandNotFound)
            {
                return NotFound(response);
            }

            if (error == ProductApplicationErrors.SkuAlreadyExists ||
                error == ProductApplicationErrors.SlugConflict)
            {
                return Conflict(response);
            }

            if (error == ProductApplicationErrors.CategoryNotFound ||
                error == ProductApplicationErrors.BrandNotFound ||
                error == ProductApplicationErrors.ProductNotFound)
            {
                return NotFound(response);
            }

            return BadRequest(response);
        }
    }
}