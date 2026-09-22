using Api.Controllers.Requests.Admin.Products;
using Api.Controllers.Responses;
using Api.Modules.Catalog.Application.Products.Create;
using Api.Modules.Catalog.Application.Products.Errors;
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

        public AdminProductsController(
            CreateProductHandler createProductHandler)
        {
            _createProductHandler = createProductHandler;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(105 * 1024 * 1024)]
        [RequestFormLimits(MultipartBodyLengthLimit = 105 * 1024 * 1024)]
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

            return BadRequest(response);
        }
    }
}