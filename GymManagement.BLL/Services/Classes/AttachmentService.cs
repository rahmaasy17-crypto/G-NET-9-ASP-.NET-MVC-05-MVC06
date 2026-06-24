using GymManagement.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class AttachmentService : IAttachmentService
    {
        private readonly ILogger<AttachmentService> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly long _MaxFileSize = 5 * 1024 * 1024;
        private readonly string[] _AllowedExtensions ={".png",".jpeg",".jpg"};

        public AttachmentService(ILogger<AttachmentService> logger ,IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public async Task<string?> UploadAsync(Stream fileStream,string fileName,string folderName,CancellationToken ct = default)
        {
            if (fileStream is null || !fileStream.CanRead)return null;

            if (fileStream.Length == 0)return null;
            //check size
            if (fileStream.Length > _MaxFileSize)
            {_logger.LogError($"File Rejected : Too Large {fileStream.Length} Bytes");
           return null;
            }

            // Check Extensions
            var Extension = Path.GetExtension(fileName);

            if (string.IsNullOrWhiteSpace(Extension) ||!_AllowedExtensions.Contains(Extension))
            {
                _logger.LogError($"File Rejected : This Extnesion Not Allowed");

                return null;
            }

            // Locate Folder                //PL كدا واقف عند ال
            var uploadFolders = Path.Combine(_environment.ContentRootPath, folderName);
            Directory.CreateDirectory(uploadFolders);//لو مش موجود تعمله

            //create uniqe name
            var storedFileName = $"{Guid.NewGuid()}{fileName}";
            //F:\route\08 MVC\session5\demo\GymManagementSolution\GymManagement\MembersPhoto\photoname.jpg
            var FullFilePath =Path. Combine(uploadFolders, storedFileName);

            //status           //what i can do
            try
            {
                using var fullfileStream = new FileStream(FullFilePath, FileMode.Create, FileAccess.Write);

                await fileStream.CopyToAsync(fullfileStream, ct);
                return storedFileName;
            }
            catch (Exception e)
            {
                _logger.LogError(e,"Failed to Upload Photo");
                return null;
            }
        }
    }
}
