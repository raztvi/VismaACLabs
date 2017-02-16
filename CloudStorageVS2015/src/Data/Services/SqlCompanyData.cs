using Core.Services;
using System.Collections.Generic;
using Core.Entities;

namespace Data.Services
{
    public class SqlCompanyData : ICompanyData
    {
        private CloudStorageDbContext _context;

        public SqlCompanyData(CloudStorageDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Company> GetAll()
        {
            return _context.Companies;
        }
    }
}