// Copyright (C) 2015-2024 The Neo Project.
//
// ExpressChainOptions.cs file belongs to the neo project and is free
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
    internal class ExpressChainOptions
    {
        [ConfigurationKeyName("magic")]
        [JsonPropertyName("magic")]
        public uint Network { get; set; }

        [ConfigurationKeyName("address-version")]
        [JsonPropertyName("address-version")]
        public byte AddressVersion { get; set; } = ProtocolSettings.Default.AddressVersion;

        [ConfigurationKeyName("consensus-nodes")]
        [JsonPropertyName("consensus-nodes")]
        public IList<ExpressConsensusNodeOptions> ConsensusNodes { get; set; } = [];

        [ConfigurationKeyName("wallets")]
        [JsonPropertyName("wallets")]
        public IList<ExpressWalletOptions> Wallets { get; set; } = [];

        [ConfigurationKeyName("settings")]
        [JsonPropertyName("settings")]
        public Dictionary<string, string> Settings { get; set; } = new(StringComparer.InvariantCultureIgnoreCase);
    }
}
