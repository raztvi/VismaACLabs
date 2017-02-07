using System;
using Microsoft.AspNetCore.Mvc;
using Core.Entities;
using CloudStorage.Services;
using CloudStorage.ViewModels;
using Core.Services;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CloudStorage.Controllers
{
    public class HomeController : Controller
    {
        private IFileData _fileData;
        private IGreeter _greeter;
        private ILogger<HomeController> _logger;

        public HomeController(IFileData fileData, IGreeter greeter, ILogger<HomeController> logger)
        {
            _fileData = fileData;
            _greeter = greeter;
            _logger = logger;
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
                _fileData.Commit();

                return RedirectToAction(nameof(Details), new { id = file.Id });
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            var model = _fileData.Get(id);
            if(model == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, UploadViewModel model)
        {
            var fileInfo = _fileData.Get(id);
            if (ModelState.IsValid)
            {
                fileInfo.FileName = model.FileName;
                fileInfo.ContentType = model.ContentType;
                _fileData.Commit();

                return RedirectToAction(nameof(Details), new { id = fileInfo.Id });
            }

            return View(fileInfo);
        }
    }
}
