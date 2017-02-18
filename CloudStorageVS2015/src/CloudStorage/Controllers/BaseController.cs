using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CloudStorage.Controllers
{
    public class BaseController : Controller
    {
        protected UserManager<User> _userManager;

        public BaseController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [NonAction]
        protected async Task<User> GetLoggedInUser()
        {
            User result = null;

            if (User.Identity.IsAuthenticated)
            {
                result = await _userManager.FindByNameAsync(User.Identity.Name);
            }

            return result;
        }
    }
}