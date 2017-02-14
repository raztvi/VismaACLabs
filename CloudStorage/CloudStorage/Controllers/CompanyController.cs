using Core.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudStorage.Controllers
{
    public class CompanyController : Controller
    {
        ICompanyData _companyData;

        public CompanyController(ICompanyData companyData)
        {
            _companyData = companyData;
        }

        public IActionResult Index()
        {
            return View(_companyData.GetAll());
        }
    }
}
