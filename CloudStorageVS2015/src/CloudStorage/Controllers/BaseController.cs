using System.Threading.Tasks;
using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Core.Constants;
using System.Linq;

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
        protected string GetNonAdminUserCompanyId()
        {
            string companyId = string.Empty;

            if (User.Identity.IsAuthenticated && !User.IsInRole(AuthConstants.AdminRole))
            {
                companyId = User.Claims.FirstOrDefault(_ => _.Type.Equals(AuthConstants.CompanyClaim))?.Value;
            }

            return companyId;
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