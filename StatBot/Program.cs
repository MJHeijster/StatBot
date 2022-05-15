// ***********************************************************************
// Assembly         : StatBot
// Author           : Jeroen Heijster
// Created          : 12-11-2017
//
// Last Modified By : Jeroen Heijster
// Last Modified On : 15-05-2022
// ***********************************************************************
// <copyright file="Program.cs">
//     Copyright ©  2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace StatBot
{
    /// <summary>
    /// Class Program.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        static void Main() => new Program().MainAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Starts the application
        /// </summary>
        /// <returns>The task.</returns>
        public Task MainAsync()
        {
            Console.WriteLine("Starting...");
            var host = CreateDefaultBuilder().Build();
            using IServiceScope serviceScope = host.Services.CreateScope();
            IServiceProvider provider = serviceScope.ServiceProvider;
            var workerInstance = provider.GetRequiredService<Worker>();
            workerInstance.DoWork();
            var mircStatsGeneratorInstance = provider.GetRequiredService<mIRCStatsGenerator>();
            mircStatsGeneratorInstance.DoWork();
            host.Run();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Creates the default builder.
        /// </summary>
        /// <returns>IHostBuilder.</returns>
        static IHostBuilder CreateDefaultBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(app =>
                {
#if DEBUG
                    app.AddJsonFile("appsettings.dev.json");
#else
                    app.AddJsonFile("appsettings.json");
#endif

                })
                .ConfigureServices(services =>
                {
                    services.AddSingleton<mIRCStatsGenerator>();
                    services.AddSingleton<Worker>();
                });
        }

    }
}
