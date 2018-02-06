using Orleans;
using System;
using System.ServiceProcess;
using TestGrains.Grains;
using TestHost;

namespace Derivco.Orniscient.TestHost
{
    partial class OrleansService : ServiceBase
    {
        private static OrleansHostWrapper _hostWrapper;

        public OrleansService()
        {
            ServiceName = "SpinSport.Services.Sports.Sandbox";
            InitializeComponent();
        }

        private static AppDomain CreateHostDomain(string[] args)
        {
            var hostDomain = AppDomain.CreateDomain("OrleansHost", null, new AppDomainSetup
            {
                AppDomainInitializer = InitSilo,
                AppDomainInitializerArguments = args
            });

            return hostDomain;
        }

        public static void StartManually(string[] args)
        {
            try
            {
                var hostDomain = CreateHostDomain(args);

                GrainClient.Initialize(AppDomain.CurrentDomain.BaseDirectory + "\\DevTestClientConfiguration.xml");

                Start();

                Console.WriteLine("Orleans Silo is running.\nPress Enter to terminate...");
                Console.ReadLine();

                hostDomain.DoCallBack(ShutdownSilo);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }

        private static void Start()
        {
            GrainClient.GrainFactory.GetGrain<IFirstGrain>("Hallo").KeepAlive();
        }

        protected override void OnStart(string[] args)
        {
            var hostDomain = CreateHostDomain(args);
            GrainClient.Initialize(AppDomain.CurrentDomain.BaseDirectory + "\\DevTestClientConfiguration.xml");
            hostDomain.DoCallBack(ShutdownSilo);
            Start();
        }

        protected override void OnStop()
        {
            _hostWrapper?.Stop();
            ShutdownSilo();
        }

        private static void InitSilo(string[] args)
        {
            _hostWrapper = new OrleansHostWrapper(args);

            if (!_hostWrapper.Run())
            {
                Console.Error.Write("Failed to initialize Orleans silo");
            }
        }

        private static void ShutdownSilo()
        {
            if (_hostWrapper != null)
            {
                _hostWrapper.Dispose();
                GC.SuppressFinalize(_hostWrapper);
            }
        }
    }
}
