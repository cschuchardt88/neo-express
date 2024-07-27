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
        public static ExpressWallet GetWallet(this ExpressWalletOptions expressWalletOptions, ProtocolSettings protocolSettings)
        {
            var wallet = new ExpressWallet(expressWalletOptions.Name, protocolSettings);
            foreach (var account in expressWalletOptions.Accounts.OrderBy(o => o.IsDefault))
            {
                wallet.CreateAccount(Convert.FromHexString(account.PrivateKey));
            }
            return wallet;
        }
    }
}
