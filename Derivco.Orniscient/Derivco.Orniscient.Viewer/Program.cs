using System;
using System.IO;
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
            return WebHost.CreateDefaultBuilder(args)
                .UseUrls($"http://{IPAddress.Any}:8080")
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseDefaultServiceProvider(options => options.ValidateScopes = false)
                .Build();
        }
    }
}
