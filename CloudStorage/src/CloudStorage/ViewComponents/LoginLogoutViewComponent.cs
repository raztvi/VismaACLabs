using Microsoft.AspNetCore.Mvc;

namespace CloudStorage.ViewComponents
{
    public class LoginLogoutViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}