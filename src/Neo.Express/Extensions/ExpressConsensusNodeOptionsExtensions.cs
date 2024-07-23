// Copyright (C) 2015-2024 The Neo Project.
//
// ExpressConsensusNodeOptionsExtensions.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Express.Models.Options;
using Neo.Wallets;

namespace Neo.Express.Extensions
{
    internal static class ExpressConsensusNodeOptionsExtensions
    {
        public static KeyPair GetKeyPair(this ExpressConsensusNodeOptions expressConsensusNodeOptions) =>
            new(Convert.FromHexString(expressConsensusNodeOptions.Wallet.Accounts.Select(s => s.PrivateKey).Distinct().Single()));
    }
}
