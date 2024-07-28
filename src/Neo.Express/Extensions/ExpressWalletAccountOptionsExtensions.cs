// Copyright (C) 2015-2024 The Neo Project.
//
// ExpressWalletAccountOptionsExtensions.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Express.Models;
using Neo.Express.Models.Options;
using Neo.SmartContract;
using Neo.Wallets;

namespace Neo.Express.Extensions
{
    internal static class ExpressWalletAccountOptionsExtensions
    {
        public static ExpressWalletAccount ToWalletAccount(this ExpressWalletAccountOptions expressWalletAccountOptions, ProtocolSettings protocolSettings)
        {
            var kp = new KeyPair(Convert.FromHexString(expressWalletAccountOptions.PrivateKey));
            var script = expressWalletAccountOptions.Contract?.Script;

            byte[]? scriptBytes = null;
            if (string.IsNullOrWhiteSpace(script) == false)
                scriptBytes = Convert.FromHexString(script);

            var contract = new Contract()
            {
                Script = scriptBytes,
                ParameterList = expressWalletAccountOptions.Contract?.Parameters
                    .Select(Enum.Parse<ContractParameterType>)
                    .ToArray()
            };

            var scriptHash = expressWalletAccountOptions.Address.ToScriptHash(protocolSettings.AddressVersion);

            return new(scriptHash, protocolSettings, kp, contract)
            {
                Label = expressWalletAccountOptions.Label,
                IsDefault = expressWalletAccountOptions.IsDefault,
            };
        }
    }
}
