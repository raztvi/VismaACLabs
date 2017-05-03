using System;
using CloudStorage.Services;
using CloudStorage.Services.Implementation;
using Core.Constants;
using Core.Entities;
using Core.Services;
using Data;
using Data.Seed;
using Data.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using CloudStorage.Seed;
using CloudStorage.Filters;

namespace CloudStorage
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddMvc(options =>
                {
                    options.Filters.Add(typeof(CustomExceptionFilterAttribute));
                })
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            services.AddSignalR(options => options.Hubs.EnableDetailedErrors = true);

            services.AddSingleton(Configuration);
            services.AddSingleton<IGreeter, Greeter>();
            services.AddScoped<IFileData, SqlFileData>();
            services.AddScoped<IBlobService, AzureBlobService>();
            services.AddScoped<ICompanyData, SqlCompanyData>();

            services.AddDbContext<CloudStorageDbContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("CloudStorage")));

            services.AddTransient<CloudStorageSeedData>();

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<CloudStorageDbContext>();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 6;

                // Cookie settings
                options.Cookies.ApplicationCookie.ExpireTimeSpan = TimeSpan.FromDays(1);
                options.Cookies.ApplicationCookie.LoginPath = "/Account/Login";
                options.Cookies.ApplicationCookie.LogoutPath = "/Account/Logout";

                // User settings
                options.User.RequireUniqueEmail = true;
            });
            services.AddAuthorization(
                options =>
                {
                    options.AddPolicy(AuthConstants.AdministratorClaimPolicy,
                        policy =>
                        {
                            policy.RequireClaim(AuthConstants.UserTypeClaim, AuthConstants.AdministratorClaimType);
                        });
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            IGreeter greeter,
            CloudStorageSeedData seeder)
        {
            loggerFactory.AddConsole().AddDebug();

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler(new ExceptionHandlerOptions
                {
                    ExceptionHandler = context => context.Response.WriteAsync("Ooops, something went wrong!")
                });

            app.UseFileServer();

            app.UseIdentity();

            app.UseMvc(ConfigureRoutes);

            app.UseWebSockets();

            app.UseSignalR();

            // Seed initial data
            seeder.EnsureSeedData().Wait();
            UsersAndRolesSeeder.SeedUsersAndRoles(app.ApplicationServices).Wait();
        }

        private void ConfigureRoutes(IRouteBuilder routeBuilder)
        {
            // GET /Home/Index

            routeBuilder.MapRoute("Default", "{controller=Home}/{action=Index}/{id?}");
        }
    }
}