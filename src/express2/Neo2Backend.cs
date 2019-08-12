﻿using Neo.Express.Abstractions;
using Neo.Express.Backend2.Persistence;
using Neo.Persistence;
using Neo.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Neo.Express.Backend2
{
    public class Neo2Backend : INeoBackend
    {
        public void CreateBlockchain(string filename, int count, ushort port)
        {
            if ((uint)port + (count * 3) >= ushort.MaxValue)
            {
                // TODO: better error message
                throw new Exception("Invalid port");
            }

            var wallets = new List<(DevWallet wallet, Wallets.WalletAccount account)>(count);

            for (int i = 1; i <= count; i++)
            {
                var wallet = new DevWallet($"node{i}");
                var account = wallet.CreateAccount();
                account.IsDefault = true;
                wallets.Add((wallet, account));
            }

            var keys = wallets.Select(t => t.account.GetKey().PublicKey).ToArray();

            var contract = Neo.SmartContract.Contract.CreateMultiSigContract((keys.Length * 2 / 3) + 1, keys);

            foreach (var (wallet, account) in wallets)
            {
                var multiSigContractAccount = wallet.CreateAccount(contract, account.GetKey());
                multiSigContractAccount.Label = "MultiSigContract";
            }

            var chain = new DevChain(wallets.Select(t => new DevConsensusNode()
            {
                Wallet = t.wallet,
                TcpPort = port++,
                WebSocketPort = port++,
                RpcPort = port++
            }));

            chain.Save(filename);
        }

        private class LogPlugin : Plugin, ILogPlugin
        {
            private readonly Action<string> consoleWrite;

            public LogPlugin(Action<string> consoleWrite)
            {
                this.consoleWrite = consoleWrite;
            }

            public override void Configure()
            {
            }

            void ILogPlugin.Log(string source, LogLevel level, string message)
            {
                Console.WriteLine($"{DateTimeOffset.Now.ToString("HH:mm:ss.ff")} {source} {level} {message}");
            }
        }

        private static CancellationTokenSource Run(Store store, DevConsensusNode consensusNode, Action<string> consoleWrite)
        {
            var cts = new CancellationTokenSource();

            Task.Factory.StartNew(() =>
            {
                try
                {
                    using (var system = new NeoSystem(store))
                    {
                        var logPlugin = new LogPlugin(consoleWrite);
                        var rpcPlugin = new ExpressNodeRpcPlugin();

                        system.StartNode(consensusNode.TcpPort, consensusNode.WebSocketPort);
                        system.StartConsensus(consensusNode.Wallet);
                        system.StartRpc(IPAddress.Any, consensusNode.RpcPort, consensusNode.Wallet);

                        cts.Token.WaitHandle.WaitOne();
                    }
                }
                catch (Exception ex)
                {
                    consoleWrite(ex.ToString());
                    cts.Cancel();
                }
                finally
                {
                    if (store is IDisposable disp)
                    {
                        disp.Dispose();
                    }
                }
            });

            return cts;
        }

        public CancellationTokenSource RunBlockchain(string filename, string storeFolder, int? index, uint secondsPerBlock, bool reset, Action<string> consoleWrite)
        {
            var devChain = DevChain.Initialize(filename, secondsPerBlock);

            if (!index.HasValue && devChain.ConsensusNodes.Count > 1)
            {
                throw new Exception("Node index not specified");
            }

            var _index = index ?? 0;
            if (_index >= devChain.ConsensusNodes.Count || _index < 0)
            {
                throw new Exception("Invalid node index");
            }

            var consensusNode = devChain.ConsensusNodes[_index];
            var blockchainPath = Path.Combine(storeFolder, consensusNode.Wallet.DefaultAccount.Address);

            if (reset && Directory.Exists(blockchainPath))
            {
                Directory.Delete(blockchainPath, true);
            }

            if (!Directory.Exists(blockchainPath))
            {
                Directory.CreateDirectory(blockchainPath);
            }

            return Run(new RocksDbStore(blockchainPath), consensusNode, consoleWrite);
        }
    }
}
