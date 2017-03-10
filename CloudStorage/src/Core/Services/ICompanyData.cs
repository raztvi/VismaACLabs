using System;
using System.Collections.Generic;
using Core.Entities;

namespace Core.Services
{
    public interface ICompanyData
    {
        IEnumerable<Company> GetAll();
        Company Get(Guid id);
        Company Add(Company company);
        void Commit();
    }
}