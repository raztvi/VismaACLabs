using Core.Constants;
using Core.Entities;
using Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CloudStorage.Seed
{
    public class UsersAndRolesSeeder
    {
        private const string Password = "Abc123!";

        public static async Task SeedUsersAndRoles(IServiceProvider serviceProvider)
        {
            var userManager = (serviceProvider.GetService(typeof(UserManager<User>)) as UserManager<User>);
            var roleManager = (serviceProvider.GetService(typeof(RoleManager<IdentityRole>)) as RoleManager<IdentityRole>);
            var companyData = (serviceProvider.GetService(typeof(ICompanyData)) as ICompanyData);

            var roleSeedSucceeded = await SeedRoles(roleManager);
            if (roleSeedSucceeded)
            {
                await SeedUsers(userManager, companyData);
            }
        }

        private static async Task<bool> SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync(AuthConstants.AdminRole))
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole
                {
                    Name = AuthConstants.AdminRole
                });

                // TODO: log result
                return roleResult.Succeeded;
            }

            return true;
        }

        private static async Task SeedUsers(UserManager<User> userManager, ICompanyData companyData)
        {
            var companies = companyData.GetAll().ToArray();

            var existingUser = await userManager.FindByNameAsync("Mili");
            if(existingUser == null)
            {
                var user = new User
                {
                    UserName = "Mili",
                    CompanyId = companies[0].Id,
                    Email = "master@world.com"
                };

                var userCreateResult = await userManager.CreateAsync(user, Password);
                // TODO: log result
                if (userCreateResult.Succeeded)
                {
                    user = await userManager.FindByNameAsync("Mili");
                    var roleAddResult = await userManager.AddToRoleAsync(user, AuthConstants.AdminRole);
                    // TODO: log result

                    var claimResult = await userManager.AddClaimsAsync(user, new List<Claim>
                    {
                        new Claim(AuthConstants.CompanyClaim, user.CompanyId.ToString())
                    });
                    // TODO: log result

                    claimResult = await userManager.AddClaimsAsync(user, new List<Claim>
                    {
                        new Claim(AuthConstants.UserTypeClaim, AuthConstants.AdministratorClaimType)
                    });
                    // TODO: log result
                }
            }

            existingUser = await userManager.FindByNameAsync("CRoberts");
            if (existingUser == null)
            {
                var user = new User
                {
                    UserName = "CRoberts",
                    CompanyId = companies[1].Id,
                    Email = "chrisroberts@rsi.com"
                };

                var userCreateResult = await userManager.CreateAsync(user, Password);
                // TODO: log result
                if (userCreateResult.Succeeded)
                {
                    user = await userManager.FindByNameAsync("CRoberts");

                    var claimResult = await userManager.AddClaimsAsync(user, new List<Claim>
                    {
                        new Claim(AuthConstants.CompanyClaim, user.CompanyId.ToString())
                    });
                    // TODO: log result
                }
            }

            existingUser = await userManager.FindByNameAsync("Gigel");
            if (existingUser == null)
            {
                var user = new User
                {
                    UserName = "Gigel",
                    CompanyId = companies[0].Id,
                    Email = "gigel@undeva.com"
                };

                var userCreateResult = await userManager.CreateAsync(user, Password);
                // TODO: log result
                if (userCreateResult.Succeeded)
                {
                    user = await userManager.FindByNameAsync("Gigel");
                    var claimResult = await userManager.AddClaimsAsync(user, new List<Claim>
                    {
                        new Claim(AuthConstants.CompanyClaim, user.CompanyId.ToString())
                    });
                    // TODO: log result
                }
            }
        }
    }
}