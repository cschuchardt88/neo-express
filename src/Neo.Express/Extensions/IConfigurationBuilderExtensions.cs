// Copyright (C) 2015-2024 The Neo Project.
//
// IConfigurationBuilderExtensions.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Neo.Express.Hosting;
using Neo.Express.Hosting.Configuration;

namespace Neo.Express.Extensions
{
    internal static class IConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddNeoExpressConfiguration(this IConfigurationBuilder builder)
        {
            builder.AddInMemoryCollection(
                [
                    new(HostDefaults.ContentRootKey, Environment.CurrentDirectory),
                ]);

            return builder;
        }

        public static IConfigurationBuilder AddNeoExpressDefaultFiles(this IConfigurationBuilder builder, Func<string> getInputFilename)
        {
            try
            {
                builder.SetBasePath(NeoExpressConfigurationDefaults.BaseDirectory);
                builder.AddJsonFile(NeoExpressConfigurationDefaults.ProgramConfigFilename, optional: true);

                builder.SetBasePath(Environment.CurrentDirectory);
                builder.AddJsonFile(getInputFilename(), optional: false);
                builder.Add(new NeoExpressConfigurationSource());
            }
            catch (FileNotFoundException)
            {
                throw;
            }

            return builder;
        }
    }
}
