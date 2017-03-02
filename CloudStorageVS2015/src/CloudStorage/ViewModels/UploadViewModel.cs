using System.ComponentModel.DataAnnotations;
using Core.Entities;
using Microsoft.AspNetCore.Http;

namespace CloudStorage.ViewModels
{
    public class UploadViewModel
    {
        [Required]
        [Display(Name = "File name")]
        [MaxLength(128, ErrorMessage = "Too long!")]
        public string FileName { get; set; }

        public FileContentType ContentType { get; set; }
        public IFormFile UploadedFile { get; set; }
    }
}