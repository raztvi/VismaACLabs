﻿using System;
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
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;


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
        // added something useless
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
        // query->filename 
        //query2->description
        public IActionResult Index(string query = null, string query2 = null, string sortOrder = null , string currentQuery = null , string currentQuery2 = null) 
        {  
            var result = _fileData.GetAll(GetNonAdminUserCompanyId());
            /*easier solution
              if (!string.IsNullOrWhiteSpace(query)) {
                   result = result.Where(s => s.FileName.ToLowerInvariant().Contains(query.ToLowerInvariant()));
              }
              if (!string.IsNullOrWhiteSpace(query2)) {
                 result = result.Where(s => s.Description.ToLowerInvariant().Contains(query2.ToLowerInvariant()));
              }
              */
              // create functionality for sort
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.SizeSortParm = sortOrder == "Size" ? "size_desc" : "Size";
            ViewBag.ContentSortParm = sortOrder == "Type" ? "type_desc" : "Type";

            // remembering search terms for sort
            if ( query == null)
            {
                query = currentQuery;
            }

            if (query2 == null)
            {
                query2 = currentQuery2;
            }

            ViewBag.CurrentQuery = query;
            ViewBag.CurrentQuery2 = query2;
           

            if ( !string.IsNullOrWhiteSpace(query) && string.IsNullOrWhiteSpace(query2))
               {// search only by name
                result = _fileData.Search(query);
               }
               else if ( !string.IsNullOrWhiteSpace(query2) && string.IsNullOrWhiteSpace(query))
                   { // search only be description
                result = _fileData.SearchDescription(query2);
                   }

               else if ( !string.IsNullOrWhiteSpace(query) && !string.IsNullOrWhiteSpace(query2))
                   {// search by both
                result = _fileData.SearchByAll(query, query2);
                   }

            switch (sortOrder)
            {//creating cases for sort
             
                case "name_desc":
                    result = result.OrderByDescending(_ => _.FileName);
                    break;
                case "size_desc":
                    result = result.OrderByDescending(_ => _.FileSizeInBytes);
                    break;
                case "Size":
                    result = result.OrderBy(_ => _.FileSizeInBytes);
                    break;
                case "type_desc":
                    result = result.OrderByDescending(_ => _.ContentType);
                    break;
                case "Type":
                    result = result.OrderBy(_ => _.ContentType);
                    break;
                default:
                    result = result.OrderBy(_ => _.FileName);
                    break;
            };


            var model = new HomePageViewModel
            {
                FileInfos = result,
                Message = _greeter.GetGreeting(),
                Query = query,
                Query2 = query2,

                
               
            };

            return View( model);
   
        }

        public IActionResult MyFiles()
        {
            var result = _fileData.GetAll(GetNonAdminUserCompanyId());

            return View(result);

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
        public async Task<IActionResult> Upload(UploadViewModel model, IList<IFormFile> files   )
        {
            if ((model != null) && ModelState.IsValid)
            {
                var user = await GetLoggedInUser();

                var containerName = user.CompanyId.ToString().ToLower();
                await _blobService.CreateContainerIfNotExists(containerName);

                foreach (var formFile in files)
              { 

                if ((formFile != null) && (formFile.Length > 0))
                {
                    using (var fileStream = formFile.OpenReadStream())
                    {
                        await
                            _blobService.UploadBlobFromStream(containerName, formFile.FileName, fileStream,
                                formFile.ContentType, false);
                    }
                    var file = new FileInfo
                    {
                        ContentType = model.ContentType,
                        FileName = formFile.FileName,
                        FileContentType = formFile.ContentType,
                        FileSizeInBytes = formFile.Length,
                        ContainerName = containerName,
                        Description = model.Description,
                        ReadOnly = model.ReadOnly,
                        FileOwner = user.UserName
                    };

                    _fileData.Add(file);
                    _fileData.Commit();

                    SendFileNotification(FileOperations.Uploaded, formFile.FileName, user.CompanyId.ToString());

                    
                }

              }

                return RedirectToAction(nameof(Index));
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
            if (!companyId.IsNullOrWhiteSpace() && !model.ContainerName.Equals(companyId, StringComparison.OrdinalIgnoreCase) || model.ReadOnly)
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
            if (!companyId.IsNullOrWhiteSpace() && !fileInfo.ContainerName.Equals(companyId, StringComparison.OrdinalIgnoreCase)
                || fileInfo.ReadOnly)
            {
                return RedirectToAction(nameof(AccountController.AccessDenied), nameof(AccountController).GetControllerName(),
                    new { returnUrl = Request.Path });
            }

            if (ModelState.IsValid)
            {
                fileInfo.Description = model.Description;
                fileInfo.ContentType = model.ContentType;
                fileInfo.ReadOnly = model.ReadOnly;
                _fileData.Commit();

                SendFileNotification(FileOperations.ModifiedMetadata, fileInfo.FileName,
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

        /*public async Task<IActionResult> TemporaryLink()
        {
            var user = await GetLoggedInUser();
            var url = await _blobService.GetTemporaryUrl(user.CompanyId.ToString(),);
        }*/

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

            if (file == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var companyId = GetNonAdminUserCompanyId();
            if (!companyId.IsNullOrWhiteSpace() && (string.IsNullOrWhiteSpace(file.ContainerName) || 
                !file.ContainerName.Equals(companyId, StringComparison.OrdinalIgnoreCase))
                || file.ReadOnly)
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
                if (!companyId.IsNullOrWhiteSpace() && !file.ContainerName.Equals(companyId, StringComparison.OrdinalIgnoreCase)
                    || file.ReadOnly)
                {
                    return RedirectToAction(nameof(AccountController.AccessDenied), nameof(AccountController).GetControllerName(),
                        new { returnUrl = Request.Path });
                }

                if (!string.IsNullOrWhiteSpace(file.ContainerName) && !string.IsNullOrWhiteSpace(file.FileName))
                {
                    try
                    {
                        await _blobService.DeleteBlob(file.ContainerName, file.FileName);
                    }
                    catch (ArgumentException)
                    {
                        // no container, fine because it's probably seed data
                    }
                }

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