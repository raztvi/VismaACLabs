using System;
using Microsoft.AspNetCore.Mvc;
using CloudStorage.Entities;
using CloudStorage.Services;
using CloudStorage.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CloudStorage.Controllers
{
    public class HomeController : Controller
    {
        private IFileData _fileData;
        private IGreeter _greeter;

        public HomeController(IFileData fileData, IGreeter greeter)
        {
            _fileData = fileData;
            _greeter = greeter;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var model = new HomePageViewModel
            {
                FileInfos = _fileData.GetAll(),
                Message = _greeter.GetGreeting()
            };

            return View(model);
        }

        public IActionResult Details(Guid id)
        {
            var model = _fileData.Get(id);

            if(model == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upload(UploadViewModel model)
        {
            if(model != null && ModelState.IsValid)
            {
                var file = new FileInfo
                {
                    ContentType = model.ContentType,
                    FileName = model.FileName,
                    FileSizeInBytes = (new Random()).Next(10, int.MaxValue)
                };

                _fileData.Add(file);

                return RedirectToAction(nameof(Details), new { id = file.Id });
            }
            return View(model);
        }
    }
}
