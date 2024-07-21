// Copyright (C) 2015-2024 The Neo Project.
//
// Program.Configuration.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Neo.Express
{
    internal partial class Program
    {
        private static IHostBuilder NeoExpressHostBuilderFactory(string[] args) =>
            new HostBuilder()
            .UseServiceProviderFactory(context => new DefaultServiceProviderFactory());
    }
}
