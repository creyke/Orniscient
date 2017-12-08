using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Derivco.Orniscient.Viewer
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
