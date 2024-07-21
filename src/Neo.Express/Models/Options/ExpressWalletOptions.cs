// Copyright (C) 2015-2024 The Neo Project.
//
// ExpressWalletOptions.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.Extensions.Configuration;
using System.Text.Json.Serialization;

namespace Neo.Express.Models.Options
{
    internal class ExpressWalletOptions
    {
        [ConfigurationKeyName("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [ConfigurationKeyName("accounts")]
        [JsonPropertyName("accounts")]
        public IList<ExpressWalletAccountOptions> Accounts { get; set; } = [];
    }
}
