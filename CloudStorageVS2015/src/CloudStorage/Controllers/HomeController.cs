using System;
using System.Linq;
using System.Threading.Tasks;
using CloudStorage.Hubs;
using CloudStorage.Services;
using CloudStorage.ViewModels;
using Core.Constants;
using Core.Entities;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Infrastructure;
using Microsoft.Extensions.Logging;
using CloudStorage.Helpers;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CloudStorage.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly IBlobService _blobService;
        private readonly IConnectionManager _connectionManager;
        private readonly IFileData _fileData;
        private readonly IGreeter _greeter;
        private ILogger<HomeController> _logger;

        public HomeController(IFileData fileData, IGreeter greeter, ILogger<HomeController> logger,
            IBlobService blobService,
            UserManager<User> userManager, IConnectionManager connectionManager) : base(userManager)
        {
            _fileData = fileData;
            _greeter = greeter;
            _logger = logger;
            _blobService = blobService;
            _connectionManager = connectionManager;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var model = new HomePageViewModel
            {
                FileInfos = _fileData.GetAll(GetNonAdminUserCompanyId()),
                Message = _greeter.GetGreeting()
            };

            return View(model);
        }

        public IActionResult Details(Guid id)
        {
            var model = _fileData.Get(id);

            if (model == null)
                return RedirectToAction(nameof(Index));

            var companyId = GetNonAdminUserCompanyId();
            if (!companyId.IsNullOrWhiteSpace() && !model.ContainerName.Equals(companyId, StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction(nameof(AccountController.AccessDenied), nameof(AccountController).GetControllerName(),
                    new { returnUrl = Request.Path });
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
            if ((model != null) && ModelState.IsValid)
            {
                var user = await GetLoggedInUser();
                var containerName = user.CompanyId.ToString().ToLower();
                await _blobService.CreateContainerIfNotExists(containerName);

                if ((model.UploadedFile != null) && (model.UploadedFile.Length > 0))
                {
                    using (var fileStream = model.UploadedFile.OpenReadStream())
                    {
                        await
                            _blobService.UploadBlobFromStream(containerName, model.FileName, fileStream,
                                model.UploadedFile.ContentType, false);
                    }

                    var file = new FileInfo
                    {
                        ContentType = model.ContentType,
                        FileName = model.FileName,
                        FileContentType = model.UploadedFile.ContentType,
                        FileSizeInBytes = model.UploadedFile.Length,
                        ContainerName = containerName
                    };

                    _fileData.Add(file);
                    _fileData.Commit();

                    SendFileNotification(FileOperations.Uploaded, model.FileName, user.CompanyId.ToString());

                    return RedirectToAction(nameof(Details), new {id = file.Id});
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            var model = _fileData.Get(id);
            if (model == null)
                return RedirectToAction(nameof(Index));

            var companyId = GetNonAdminUserCompanyId();
            if (!companyId.IsNullOrWhiteSpace() && !model.ContainerName.Equals(companyId, StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction(nameof(AccountController.AccessDenied), nameof(AccountController).GetControllerName(),
                    new { returnUrl = Request.Path });
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, UploadViewModel model)
        {
            var fileInfo = _fileData.Get(id);

            var companyId = GetNonAdminUserCompanyId();
            if (!companyId.IsNullOrWhiteSpace() && !fileInfo.ContainerName.Equals(companyId, StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction(nameof(AccountController.AccessDenied), nameof(AccountController).GetControllerName(),
                    new { returnUrl = Request.Path });
            }

            if (ModelState.IsValid)
            {
                fileInfo.FileName = model.FileName;
                fileInfo.ContentType = model.ContentType;
                _fileData.Commit();

                SendFileNotification(FileOperations.ModifiedMetadata, model.FileName,
                    User.Claims.FirstOrDefault(_ => _.Type.Equals(AuthConstants.CompanyClaim)).Value);

                return RedirectToAction(nameof(Details), new {id = fileInfo.Id});
            }

            return View(fileInfo);
        }

        [HttpGet]
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
            if (fileInfo != null)
            {
                var companyId = GetNonAdminUserCompanyId();
                if (!companyId.IsNullOrWhiteSpace() && !fileInfo.ContainerName.Equals(companyId, StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }

                var stream = await _blobService.GetBlobStream(fileInfo.ContainerName, fileInfo.FileName);
                return File(stream, fileInfo.FileContentType, fileInfo.FileName);
            }
            return null;
        }

        [HttpGet]
        public IActionResult Delete(Guid id)
        {
            var file = _fileData.Get(id);

            var companyId = GetNonAdminUserCompanyId();
            if (!companyId.IsNullOrWhiteSpace() && !file.ContainerName.Equals(companyId, StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction(nameof(AccountController.AccessDenied), nameof(AccountController).GetControllerName(),
                    new { returnUrl = Request.Path });
            }

            return View(file);
        }

        [ActionName("Delete")]
        [HttpPost]
        public async Task<IActionResult> DeleteConfirm(Guid id)
        {
            var file = _fileData.Get(id);
            if (file != null)
            {
                var companyId = GetNonAdminUserCompanyId();
                if (!companyId.IsNullOrWhiteSpace() && !file.ContainerName.Equals(companyId, StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction(nameof(AccountController.AccessDenied), nameof(AccountController).GetControllerName(),
                        new { returnUrl = Request.Path });
                }

                if (!string.IsNullOrWhiteSpace(file.ContainerName) && !string.IsNullOrWhiteSpace(file.FileName))
                    await _blobService.DeleteBlob(file.ContainerName, file.FileName);
                _fileData.Delete(file);
                _fileData.Commit();
                SendFileNotification(FileOperations.Deleted, file.FileName,
                    User.Claims.FirstOrDefault(_ => _.Type.Equals(AuthConstants.CompanyClaim)).Value);
            }

            return RedirectToAction(nameof(Index));
        }


        private void SendFileNotification(string operation, string fileName, string companyId)
        {
            var hubContext = _connectionManager.GetHubContext<FileOperationsHub>();

            hubContext.Clients.Group(companyId /*, Context.ConnectionId" */)
                .fileModified(
                    $"{User.Identity.Name} executed the following operation {operation} on the following file: {fileName}");
        }
    }
}