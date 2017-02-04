using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudStorage.Models
{
    public class FileInfo
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public int FileSizeInBytes { get; set; }
    }
}
