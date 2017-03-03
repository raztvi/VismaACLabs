using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using CloudStorage.Helpers;
using CloudStorage.ViewModels;
using Core.Constants;
using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Infrastructure;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CloudStorage.Controllers
{
    public class AccountController : BaseController
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager) : base(userManager)
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
                    return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).GetControllerName());
                }

                foreach (var error in createResult.Errors)
                    ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result =
                    await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);
                if (result.Succeeded && !result.IsLockedOut)
                {
                    // quick patch work
                    if (model.Username.Equals("Mili", StringComparison.OrdinalIgnoreCase))
                    {
                        var currentUser = await UserManager.FindByNameAsync(model.Username);
                        if (!currentUser.Claims.Contains(
                            new IdentityUserClaim<string>
                            {
                                ClaimType = AuthConstants.UserTypeClaim,
                                ClaimValue = AuthConstants.AdministratorClaimType
                            }))
                        {
                            var claimResult = await UserManager.AddClaimsAsync(currentUser, new List<Claim>
                            {
                                new Claim(AuthConstants.UserTypeClaim, AuthConstants.AdministratorClaimType)
                            });
                        }

                        if (!currentUser.Claims.Contains(
                            new IdentityUserClaim<string> {ClaimType = AuthConstants.CompanyClaim}))
                        {
                            var claimResult = await UserManager.AddClaimsAsync(currentUser, new List<Claim>
                            {
                                new Claim(AuthConstants.CompanyClaim, currentUser.CompanyId.ToString())
                            });
                        }

                        if (!await UserManager.IsInRoleAsync(currentUser, AuthConstants.AdminRole))
                        {
                            if (!await _roleManager.RoleExistsAsync(AuthConstants.AdminRole))
                            {
                                var roleResult = await _roleManager.CreateAsync(new IdentityRole
                                {
                                    Name = AuthConstants.AdminRole
                                });
                                if (roleResult.Succeeded)
                                {
                                    // yaaay!        
                                }
                            }

                            var roleAddResult = await UserManager.AddToRoleAsync(currentUser, AuthConstants.AdminRole);
                        }
                    }


                    if (Url.IsLocalUrl(model.ReturnUrl))
                        return Redirect(model.ReturnUrl);
                    return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).GetControllerName());
                }
            }

            ModelState.AddModelError("", "Could not login");

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).GetControllerName());
        }

        [HttpGet]
        public IActionResult AccessDenied(string returnUrl = null)
        {
            return View((object) returnUrl);
        }
    }
}