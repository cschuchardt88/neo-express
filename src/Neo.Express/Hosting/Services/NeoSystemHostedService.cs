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
using Neo.Express.Models.Options;

namespace Neo.Express.Hosting.Services
{
    internal class NeoSystemHostedService
        (IOptions<ExpressChainOptions> options) : IHostedService, IAsyncDisposable
    {
        private readonly ExpressChainOptions _expressChainOptions = options.Value;

        private readonly CancellationTokenSource _stopCts = new();
        private readonly TaskCompletionSource _stoppedTcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

        private readonly SemaphoreSlim _neoSystemStoppedSemaphore = new(1);

        private NeoSystem? _neoSystem;

        private bool _hasStarted = false;
        private int _stopping;

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

                _neoSystem = new(_expressChainOptions.GetProtocolSettings(), string.Empty);
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

            return Task.CompletedTask;
        }
    }
}
