using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Derivco.Orniscient.Viewer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var isDocker = args != null && args.Contains("–host=0.0.0.0");

            Console.WriteLine("IsDocker = " + isDocker);
            
            var webHost = WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseDefaultServiceProvider(options => options.ValidateScopes = false);

            if (isDocker)
            {
                webHost.UseUrls($"http://{IPAddress.Any}:8080");
            }

            return webHost.Build();
        }
    }
}
