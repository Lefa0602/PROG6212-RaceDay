
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDay.API.Services;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BlobStorageTestController : ControllerBase
    {
        private readonly BlobStorageService _blobStorage;

        public BlobStorageTestController(BlobStorageService blobStorage)
        {
            _blobStorage = blobStorage;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadTestImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("Please select an image.");
            }

            if (image.Length > 5 * 1024 * 1024)
            {
                return BadRequest("Image must be 5 MB or smaller.");
            }

            string[] allowedTypes =
            {
                "image/jpeg",
                "image/png",
                "image/webp"
            };

            if (!allowedTypes.Contains(image.ContentType.ToLowerInvariant()))
            {
                return BadRequest("Only JPG, PNG and WEBP images are allowed.");
            }

            string blobName = await _blobStorage.UploadImageAsync(
                image,
                "test");

            return Ok(new
            {
                message = "Image uploaded successfully to Blob Storage.",
                blobName
            });
        }
    }
}
