// ***********************************************************************
// Assembly         : StatBot
// Author           : Jeroen Heijster
// Created          : 12-11-2017
//
// Last Modified By : Jeroen Heijster
// Last Modified On : 13-05-2022
// ***********************************************************************
// <copyright file="Program.cs" company="Jeroen Heijster">
//     Copyright ©  2017
// </copyright>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using System.IO;
using Discord.WebSocket;
using StatBot.PushoverMessaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        public async Task MainAsync()
        {
            Console.WriteLine("Starting...");
            var host = CreateDefaultBuilder().Build();
            using IServiceScope serviceScope = host.Services.CreateScope();
            IServiceProvider provider = serviceScope.ServiceProvider;
            var workerInstance = provider.GetRequiredService<Worker>();
            workerInstance.DoWork();
            host.Run();
            

        }

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
                    services.AddSingleton<Worker>();
                });
        }

    }
}
