using System;

namespace CloudStorage.Entities
{
    public class FileInfo
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public int FileSizeInBytes { get; set; }
        public FileContentType ContentType { get; set; }
    }
}