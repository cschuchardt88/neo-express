// Copyright (C) 2015-2024 The Neo Project.
//
// ExpressWalletOptionsExtensions.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Express.Models;
using Neo.Express.Models.Options;

namespace Neo.Express.Extensions
{
    internal static class ExpressWalletOptionsExtensions
    {
        public static ExpressWallet ToWallet(this ExpressWalletOptions expressWalletOptions, ProtocolSettings protocolSettings)
        {
            var accounts = expressWalletOptions.Accounts
                .Select(s => s.ToWalletAccount(protocolSettings));
            return new(expressWalletOptions.Name, protocolSettings, accounts);
        }
    }
}
