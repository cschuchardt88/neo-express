// Copyright (C) 2015-2024 The Neo Project.
//
// ExpressWalletAccountConfig.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Text.Json.Serialization;

namespace Neo.Express.Models
{
    internal class ExpressWalletAccountOptions
    {
        public class AccountContract
        {
            [JsonPropertyName("script")]
            public string Script { get; set; } = string.Empty;

            [JsonPropertyName("parameters")]
            public List<string> Parameters { get; set; } = [];
        }

        [JsonPropertyName("private-key")]
        public string PrivateKey { get; set; } = string.Empty;

        [JsonPropertyName("script-hash")]
        public string Address { get; set; } = string.Empty;

        [JsonPropertyName("label")]
        public string? Label { get; set; }

        [JsonPropertyName("is-default")]
        public bool IsDefault { get; set; }

        [JsonPropertyName("contract")]
        public AccountContract? Contract { get; set; }
    }
}
