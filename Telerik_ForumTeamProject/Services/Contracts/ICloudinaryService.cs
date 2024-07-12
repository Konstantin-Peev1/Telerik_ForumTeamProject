using CloudinaryDotNet.Actions;
using Telerik_ForumTeamProject.Models.ServiceModel;

namespace Telerik_ForumTeamProject.Services.Contracts
{
    public interface ICloudinaryService
    {
        Task<CloudinaryUploadResult> UploadImageAsync(IFormFile file);
    }
}