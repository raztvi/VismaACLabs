using System.ComponentModel.DataAnnotations;

namespace CloudStorage.ViewModels
{
    public class CreateCompanyViewModel
    {
        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public string MainAddress { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        public string ContactPhoneNumber { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string ContactEmail { get; set; }
    }
}