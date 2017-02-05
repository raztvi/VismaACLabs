using CloudStorage.Entities;

namespace CloudStorage.ViewModels
{
    public class UploadViewModel
    {
        public string FileName { get; set; }
        public FileContentType ContentType { get; set; }
    }
}
