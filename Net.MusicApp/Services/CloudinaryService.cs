using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
namespace Net.MusicApp.Services
{

    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration config)
        {
            var acc = new Account(
                config["Cloudinary:CloudName"],
                config["Cloudinary:ApiKey"],
                config["Cloudinary:ApiSecret"]
            );

            _cloudinary = new Cloudinary(acc);
        }

        // Upload file từ backend (ít dùng)
        public async Task<string> UploadAsync(string filePath)
        {
            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(filePath),
                Folder = "musicapp"
            };

            var result = await _cloudinary.UploadAsync(uploadParams);
            return result.SecureUrl.ToString();
        }

        // Tạo signature cho client upload
        public string CreateSignature(Dictionary<string, object> parameters)
        {
            return _cloudinary.Api.SignParameters(parameters);
        }
        
    }
}
