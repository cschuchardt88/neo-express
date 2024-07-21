// Copyright (C) 2015-2024 The Neo Project.
//
// HostBuilderExtensions.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neo.Express.Hosting;
using Neo.Express.Hosting.Configuration;
using Neo.Express.Models.Options;

namespace Neo.Express.Extensions
{
    internal static class HostBuilderExtensions
    {
        public static IHostBuilder UseNeoExpressHostConfiguration(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureHostConfiguration(config =>
            {
                config.AddNeoExpressConfiguration();
            });

            return hostBuilder;
        }

        public static IHostBuilder UseNeoExpressAppConfiguration(this IHostBuilder hostBuilder, string[]? args = null)
        {
            hostBuilder.ConfigureAppConfiguration((context, config) =>
            {
                if (args != null)
                    config.AddCommandLine(args);

                config.AddNeoExpressConfiguration();

                try
                {
                    config.AddJsonFile(NeoExpressConfigurationDefaults.ProgramConfigFilename, optional: true);
                    config.AddJsonFile(NeoExpressConfigurationDefaults.ExpressConfigFilename, optional: false);
                    config.Add(new NeoExpressConfigurationSource());
                }
                catch (FileNotFoundException)
                {
                    throw;
                }
            });

            return hostBuilder;
        }

        public static IHostBuilder UseNeoExpressServices(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((context, services) =>
            {
                services.ConfigureOptions<ExpressChainOptionsSetup>();
            });

            return hostBuilder;
        }
    }
}
