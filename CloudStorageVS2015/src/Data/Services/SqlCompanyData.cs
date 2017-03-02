using System;
using System.Collections.Generic;
using Core.Entities;
using Core.Services;

namespace Data.Services
{
    public class SqlCompanyData : ICompanyData
    {
        private readonly CloudStorageDbContext _context;

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