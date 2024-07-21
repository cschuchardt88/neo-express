// Copyright (C) 2015-2024 The Neo Project.
//
// CommandLineExtensions.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Invocation;

namespace Neo.Express.Extensions
{
    internal static class CommandLineExtensions
    {
        private const string ConfigurationDirectiveName = "config";

        internal static CommandLineBuilder UseNeoExpressHost(this CommandLineBuilder builder,
            Func<string[], IHostBuilder> hostBuilderFactory,
            Action<IHostBuilder>? configureHost = null) =>
            builder.AddMiddleware(async (invocation, next) =>
            {
                var argsRemaining = invocation.ParseResult.UnparsedTokens.ToArray();
                var hostBuilder = hostBuilderFactory?.Invoke(argsRemaining)
                    ?? new HostBuilder();

                hostBuilder.Properties[typeof(InvocationContext)] = invocation;

                hostBuilder.ConfigureHostConfiguration(config =>
                {
                    config.AddCommandLineDirectives(invocation.ParseResult, ConfigurationDirectiveName);
                });

                hostBuilder.ConfigureServices(services =>
                {
                    services.AddSingleton(invocation);
                    services.AddSingleton(invocation.BindingContext);
                    services.AddSingleton(invocation.Console);
                    services.AddTransient(_ => invocation.InvocationResult!);
                    services.AddTransient(_ => invocation.ParseResult);
                });

                hostBuilder.UseInvocationLifetime(invocation);
                configureHost?.Invoke(hostBuilder);

                using var host = hostBuilder.Build();

                invocation.BindingContext.AddService(typeof(IHost), _ => host);

                await next(invocation);
            });
    }
}
