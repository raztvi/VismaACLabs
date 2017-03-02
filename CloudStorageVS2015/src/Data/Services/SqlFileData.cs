using System;
using System.Collections.Generic;
using Core.Entities;
using Core.Services;

namespace Data.Services
{
    public class SqlFileData : IFileData
    {
        private readonly CloudStorageDbContext _context;

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

        public void Delete(FileInfo fileInfo)
        {
            if (fileInfo != null)
                _context.FileInfos.Remove(fileInfo);
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