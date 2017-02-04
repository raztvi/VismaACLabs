using CloudStorage.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudStorage.Services
{
    public interface IFileData
    {
        IEnumerable<FileInfo> GetAll();
    }
}
