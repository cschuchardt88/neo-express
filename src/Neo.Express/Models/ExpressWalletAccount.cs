// Copyright (C) 2015-2024 The Neo Project.
//
// ExpressWalletAccount.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract;
using Neo.Wallets;

namespace Neo.Express.Models
{
    internal sealed class ExpressWalletAccount : WalletAccount
    {
        public override bool HasKey => _keyPair != null;

        private readonly KeyPair? _keyPair;

        public ExpressWalletAccount(
            UInt160 scriptHash,
            ProtocolSettings protocolSettings,
            KeyPair? keyPair = null,
            Contract? contract = null) : base(scriptHash, protocolSettings)
        {
            _keyPair = keyPair;
            Contract = contract;
        }

        public override KeyPair? GetKey() => _keyPair;
    }
}
