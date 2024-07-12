using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Telerik_ForumTeamProject.Models.ServiceModel;
using Telerik_ForumTeamProject.Services.Contracts;

namespace Telerik_ForumTeamProject.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }
        public async Task<CloudinaryUploadResult> UploadImageAsync(IFormFile file)
        {
            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, stream),
                        Transformation = new Transformation().Crop("fill").Gravity("face").Width(150).Height(150)
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    // Map the ImageUploadResult to CloudinaryUploadResult
                    return new CloudinaryUploadResult
                    {
                        Url = uploadResult.SecureUrl?.AbsoluteUri,
                        PublicId = uploadResult.PublicId
                    };
                }
            }

            return null;
        }
    }
}
