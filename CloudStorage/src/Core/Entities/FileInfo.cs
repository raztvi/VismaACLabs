using System;

namespace Core.Entities
{
    public class FileInfo
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public long FileSizeInBytes { get; set; }
        public FileContentType ContentType { get; set; }
        public string FileContentType { get; set; }
        public string ContainerName { get; set; }
        public string Description { get; set; }
        public bool ReadOnly { get; set; }
    }
}