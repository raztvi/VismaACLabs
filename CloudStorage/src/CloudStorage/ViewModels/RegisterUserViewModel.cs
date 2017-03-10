using System;
using System.ComponentModel.DataAnnotations;

namespace CloudStorage.ViewModels
{
    public class RegisterUserViewModel
    {
        [Required]
        [MaxLength(256)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

        [Required]
        public Guid CompanyId { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}