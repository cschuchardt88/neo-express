// Copyright (C) 2015-2024 The Neo Project.
//
// NeoSystemHostedService.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Neo.Express.Extensions;
using Neo.Express.Models;
using Neo.Express.Models.Options;
using Neo.Express.Storage.FasterDb;
using Neo.Plugins.DBFTPlugin;
using Neo.SmartContract.Native;
using System.CommandLine;
using System.Net;

namespace Neo.Express.Hosting.Services
{
    internal partial class NeoSystemHostedService : IHostedService, IAsyncDisposable
    {
        private readonly ExpressChainOptions _expressChainOptions;
        private readonly ExpressApplicationOptions _expressAppOptions;
        private readonly ExpressConsensusNodeOptions _consensusNode;

        private readonly ProtocolSettings _protocolSettings;

        private readonly CancellationTokenSource _stopCts = new();
        private readonly TaskCompletionSource _stoppedTcs = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly SemaphoreSlim _neoSystemStoppedSemaphore = new(1);

        private readonly ExpressWallet _dbftWallet;
        private readonly IConsole _console;

        private NeoSystem? _neoSystem;
        private FasterDBStore? _fasterDbStore;
        private DBFTPlugin? _dbftPlugin;

        private bool _hasStarted = false;
        private int _stopping;

        public NeoSystemHostedService(
            IOptions<ExpressApplicationOptions> appOptions,
            IOptions<ExpressChainOptions> chainOptions,
            IConsole console)
        {
            _expressChainOptions = chainOptions.Value;
            _expressAppOptions = appOptions.Value;

            var nodeIndex = _expressAppOptions.Blockchain.Node.Index;
            _consensusNode = _expressChainOptions.ConsensusNodes[nodeIndex];

            _protocolSettings = _expressChainOptions.GetProtocolSettings(_expressAppOptions.Blockchain.Node.MillisecondsPerBlock);
            _dbftWallet = _consensusNode.Wallet.ToWallet(_protocolSettings);
            _console = console;
        }

        public async ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref _stopping, 1) == 1)
            {
                await _stoppedTcs.Task.ConfigureAwait(false);
                return;
            }

            _stopCts.Cancel();
            await _neoSystemStoppedSemaphore.WaitAsync().ConfigureAwait(false);

            try
            {
                await StopAsync(CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _stoppedTcs.TrySetException(ex);
                throw;
            }
            finally
            {
                _stopCts.Dispose();
                _neoSystemStoppedSemaphore.Release();
            }

            _stoppedTcs.TrySetResult();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (_hasStarted)
                    throw new InvalidOperationException($"{nameof(NeoSystemHostedService)} has already been started.");
                _hasStarted = true;

                var chainDefaultAccountAddress = _consensusNode.GetDefaultAccountAddress();
                var storagePath = $@"{_expressAppOptions.Blockchain.Storage.Path}\{chainDefaultAccountAddress}";

                _fasterDbStore = new FasterDBStore();
                _dbftPlugin = new DBFTPlugin(_expressChainOptions.GetConsensusSettings());
                _neoSystem = new(_protocolSettings, NeoExpressConfigurationDefaults.StoreProviderName, storagePath);
                _neoSystem.StartNode(new()
                {
                    Tcp = new(IPAddress.Loopback, _consensusNode.TcpPort),
                });

                _dbftPlugin.Start(_dbftWallet);
                _console.InfoMessage($"{NativeContract.Ledger.CurrentIndex(_neoSystem.StoreView)}");
            }
            catch
            {
                await StopAsync(cancellationToken).ConfigureAwait(false);
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_hasStarted == false)
                return Task.CompletedTask;

            _hasStarted = false;

            _neoSystem?.Dispose();
            _dbftPlugin?.Dispose();
            _fasterDbStore?.Dispose();

            _neoSystem = null;
            _dbftPlugin = null;
            _fasterDbStore = null;

            return Task.CompletedTask;
        }
    }
}
