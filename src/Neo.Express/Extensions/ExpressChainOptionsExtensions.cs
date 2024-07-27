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
        public static ProtocolSettings GetProtocolSettings(this ExpressChainOptions expressChainOptions, uint millisecondsPerBlock) =>
            ProtocolSettings.Default with
            {
                Network = expressChainOptions.Network,
                AddressVersion = expressChainOptions.AddressVersion,
                MillisecondsPerBlock = millisecondsPerBlock,
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
                BindAddress = IPAddress.Parse(expressChainOptions.GetRpcBindAddressValue()),
                Port = expressChainOptions.ConsensusNodes[0].RpcPort,
                MaxGasInvoke = (long)new BigDecimal(decimal.Parse(expressChainOptions.GetRpcMaxGasInvokeValue()), NativeContract.GAS.Decimals).Value,
                MaxFee = (long)new BigDecimal(decimal.Parse(expressChainOptions.GetRpcMaxFeeValue()), NativeContract.GAS.Decimals).Value,
                MaxIteratorResultItems = int.Parse(expressChainOptions.GetRpcMaxIteratorResultItemsValue()),
                SessionEnabled = bool.Parse(expressChainOptions.GetRpcSessionEnabledValue())
            };

        public static Plugins.DBFTPlugin.Settings GetConsensusSettings(this ExpressChainOptions expressChainOptions)
        {
            var settings = new Dictionary<string, string?>()
            {
                ["PluginConfiguration:Network"] = $"{expressChainOptions.Network}",
                ["PluginConfiguration:IgnoreRecoveryLogs"] = bool.TrueString,
            };

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();

            return new(config.GetSection("PluginConfiguration"));
        }

        public static string GetRpcBindAddressValue(this ExpressChainOptions expressChainOptions) =>
            expressChainOptions.Settings[RpcServerSettingsKeyNames.BindAddress] ?? $"{IPAddress.Loopback}";

        public static string GetRpcMaxGasInvokeValue(this ExpressChainOptions expressChainOptions) =>
            expressChainOptions.Settings[RpcServerSettingsKeyNames.MaxGasInvoke] ?? $"{RpcServerSettings.Default.MaxGasInvoke}";

        public static string GetRpcMaxFeeValue(this ExpressChainOptions expressChainOptions) =>
            expressChainOptions.Settings[RpcServerSettingsKeyNames.MaxFee] ?? $"{RpcServerSettings.Default.MaxFee}";

        public static string GetRpcMaxIteratorResultItemsValue(this ExpressChainOptions expressChainOptions) =>
            expressChainOptions.Settings[RpcServerSettingsKeyNames.MaxIteratorResultItems] ?? $"{RpcServerSettings.Default.MaxIteratorResultItems}";

        public static string GetRpcSessionEnabledValue(this ExpressChainOptions expressChainOptions) =>
            expressChainOptions.Settings[RpcServerSettingsKeyNames.SessionEnabled] ?? $"{RpcServerSettings.Default.SessionEnabled}";
    }
}
