using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;

namespace Yggdrasil
{
    public class Startup
    {

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddMvcCore();
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
