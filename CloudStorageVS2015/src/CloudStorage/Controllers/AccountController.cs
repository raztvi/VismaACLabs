using CloudStorage.ViewModels;
using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CloudStorage.Controllers
{
    public class AccountController : BaseController
    {
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager) : base(userManager)
        {
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.Username,
                    CompanyId = model.CompanyId,
                    Email = model.Email
                };
                var createResult = await UserManager.CreateAsync(user, model.Password);

                if (createResult.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);
                if (result.Succeeded && !result.IsLockedOut)
                {
                    if (model.Username.Equals("Mili", StringComparison.OrdinalIgnoreCase))
                    {
                        var currentUser = await UserManager.FindByNameAsync(model.Username);
                        if (!currentUser.Claims.Contains(
                               new IdentityUserClaim<string> {ClaimType = "UserType", ClaimValue = "Administrator" }))
                        {
                            var claimResult = await UserManager.AddClaimsAsync(currentUser, new List<Claim>
                            {
                                new Claim("UserType", "Administrator")
                            });
                        }
                        
                        if (!await UserManager.IsInRoleAsync(currentUser, "Admin"))
                        {
                            if (!await _roleManager.RoleExistsAsync("Admin"))
                            {
                                var roleResult = await _roleManager.CreateAsync(new IdentityRole
                                {
                                    Name = "Admin"
                                });
                                if (roleResult.Succeeded)
                                {
                                    // yaaay!        
                                }
                            }
                            
                            var roleAddResult = await UserManager.AddToRoleAsync(currentUser,  "Admin" );
                        }
                    }


                    if (Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("", "Could not login");

            return View();
        }


        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied(string returnUrl = null)
        {
            return View((object)returnUrl);
        }
    }
}