﻿using System;
using System.Collections.Generic;
using System.Linq;
using ExchangeRateUpdater.Services;
using ExchangeRateUpdater.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using System.IO;

namespace ExchangeRateUpdater
{
    public static class Program
    {
        private static IEnumerable<Currency> currencies = new[]
        {
            new Currency("USD"),
            new Currency("EUR"),
            new Currency("CZK"),
            new Currency("JPY"),
            new Currency("KES"),
            new Currency("RUB"),
            new Currency("THB"),
            new Currency("TRY"),
            new Currency("XYZ")
        };

        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            var rates = host.Services.GetService<IExchangeRateProvider>().GetExchangeRates(currencies, System.Threading.CancellationToken.None).GetAwaiter().GetResult();
        
            foreach (var rate in rates)
            {
                Console.WriteLine(rate.ToString());
            }
        }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {

        var hostBuilder = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, builder) =>
            {
                builder.SetBasePath(Directory.GetCurrentDirectory());
            })
            .ConfigureServices((context, services) =>
            {
                // Remove httpClient default Console.WriteLine behaviour
                services.RemoveAll<IHttpMessageHandlerBuilderFilter>();

                // Services
                services.AddSingleton<IExchangeRateProvider, ExchangeRateProvider>();
                services.AddSingleton<ICnbApiService, CnbApiService>();

                // CnbHttpClient
                services.AddHttpClient("CnbClient", x => 
                {
                    x.BaseAddress = new Uri("https://api.cnb.cz");
                });
            });

        return hostBuilder;
    }
    }
}