// Copyright (C) 2015-2024 The Neo Project.
//
// Program.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Express.Command;
using Neo.Express.Command.Handler;
using Neo.Express.Extensions;
using Neo.Express.Hosting;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;

namespace Neo.Express
{
    internal sealed partial class Program
    {
        private static async Task<int> Main(string[] args)
        {
            var rootCommand = new ProgramCommand();
            var parser = new CommandLineBuilder(rootCommand)
                .UseNeoExpressHost(NeoExpressHostBuilderFactory, builder =>
                {
                    builder.UseNeoExpressHostConfiguration();
                    builder.UseNeoExpressAppConfiguration(args);
                    builder.UseCommandHandler<ProgramCommand, EmptyHandler>();
                })
                .UseDefaults()
                .UseExceptionHandler(CommandLineExceptionFilter.Handler)
                .Build();

            return await parser.InvokeAsync(args);
        }
    }
}
