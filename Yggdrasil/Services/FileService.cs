using System.Text;
using Microsoft.Extensions.Options;
using Yggdrasil.OptionsModels;

namespace Yggdrasil.Services
{
    public interface IFileService
    {
        public bool IsFilesValidForHomework(IEnumerable<IFormFile> files, bool allowsNull = false);
        public string ErrorMessageForHomework();
    }

    public class FileService : IFileService
    {
        private readonly FileSettings _settings;
        public FileService(IOptions<FileSettings> settings)
        {
            _settings = settings.Value;
        }

        private string CreateExtensionInfoString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var extension in _settings.AllowedExtensions)
            {
                sb.Append(extension);
                sb.Append(" | ");
            }

            //sb.Remove(sb.Length-3, " | ".Length);
            return sb.ToString();
        }
        public string ErrorMessageForHomework()  =>
            $"Размер файла не может превышать {_settings.FileSizeLimit / (1024 * 1024)} Мб, иметь формат, отличный от {CreateExtensionInfoString()}, а также нельзя загружать более {_settings.MaxFilesPerHomework} файлов";

        public bool IsFilesValidForHomework(IEnumerable<IFormFile>? files, bool allowsNull = false)
        {
            bool isValid = true;

            if (files is null)
                if (allowsNull == false)
                    return !isValid;
                else
                    return isValid;


            if (files.Count() <= _settings.MaxFilesPerHomework)
            {
                long sizeLimit = _settings.FileSizeLimit;
                var permittedExtensions = _settings.AllowedExtensions!;
                foreach (var file in files)
                {
                    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                    if (file.Length > sizeLimit || string.IsNullOrEmpty(extension) || !permittedExtensions.Contains(extension))
                    {
                        isValid = false;
                        break;
                    }
                }
            }
            else
            {
                isValid = false;
            }
            return isValid;
        }
    }
}
