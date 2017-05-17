using System;
using System.Collections.Generic;
using Core.Entities;
using Core.Services;
using System.Linq;

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

        public IEnumerable<FileInfo> Search(string term, string companyId = null)
        {
            var tmp = _context.FileInfos.Where(_ => _.FileName.ToLowerInvariant().Contains(term.ToLowerInvariant()));
            return string.IsNullOrWhiteSpace(companyId)
                ? tmp
                : ((IEnumerable<FileInfo>)tmp).Where(
                    _ => companyId.Equals(_.ContainerName, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<FileInfo> SearchDescription(string term, string companyId = null)
        {
            var tmp = _context.FileInfos.Where(_ => _.Description.ToLowerInvariant().Contains(term.ToLowerInvariant()));
                                            
            return string.IsNullOrWhiteSpace(companyId)
                ? tmp
                : ((IEnumerable<FileInfo>) tmp).Where(
                    _ => companyId.Equals(_.ContainerName, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<FileInfo> SearchByAll(string term, string term2, string companyId = null)
        {
            var tmp = _context.FileInfos.Where(_ => _.FileName.ToLowerInvariant().Contains(term.ToLowerInvariant())
                                                && _.Description.ToLowerInvariant().Contains(term2.ToLowerInvariant())
                                                );

            return string.IsNullOrWhiteSpace(companyId)
                ? tmp
                : ((IEnumerable<FileInfo>)tmp).Where(
                    _ => companyId.Equals(_.ContainerName, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<FileInfo> GetAll(string companyId = null)
        {
            return string.IsNullOrWhiteSpace(companyId) ? _context.FileInfos :
                _context.FileInfos.Where(_ => companyId.Equals(_.ContainerName, StringComparison.OrdinalIgnoreCase));
        }
    }
}