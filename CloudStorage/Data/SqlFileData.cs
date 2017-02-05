using Core.Services;
using System;
using System.Collections.Generic;
using Core.Entities;

namespace Data
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
            _context.SaveChanges();
            return file;
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
