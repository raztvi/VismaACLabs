using Core.Entities;
using System;
using System.Collections.Generic;

namespace Core.Services
{
    public interface IFileData
    {
        IEnumerable<FileInfo> GetAll();
        FileInfo Get(Guid id);
        FileInfo Add(FileInfo file);
        void Commit();
    }
}
