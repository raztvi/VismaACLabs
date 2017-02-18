using Core.Services;
using System.Collections.Generic;
using Core.Entities;
using System;

namespace Data.Services
{
    public class SqlCompanyData : ICompanyData
    {
        private CloudStorageDbContext _context;

        public SqlCompanyData(CloudStorageDbContext context)
        {
            _context = context;
        }

        public Company Add(Company company)
        {
            _context.Companies.Add(company);
            return company;
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public Company Get(Guid id)
        {
            return _context.Companies.Find(id);
        }

        public IEnumerable<Company> GetAll()
        {
            return _context.Companies;
        }
    }
}