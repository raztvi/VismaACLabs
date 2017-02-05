using CloudStorage.Entities;
using System;
using System.Collections.Generic;

namespace CloudStorage.Services
{
    public interface IFileData
    {
        IEnumerable<FileInfo> GetAll();
        FileInfo Get(Guid id);
        FileInfo Add(FileInfo file);
    }
}
