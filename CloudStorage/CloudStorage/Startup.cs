using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using CloudStorage.Services;
using CloudStorage.Services.Implementation;
using Microsoft.AspNetCore.Routing;
using Core.Services;
using Data;
using Microsoft.EntityFrameworkCore;
using Data.Seed;

namespace CloudStorage
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            Configuration = builder.Build();

        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSingleton(Configuration);
            services.AddSingleton<IGreeter, Greeter>();
            services.AddScoped<IFileData, SqlFileData>();
            services.AddDbContext<CloudStorageDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("CloudStorage")));
            services.AddTransient<CloudStorageSeedData>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(  IApplicationBuilder app, 
                                IHostingEnvironment env, 
                                ILoggerFactory loggerFactory,
                                IGreeter greeter,
                                CloudStorageSeedData seeder)
        {
            loggerFactory.AddConsole();
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(new ExceptionHandlerOptions
                {
                    ExceptionHandler = context => context.Response.WriteAsync("Ooops, something went wrong!")
                });
            }

            app.UseFileServer();

            app.UseMvc(ConfigureRoutes);

            app.Run(context => context.Response.WriteAsync($"Not found: {context.Request.Path}"));

            seeder.EnsureSeedData().Wait();
        }

        private void ConfigureRoutes(IRouteBuilder routeBuilder)
        {
            // GET /Home/Index

            routeBuilder.MapRoute("Default", "{controller=Home}/{action=Index}/{id?}");
        }
    }
}