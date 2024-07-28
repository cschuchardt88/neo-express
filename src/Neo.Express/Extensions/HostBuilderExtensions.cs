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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neo.Express.Hosting.Services;
using Neo.Express.Hosting.Setup;
using System.CommandLine.Invocation;
using System.Diagnostics.CodeAnalysis;

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
            _ = hostBuilder.TryGetGlobalInputOption(out var inputFilename);

            hostBuilder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddNeoExpressConfiguration();
                config.AddNeoExpressDefaultFiles(() => inputFilename);
            });

            return hostBuilder;
        }

        public static IHostBuilder UseNeoExpressServices(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((context, services) =>
            {
                services.ConfigureOptions<ExpressChainOptionsSetup>();
                services.ConfigureOptions<ExpressApplicationOptionsSetup>();
                services.AddHostedService<NeoSystemHostedService>();
            });

            return hostBuilder;
        }

        public static bool TryGetGlobalInputOption(this IHostBuilder hostBuilder, [NotNullWhen(true)] out string? filename)
        {
            var invocation = hostBuilder.Properties[typeof(InvocationContext)] as InvocationContext;
            var command = invocation!.ParseResult.CommandResult.Command;
            var inputOptions = command.Options.FirstOrDefault(f => f.Name == "input");

            filename = null;

            if (inputOptions != null)
                filename = invocation.ParseResult.GetValueForOption(inputOptions) as string;

            return filename != null;
        }
    }
}
