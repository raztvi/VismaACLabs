using System;
using Microsoft.AspNetCore.Mvc;
using Core.Entities;
using CloudStorage.Services;
using CloudStorage.ViewModels;
using Core.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CloudStorage.Controllers
{
    public class HomeController : BaseController
    {
        private IFileData _fileData;
        private IGreeter _greeter;
        private ILogger<HomeController> _logger;
        private IBlobService _blobService;

        public HomeController(IFileData fileData, IGreeter greeter, ILogger<HomeController> logger, IBlobService blobService,
            UserManager<User> userManager) : base(userManager)
        {
            _fileData = fileData;
            _greeter = greeter;
            _logger = logger;
            _blobService = blobService;
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
        public async Task<IActionResult> Upload(UploadViewModel model)
        {
            if(model != null && ModelState.IsValid)
            {
                var user = await GetLoggedInUser();
                var containerName = user.CompanyId.ToString().ToLower();
                await _blobService.CreateContainerIfNotExists(containerName);

                if(model.UploadedFile != null && model.UploadedFile.Length > 0)
                {
                    using (var fileStream = model.UploadedFile.OpenReadStream())
                    {
                        await _blobService.UploadBlobFromStream(containerName, model.FileName, fileStream, model.UploadedFile.ContentType, false);
                    }

                    var file = new FileInfo
                    {
                        ContentType = model.ContentType,
                        FileName = model.FileName,
                        FileContentType = model.UploadedFile.ContentType,
                        FileSizeInBytes = model.UploadedFile.Length
                    };

                    _fileData.Add(file);
                    _fileData.Commit();

                    return RedirectToAction(nameof(Details), new { id = file.Id });
                }
            }
            return View(model);
        }

        [Authorize]
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

        [Authorize]
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

        [HttpGet, Authorize]
        public async Task<IActionResult> TotalCompanyFilesSize()
        {
            var user = await GetLoggedInUser();
            var size = await _blobService.GetContainerSize(user.CompanyId.ToString());

            return View(size);
        }

        [HttpGet]
        public async Task<FileResult> Download(Guid id)
        {
            var fileInfo = _fileData.Get(id);
            if(fileInfo != null)
            {
                var stream = await _blobService.GetBlobStream(fileInfo.ContainerName, fileInfo.FileName);
                return File(stream, fileInfo.FileContentType, fileInfo.FileName);
            }
            return null;
        }

        [HttpGet, Authorize]
        public IActionResult Delete(Guid id)
        {
            var file = _fileData.Get(id);
            return View(file);
        }

        [ActionName("Delete"), HttpPost, Authorize]
        public async Task<IActionResult> DeleteConfirm(Guid id)
        {
            var file = _fileData.Get(id);
            if(file != null)
            {
                if(!string.IsNullOrWhiteSpace(file.ContainerName) && !string.IsNullOrWhiteSpace(file.FileName))
                {
                    await _blobService.DeleteBlob(file.ContainerName, file.FileName);
                }
                _fileData.Delete(file);
                _fileData.Commit();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}