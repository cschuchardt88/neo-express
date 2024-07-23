// Copyright (C) 2015-2024 The Neo Project.
//
// ExpressChainOptionsSetup.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Neo.Express.Models.Options;

namespace Neo.Express.Hosting.Setup
{
    internal sealed class ExpressChainOptionsSetup
        (IConfiguration configuration) : IConfigureOptions<ExpressChainOptions>
    {
        public void Configure(ExpressChainOptions options)
        {
            configuration.Bind(options);
        }
    }
}