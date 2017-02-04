using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudStorage.Models;

namespace CloudStorage.Services.Implementation
{
    public class InMemoryFileData : IFileData
    {
        private List<FileInfo> _fileInfos;

        public InMemoryFileData()
        {
            _fileInfos = new List<FileInfo>
            {
                new FileInfo {Id = Guid.NewGuid(), FileName = "test.jpeg", FileSizeInBytes = 13},
                new FileInfo {Id = Guid.NewGuid(), FileName = "bla.txt", FileSizeInBytes = 333},
                new FileInfo {Id = Guid.NewGuid(), FileName = "index.html", FileSizeInBytes = 443},
                new FileInfo {Id = Guid.NewGuid(), FileName = "hello.pptx", FileSizeInBytes = 54323}
            };
        }

        public IEnumerable<FileInfo> GetAll()
        {
            return _fileInfos;   
        }
    }
}
