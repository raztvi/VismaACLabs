using System.Threading.Tasks;
using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CloudStorage.Controllers
{
    public class BaseController : Controller
    {
        protected UserManager<User> UserManager;

        public BaseController(UserManager<User> userManager)
        {
            UserManager = userManager;
        }

        [NonAction]
        protected async Task<User> GetLoggedInUser()
        {
            User result = null;

            if (User.Identity.IsAuthenticated)
                result = await UserManager.FindByNameAsync(User.Identity.Name);

            return result;
        }
    }
}