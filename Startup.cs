using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assignment_5___Jackson_vdw.Models;
using Microsoft.AspNetCore.Http;

namespace Assignment_5___Jackson_vdw
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            //Add connection string to allow database connection
            services.AddDbContext<BookDbContext>(options =>
            {
                options.UseSqlite(Configuration["ConnectionStrings:BookConnection"]);
            });

            //add scoped to assist with database creation and repository
            services.AddScoped<IBookRepository, EFBookRepository>();

            //allows you to use razor pages
            services.AddRazorPages();

            //setting up session storage
            services.AddDistributedMemoryCache();
            services.AddSession();

            //create a service for the Cart class
            //goal is to satisfy requests for Cart objects with SessionCart objects that will seamlessly store themselves.
            services.AddScoped<Cart>(sp => SessionCart.GetCart(sp));
            //specifies that the same object should always be used
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            //allows you to use session storage
            app.UseSession();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //if they type a category and page into the URL
                endpoints.MapControllerRoute(
                    "catpage",
                    "{category}/{pageNum:int}",
                    new { Controller = "Home", Action = "Index" });

                //if they only type the page number
                endpoints.MapControllerRoute(
                    "pageNum",
                    "{pageNum:int}",
                    new { Controller = "Home", Action = "Index" });

                //if they type a category into the URL. Set the page to 1 since the user didn't provide it
                endpoints.MapControllerRoute(
                    "category",
                    "{category}",
                    new { Controller = "Home", Action = "Index", pageNum = 1 });

                //if they type projects/page into the URL
                endpoints.MapControllerRoute(
                    "pagination",
                    "Projects/{pageNum}",
                    new { Controller = "Home", Action = "Index" });

                //if what comes in doesn't match anything, use the default route setup (Home -> Index)
                endpoints.MapDefaultControllerRoute();

                //allows endpoints to use razor pages (add routing for razor pages)
                endpoints.MapRazorPages();
            });

            //goes to the SeedData class, calls the EnsurePopulated method
            //decide if there is anything in the database and make decisions based on that
            SeedData.EnsurePopulated(app);
        }
    }
}
