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

using Microsoft.Extensions.Configuration;
using Neo.Express.Hosting;
using Neo.Express.Models.Options;
using Neo.Plugins.RpcServer;
using Neo.SmartContract.Native;
using System.Net;

namespace Neo.Express.Extensions
{
    internal static class ExpressChainOptionsExtensions
    {
        public static ProtocolSettings GetProtocolSettings(this ExpressChainOptions expressChainOptions) =>
            ProtocolSettings.Default with
            {
                Network = expressChainOptions.Network,
                AddressVersion = expressChainOptions.AddressVersion,
                MillisecondsPerBlock = 5000,
                ValidatorsCount = expressChainOptions.ConsensusNodes.Count,
                StandbyCommittee = expressChainOptions.ConsensusNodes
                    .Select(s => s.GetKeyPair().PublicKey)
                    .ToArray(),
                SeedList = expressChainOptions.ConsensusNodes
                    .Select(s => $"{IPAddress.Loopback}:{s.TcpPort}")
                    .ToArray(),
            };

        public static RpcServerSettings GetRpcServerSettings(this ExpressChainOptions expressChainOptions) =>
            RpcServerSettings.Default with
            {
                Network = expressChainOptions.Network,
                BindAddress = IPAddress.Parse(expressChainOptions.Settings[RpcServerSettingsKeyNames.BindAddress]),
                Port = expressChainOptions.ConsensusNodes[0].RpcPort,
                MaxGasInvoke = (long)new BigDecimal(decimal.Parse(expressChainOptions.Settings[RpcServerSettingsKeyNames.MaxGasInvoke]), NativeContract.GAS.Decimals).Value,
                MaxFee = (long)new BigDecimal(decimal.Parse(expressChainOptions.Settings[RpcServerSettingsKeyNames.MaxFee]), NativeContract.GAS.Decimals).Value,
                MaxIteratorResultItems = int.Parse(expressChainOptions.Settings[RpcServerSettingsKeyNames.MaxIteratorResultItems]),
                SessionEnabled = bool.Parse(expressChainOptions.Settings[RpcServerSettingsKeyNames.SessionEnabled])
            };

        public static Plugins.DBFTPlugin.Settings GetConsensusSettings(this ExpressChainOptions expressChainOptions)
        {
            var settings = new Dictionary<string, string?>()
            {
                ["PluginConfiguration:Network"] = $"{expressChainOptions.Network}",
                ["IgnoreRecoveryLogs"] = "true",
            };

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();

            return new(config.GetSection("PluginConfiguration"));
        }
    }
}
