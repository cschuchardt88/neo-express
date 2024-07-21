// Copyright (C) 2015-2024 The Neo Project.
//
// ExpressChainOptionsExtensions.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Express.Models.Options;
using System.Net;

namespace Neo.Express.Extensions
{
    internal static class ExpressChainOptionsExtensions
    {
        public static ProtocolSettings GetProtocolSettings(this ExpressChainOptions expressChainOptions)
        {
            return ProtocolSettings.Default with
            {
                Network = expressChainOptions.Network,
                AddressVersion = expressChainOptions.AddressVersion,
                MillisecondsPerBlock = 5000,
                ValidatorsCount = expressChainOptions.ConsensusNodes.Count,
                StandbyCommittee = expressChainOptions.ConsensusNodes
                    .Select(s => s.GetPublicKey().PublicKey)
                    .ToArray(),
                SeedList = expressChainOptions.ConsensusNodes
                    .Select(s => $"{IPAddress.Loopback}:{s.TcpPort}")
                    .ToArray(),
            };
        }
    }
}
