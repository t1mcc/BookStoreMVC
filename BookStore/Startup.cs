using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BookStore.Data;
using BookStore.Models;
using BookStore.Models.Cart;
using BookStore.Repositories;
using BookStore.Repositories.Interfaces;
using BookStore.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BookStore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<BookStoreDbContext>(config => 
                    config.UseSqlServer(
                        Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<User, IdentityRole>(config =>
                {
                    config.Password.RequiredLength = 6;
                    config.Password.RequireUppercase = false;
                    config.Password.RequireDigit = false;
                    config.Password.RequireNonAlphanumeric = false;
                    config.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<BookStoreDbContext>();
                //.AddDefaultTokenProviders();

            services.AddControllersWithViews();
            
            services.AddDistributedMemoryCache();
            services.AddSession();

            services.AddAutoMapper(typeof(MapperProfiles));

            services.AddScoped<Cart>(x => CartService.GetCart(x));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<IBookRepository, EFBookRepository>();
            services.AddScoped<IOrderRepository, EFOrderRepository>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSession();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
