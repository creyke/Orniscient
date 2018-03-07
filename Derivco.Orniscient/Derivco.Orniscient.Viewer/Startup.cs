using System;
using System.IO;
using Derivco.Orniscient.Viewer.Hubs;
using Derivco.Orniscient.Viewer.Observers;
using JavaScriptEngineSwitcher.ChakraCore;
using JavaScriptEngineSwitcher.Core;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using React.AspNet;

namespace Derivco.Orniscient.Viewer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);


            Configuration = builder.Build();
        }
        
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Adds services required for using options.
            services.AddOptions();

            services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(Environment.CurrentDirectory)).DisableAutomaticKeyGeneration();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();

            // Register the IConfiguration instance which MyOptions binds against.
            services.Configure<IConfiguration>(Configuration);


            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSignalR();
            services.AddScoped<OrniscientHub>();
            services.AddSingleton<OrniscientObserver>();

            services.AddReact();

            services.AddMvc();
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

            var engineSwitcher = JsEngineSwitcher.Instance;
            engineSwitcher.DefaultEngineName = ChakraCoreJsEngine.EngineName;
            engineSwitcher.EngineFactories
                .Add(new ChakraCoreJsEngineFactory());

            app.UseReact(config =>
                config
                    .SetReuseJavaScriptEngines(true)
                    .SetUseDebugReact(true));

            app.UseStaticFiles();
            app.UseFileServer();


            app.UseSignalR(routes =>
            {
                routes.MapHub<OrniscientHub>("orniscientHub");
            });

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Connection}/{action=Index}");
            });
        }
    }
}
