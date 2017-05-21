using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Core.Entities;

namespace CloudStorage.ViewModels
{
    public class HomePageViewModel
    {
        public string Message { get; set; }

        [Display(Name = "Search by Name")]
        public string Query { get; set; }

        [Display(Name = "Search by Description")]
        public string Query2 { get; set; }

        public IEnumerable<FileInfo> FileInfos { get; set; }


    }
}