using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.UI.Services;
using DeliverNET.Data;
using System.Security.Claims;
using DeliverNET.Comms;
using DeliverNET.Services;
using Microsoft.Extensions.Logging;
using DeliverNET.Models;
using Microsoft.AspNetCore.Authentication;
using DeliverNET.Comms.Hubs;
using DeliverNET.Infrastructure.Account;
using DeliverNET.Managers;
using DeliverNET.Managers.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Hosting;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace DeliverNET
{
    public class Startup  // TODO :: Delete all useless "using" from .cs files (optional & OCD)
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            
            // TODO Strict password requirements
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            });

            services.AddDbContext<DeliverNETContext>(options =>
                    options.UseSqlServer(
                        Configuration.GetConnectionString("DeliverNETConnection")));

            services.AddIdentity<DeliverNETUser, IdentityRole>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<DeliverNETContext>()
            .AddDefaultTokenProviders();

            // Add managers as services
            services.AddTransient<IOrderManager, OrderManager>();
            services.AddTransient<IDelivererManager, DelivererManager>();
            services.AddTransient<IBusinessManager, BusinessManager>();
            services.AddTransient<IBusinessCashierManager, BusinessCashierManager>();
            services.AddTransient<IBusinessOwnerManager, BusinessOwnerManager>();
            services.AddTransient<IMasterManager, MasterManager>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddAuthentication().AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
                facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("BusiOnly", policy => policy.RequireClaim("JobType", new string[] { "Businessman", "Cashier" }));
                options.AddPolicy("OwnerOnly", policy => policy.RequireClaim("JobType", "Businessman"));
                options.AddPolicy("IndividualOnly", policy => policy.RequireClaim("JobType", "Individual"));
            });

            services.AddSignalR();

            services.AddSingleton<IEmailSender, MailSender>();
            services.AddTransient<DeliverNETClaimManager>();
            services.AddTransient<Seeder>();

            // Add hosted services
            services.AddSingleton<IHostedService, OrderTimeWatcher>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, Seeder seeder)
        {
            seeder.Seed().Wait();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseSignalR(route =>
                route.MapHub<MainHub>("/Comms/Hubs/MainHub")
            );

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });


        }
    }
}
