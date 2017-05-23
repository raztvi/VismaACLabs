using System.ComponentModel.DataAnnotations;
using Core.Entities;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;


namespace CloudStorage.ViewModels
{
    public class UploadViewModel
    {
        [Required]
        [Display(Name = "Description")]
        [MaxLength(512, ErrorMessage = "Too long!")]
        public string Description { get; set; }

        [Display(Name = "Content type")]
        public FileContentType ContentType { get; set; }

        [Display(Name = "File")]
        public List<IFormFile> UploadedFile { get; set; }

        [Display(Name = "Read only")]
        public bool ReadOnly { get; set; }

    }
}