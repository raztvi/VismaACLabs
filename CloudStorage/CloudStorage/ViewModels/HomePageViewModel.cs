using CloudStorage.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudStorage.ViewModels
{
    public class HomePageViewModel
    {
        public string Message { get; set; }
        public IEnumerable<FileInfo> FileInfos { get; set; }
    }
}
