using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CloudStorage.Models;
using CloudStorage.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CloudStorage.Controllers
{
    public class HomeController : Controller
    {
        private IFileData _fileData;

        public HomeController(IFileData fileData)
        {
            _fileData = fileData;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var model = _fileData.GetAll();
            return View(model);
        }
    }
}
