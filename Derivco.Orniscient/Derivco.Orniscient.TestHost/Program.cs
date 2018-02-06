using System;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using Derivco.Orniscient.TestHost;

namespace TestHost
{
    class Program
    {
        private static OrleansHostWrapper hostWrapper;

        static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));

            if (Environment.UserInteractive)
            {
                OrleansService.StartManually(args);
            }
            else
            {
                var servicesToRun = new ServiceBase[] { new OrleansService() };
                ServiceBase.Run(servicesToRun);
            }
        }

        private static void InitSilo(string[] args)
        {
            hostWrapper = new OrleansHostWrapper(args);

            if (!hostWrapper.Run())
            {
                Console.Error.WriteLine("Failed to initialize Orleans silo");
            }
        }

        private static void ShutdownSilo()
        {
            if (hostWrapper != null)
            {
                hostWrapper.Dispose();
                GC.SuppressFinalize(hostWrapper);
            }
        }
    }
}
