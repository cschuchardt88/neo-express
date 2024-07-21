// Copyright (C) 2015-2024 The Neo Project.
//
// ExpressWalletAccountOptions.cs file belongs to the neo project and is free
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
    internal class ExpressWalletAccountOptions
    {
        public class AccountContract
        {
            [ConfigurationKeyName("script")]
            [JsonPropertyName("script")]
            public string Script { get; set; } = string.Empty;

            [ConfigurationKeyName("parameters")]
            [JsonPropertyName("parameters")]
            public List<string> Parameters { get; set; } = [];
        }

        [ConfigurationKeyName("private-key")]
        [JsonPropertyName("private-key")]
        public string PrivateKey { get; set; } = string.Empty;

        [ConfigurationKeyName("script-hash")]
        [JsonPropertyName("script-hash")]
        public string Address { get; set; } = string.Empty;

        [ConfigurationKeyName("label")]
        [JsonPropertyName("label")]
        public string? Label { get; set; }

        [ConfigurationKeyName("is-default")]
        [JsonPropertyName("is-default")]
        public bool IsDefault { get; set; }

        [ConfigurationKeyName("contract")]
        [JsonPropertyName("contract")]
        public AccountContract? Contract { get; set; }
    }
}
