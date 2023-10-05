using Microsoft.AspNetCore.Server.IIS.Core;

namespace Yggdrasil
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
                webBuilder.ConfigureAppConfiguration((context, options) =>
                {

                });
            });
    }
}