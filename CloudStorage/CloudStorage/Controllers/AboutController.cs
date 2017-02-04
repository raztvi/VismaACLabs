using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CloudStorage.Controllers
{
    [Route("[controller]/[action]")]
    public class AboutController : Controller
    {
        // GET: /<controller>/
        public string Phone()
        {
            return "123-456-789";
        }

        public string Address()
        {
            return "Timisoara, Romania";
        }
    }
}
