using CloudStorage.Services;
using CloudStorage.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CloudStorage.Controllers
{
    [Route("[controller]/[action]")]
    public class AboutController : Controller
    {
        private readonly IGreeter _greeter;

        public AboutController(IGreeter greeter)
        {
            _greeter = greeter;
        }

        // GET: /<controller>/
        public string Phone()
        {
            return "123-456-789";
        }

        public string Address()
        {
            return "Timisoara, Romania";
        }

        public IActionResult TestMe()
        {
            var model = new TestingViewModel
            {
                Greeting = _greeter.GetGreeting(),
                CurrentDate = DateTime.UtcNow
            };

            return View(model);
        }
    }
}