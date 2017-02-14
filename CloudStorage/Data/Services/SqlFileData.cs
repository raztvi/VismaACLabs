using Core.Services;
using System;
using System.Collections.Generic;
using Core.Entities;

namespace Data.Services
{
    public class SqlFileData : IFileData
    {
        private CloudStorageDbContext _context;

        public SqlFileData(CloudStorageDbContext context)
        {
            _context = context;
        }

        public FileInfo Add(FileInfo file)
        {
            _context.Add(file);
            return file;
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public FileInfo Get(Guid id)
        {
            return _context.FileInfos.Find(id);
        }

        public IEnumerable<FileInfo> GetAll()
        {
            return _context.FileInfos;
        }
    }
}
