using Core.Entities;
using System;
using System.Collections.Generic;

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