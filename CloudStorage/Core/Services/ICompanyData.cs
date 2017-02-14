using Core.Entities;
using System.Collections.Generic;

namespace Core.Services
{
    public interface ICompanyData
    {
        IEnumerable<Company> GetAll();
    }
}