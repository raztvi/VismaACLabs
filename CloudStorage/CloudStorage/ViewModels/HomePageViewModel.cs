using CloudStorage.Entities;
using System.Collections.Generic;

namespace CloudStorage.ViewModels
{
    public class HomePageViewModel
    {
        public string Message { get; set; }
        public IEnumerable<FileInfo> FileInfos { get; set; }
    }
}
