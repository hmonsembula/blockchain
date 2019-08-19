using Blockchain_Eligibility.Service.Repositories;
using Blockchain_Eligibility.Service.Settings;
using Blockchain_Eligibility.Service.System;
using Blockchain_UserJourney.ActorSystem;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Blockchain_UserJourney
{
    class Program
    {
        static void Main(string[] args)
        {
            var isService = !(Debugger.IsAttached || args.Contains("--console"));

            if (isService)
            {
                var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
                var pathToContentRoot = Path.GetDirectoryName(pathToExe);
                Directory.SetCurrentDirectory(pathToContentRoot);
            }

            var builder = CreateWebHostBuilder(args.Where(arg => arg != "--console").ToArray());

            using (var host = builder.Build())
            {
                if (isService)
                {
                    host.RunAsService();
                }
                else
                {
                    host.Run();
                }
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .ConfigureLogging((logging) =>
            {

            })
            .ConfigureAppConfiguration((context, config) =>
            {

            })
            .ConfigureServices(serviceCollection =>
            {
                var blockChainCollectionName = "blockchain_insession";
                var eligibilityDb = "eligibility";
                var connection = "mongodb://localhost:27017/?connectTimeoutMS=10000&3t.uriVersion=3&3t.connection.name=localhost";

                var eligibilityBlockRepoSettings = new EligibilityBlockRepositorySettings
                {
                    CollectionName = blockChainCollectionName,
                    DataBaseName = eligibilityDb,
                    ConnectionString = connection
                };

                var eligibilityRepoSettings = new EligibilityRepositorySettings
                {
                    CollectionName = "daily_deal",
                    DataBaseName = eligibilityDb,
                    ConnectionString = connection
                };

                var experienceBlockRepoSettings = new ExperienceBlockRepositorySettings
                {
                    CollectionName = blockChainCollectionName,
                    DataBaseName = "player_experience",
                    ConnectionString = connection
                };

                var creditBlockRepoSettings = new CreditBlockRepositorySettings
                {
                    CollectionName = blockChainCollectionName,
                    DataBaseName = "credit",
                    ConnectionString = connection
                };

                serviceCollection
                .AddTransient<IEligibilityBlockRepositorySettings>(s => eligibilityBlockRepoSettings)
                .AddTransient<IEligibilityRepositorySettings>(s => eligibilityRepoSettings)
                .AddTransient<IExperienceBlockRepositorySettings>(s => experienceBlockRepoSettings)
                .AddTransient<ICreditBlockRepositorySettings>(s => creditBlockRepoSettings)

                .AddSingleton<IInsessionBlockchain, InsessionBlockchain>()

                .AddSingleton<IEligibilitySystem, EligibilitySystem>()
                .AddSingleton<IEligibilityBlockRepository, EligibilityBlockRepository>()
                .AddSingleton<IEligibilityRepository, EligibilityRepository>()
                .AddSingleton<IExperienceBlockRepository, ExperienceBlockRepository>()
                .AddSingleton<ICreditBlockRepository, CreditBlockRepository>()

                .AddLogging()


                .BuildServiceProvider();
            })
            .UseStartup<Startup>();
    }
}
