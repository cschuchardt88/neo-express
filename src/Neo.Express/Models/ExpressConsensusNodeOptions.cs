// Copyright (C) 2015-2024 The Neo Project.
//
// ExpressConsensusNodeConfig.cs file belongs to the neo project and is free
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
    internal class ExpressConsensusNodeOptions
    {
        [JsonPropertyName("tcp-port")]
        public ushort TcpPort { get; set; }

        [JsonPropertyName("rpc-port")]
        public ushort RpcPort { get; set; }

        [JsonPropertyName("wallet")]
        public ExpressWalletOptions Wallet { get; set; } = new();
    }
}