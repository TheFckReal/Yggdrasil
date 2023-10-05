namespace Yggdrasil.OptionsModels
{
    public class FileSettings
    {
        public long FileSizeLimit { get; set; }
        public IEnumerable<string> AllowedExtensions { get; set; } = null!;
        public int MaxFilesPerHomework {get; set; }
    }
}
