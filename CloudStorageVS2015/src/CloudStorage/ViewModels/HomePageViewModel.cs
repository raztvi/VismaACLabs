using System.Collections.Generic;
using Core.Entities;

namespace CloudStorage.ViewModels
{
    public class HomePageViewModel
    {
        public string Message { get; set; }
        public IEnumerable<FileInfo> FileInfos { get; set; }
    }
}