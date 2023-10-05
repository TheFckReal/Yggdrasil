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
                    //options.AddEnvironmentVariables();
                });
            });
        //}).ConfigureAppConfiguration((context, config) =>
        //{
        //    //config.AddJsonFile("conf.json");
        //});

        //static IHostBuilder CreateHostBuilder(string[] args)
        //{
        //    var builder = new HostBuilder()
        //        .UseContentRoot(Directory.GetCurrentDirectory())
        //        .ConfigureHostConfiguration(config =>
        //        {

        //        })
        //        .ConfigureAppConfiguration((context, config) =>
        //        {

        //        })
        //        .ConfigureLogging((context, options) =>
        //        {
        //            options.AddConfiguration(context.Configuration.GetSection("Logging"));
        //            options.AddConsole();
        //            options.AddDebug();
        //        })
        //        .UseDefaultServiceProvider((context, options) =>
        //        {
        //            var isDevelopment = context.HostingEnvironment.IsDevelopment();
        //            options.ValidateOnBuild = isDevelopment;
        //            options.ValidateScopes = isDevelopment;
        //        });
        //    return builder;
        //}
    }
}