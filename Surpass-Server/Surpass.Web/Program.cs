using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Surpass.Web
{
    public class Program
    {
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

        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(new ConfigurationBuilder().SetBasePath(GetWebsiteRootDirectory())
                    .AddJsonFile("appsettings.json", true)
                    .Build()).ConfigureServices(
                    x =>
                    {
                        Application.Initialize(GetWebsiteRootDirectory());
                    }).Configure(app =>
                    {
                        var env = new HostingEnvironment();
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
                    })
                    //.UseStartup<Startup>()
                    .Build();
    }
}
