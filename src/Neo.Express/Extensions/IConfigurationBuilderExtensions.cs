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

            builder.SetBasePath(NeoExpressConfigurationDefaults.BaseDirectory);

            return builder;
        }
    }
}
