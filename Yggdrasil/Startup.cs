using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Yggdrasil.DbModels;
using Yggdrasil.OptionsModels;
using Yggdrasil.Services;
using File = System.IO.File;

namespace Yggdrasil
{
    public class Startup
    {
        public Startup(IHostEnvironment env)
        {
            var builder = new ConfigurationBuilder().SetBasePath(env.ContentRootPath).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            Configuration = builder.Build();
        }
        public IConfiguration Configuration {get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.Configure<FileSettings>(Configuration.GetSection(nameof(FileSettings)));
            services.Configure<RouteOptions>(Configuration.GetSection(nameof(RouteOptions)));
            services.AddDbContext<StudyDbContext>();
            services.AddScoped<IHomeworkService, HomeworkService>();
            services.AddScoped<IFileService,  FileService>();
            services.AddHttpClient();
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();


            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseFileServer(new FileServerOptions()
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(env.ContentRootPath, "node_modules")
                ),
                RequestPath = "/node_modules",
                EnableDirectoryBrowsing = false
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
