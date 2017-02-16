using Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace CloudStorage.ViewModels
{
    public class UploadViewModel
    {
        [Required]
        [Display(Name = "File name")]
        [MaxLength(128, ErrorMessage = "Too long!")]
        public string FileName { get; set; }
        public FileContentType ContentType { get; set; }
    }
}
