using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CityAPI.Contexts;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.LayoutRenderers;
using NLog.Web;

namespace CityAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

            try
            {
                logger.Info("initializing application...");
                var host = CreateWebHostBuilder(args).Build();

                    using (var scope = host.Services.CreateScope())
                    {
                        try
                        {
                            var context = scope.ServiceProvider.GetService<CityContext>();
                            context.Database.EnsureDeleted();
                            context.Database.Migrate();
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex, "An error occurred while migrating the database");
                        }
                    }

                host.Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "application failed to start");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseNLog();
    }
}
