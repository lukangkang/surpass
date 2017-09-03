using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Surpass.Database;
using Surpass.Domain.Entities;
using Surpass.Infrastructure.Database;

namespace Surpass.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddLogging().AddLocalization();

            //services.AddDbContext<EFCoreDbContext>(options =>
            //    options.UseSqlServer(Configuration.GetConnectionString("EFCoreDbContext")));

            //services.AddIdentity<User, IdentityRole>()
            //    .AddEntityFrameworkStores<EFCoreDbContext>()
            //    .AddDefaultTokenProviders();

            services.AddMvc();
            services.AddLogging();

            services.AddOptions();
            services.Configure<List<DatabaseOption>>(Configuration.GetSection("OtherDatabases"));

            return services.BuildServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
