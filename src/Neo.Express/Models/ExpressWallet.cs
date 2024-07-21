// Copyright (C) 2015-2024 The Neo Project.
//
// ExpressWallet.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract;
using Neo.Wallets;
using System.Collections.Concurrent;

namespace Neo.Express.Models
{
    internal sealed class ExpressWallet
        (string name, ProtocolSettings settings) : Wallet(string.Empty, settings)
    {
        public override string Name { get; } = name;

        public override Version? Version => null;

        private readonly ConcurrentDictionary<UInt160, ExpressWalletAccount> _accounts = new();

        public override bool Contains(UInt160 scriptHash) =>
            _accounts.ContainsKey(scriptHash);

        public override WalletAccount CreateAccount(byte[] privateKey)
        {
            var kp = new KeyPair(privateKey);
            var contract = Contract.CreateSignatureContract(kp.PublicKey);
            var account = new ExpressWalletAccount(contract.ScriptHash, ProtocolSettings, kp, contract);

            if (_accounts.TryAdd(contract.ScriptHash, account) == false)
                throw new KeyNotFoundException();
            return account;
        }

        public override WalletAccount CreateAccount(Contract contract, KeyPair? key = null)
        {
            var account = new ExpressWalletAccount(contract.ScriptHash, ProtocolSettings, key, contract);

            if (_accounts.TryAdd(contract.ScriptHash, account) == false)
                throw new KeyNotFoundException();
            return account;
        }

        public override WalletAccount CreateAccount(UInt160 scriptHash)
        {
            var account = new ExpressWalletAccount(scriptHash, ProtocolSettings);

            if (_accounts.TryAdd(scriptHash, account) == false)
                throw new KeyNotFoundException();
            return account;
        }

        public override WalletAccount GetAccount(UInt160 scriptHash)
        {
            if (_accounts.TryGetValue(scriptHash, out var account) == false)
                throw new KeyNotFoundException();
            return account;
        }

        public override IEnumerable<WalletAccount> GetAccounts()
        {
            return _accounts.Values;
        }

        public override bool ChangePassword(string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public override void Delete()
        {
            throw new NotImplementedException();
        }

        public override bool DeleteAccount(UInt160 scriptHash)
        {
            throw new NotImplementedException();
        }

        public override void Save()
        {
            throw new NotImplementedException();
        }

        public override bool VerifyPassword(string password)
        {
            throw new NotImplementedException();
        }
    }
}
