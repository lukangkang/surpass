using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Surpass.Database;
using Surpass.Plugin;

namespace Surpass.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// Get website root directory<br/>
        /// 获取网站的根目录<br/>
        /// </summary>
        /// <returns></returns>
        private static string GetWebsiteRootDirectory()
        {
            var path = Path.GetDirectoryName(typeof(Program).Assembly.Location);
            while (!Directory.Exists(Path.Combine(path, "App_Data")))
            {
                path = Path.GetDirectoryName(path);
                if (string.IsNullOrEmpty(path))
                {
                    throw new DirectoryNotFoundException("Website root directory not found");
                }
            }
            return path;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddLocalization();
            services.AddLogging(builder =>
            {
                builder
                    .AddConfiguration(Configuration.GetSection("Logging"))
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddConsole();
            });

            //services.AddDbContext<EFCoreDbContext>(options =>
            //    options.UseSqlServer(Configuration.GetConnectionString("EFCoreDbContext")));

            //services.AddIdentity<User, IdentityRole>()
            //    .AddEntityFrameworkStores<EFCoreDbContext>()
            //    .AddDefaultTokenProviders();
            var a = new TokenValidationParameters();
            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            //{
            //    options.TokenValidationParameters=
            //})

            services.AddMvc();
            services.AddLogging();

            services.AddOptions();
            services.Configure<DatabaseOptions>(Configuration.GetSection("Databases"));
            services.Configure<PluginOptions>(Configuration.GetSection("Plugins"));

            Application.Initialize(services, GetWebsiteRootDirectory());

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
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
