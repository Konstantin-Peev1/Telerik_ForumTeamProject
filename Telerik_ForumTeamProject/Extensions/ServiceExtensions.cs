using CloudinaryDotNet;
using Telerik_ForumTeamProject.Models.Settings;
using Telerik_ForumTeamProject.Services;
using Telerik_ForumTeamProject.Services.Contracts;

namespace Telerik_ForumTeamProject.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddCloudinaryService(this IServiceCollection services, IConfiguration configuration)
        {
            var cloudinarySettings = configuration.GetSection("CloudinarySettings").Get<CloudinarySettings>();

            var account = new Account(
                cloudinarySettings.CloudName,
                cloudinarySettings.ApiKey,
                cloudinarySettings.ApiSecret);

            var cloudinary = new Cloudinary(account)
            {
                Api = { Secure = true }
            };

            services.AddSingleton(cloudinary);
            services.AddScoped<ICloudinaryService, CloudinaryService>();
        }
    }
}
