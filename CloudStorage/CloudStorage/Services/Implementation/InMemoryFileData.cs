using System;
using System.Collections.Generic;
using System.Linq;
using CloudStorage.Entities;

namespace CloudStorage.Services.Implementation
{
    public class InMemoryFileData : IFileData
    {
        private static List<FileInfo> _fileInfos;

        static InMemoryFileData()
        {
            _fileInfos = new List<FileInfo>
            {
                new FileInfo {Id = new Guid("ded4a7f5-361a-414e-830c-5ee42f0cf6be"), FileName = "test.jpeg", FileSizeInBytes = 13},
                new FileInfo {Id = new Guid("a0914891-fdb4-4c44-99d0-5cd6ad3dd3fc"), FileName = "bla.txt", FileSizeInBytes = 333},
                new FileInfo {Id = new Guid("edcd07a3-1e53-45d1-8e8c-bfa82a310938"), FileName = "index.html", FileSizeInBytes = 443},
                new FileInfo {Id = new Guid("67b6658e-0775-401e-b76a-93fb85f7200e"), FileName = "hello.pptx", FileSizeInBytes = 54323}
            };
        }

        public FileInfo Add(FileInfo file)
        {
            file.Id = Guid.NewGuid();
            _fileInfos.Add(file);
            return file;
        }

        public FileInfo Get(Guid id)
        {
            return _fileInfos.FirstOrDefault(_ => _.Id == id);
        }

        public IEnumerable<FileInfo> GetAll()
        {
            return _fileInfos;   
        }
    }
}
