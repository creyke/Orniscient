using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Derivco.Orniscient.Viewer.Core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            StartKestrelWebHost();
        }

        private static void StartKestrelWebHost()
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .UseUrls("http://*:80")
                .Build();

            host.Run();
        }
    }
}
