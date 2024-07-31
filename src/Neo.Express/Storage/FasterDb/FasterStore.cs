// Copyright (C) 2015-2024 The Neo Project.
//
// FasterStore.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using FASTER.core;
using Neo.Express.Hosting;
using Neo.Persistence;
using System.Collections;

namespace Neo.Express.Storage.FasterDb
{
    internal sealed class FasterStore : IStore, IEnumerable<KeyValuePair<byte[], byte[]>>
    {
        private readonly AsyncPool<ClientSession<byte[], byte[], byte[], byte[], Empty, SimpleFunctions<byte[], byte[]>>> _sessionPool;
        private readonly FasterKV<byte[], byte[]> _store;

        private readonly IDevice _logDevice;
        private readonly IDevice _objLogDevice;

        private readonly string _storePath;

        public FasterStore(string dirPath)
        {
            _storePath = Path.GetFullPath(dirPath);

            _logDevice = Devices.CreateLogDevice(Path.Combine(_storePath, "LOG"), recoverDevice: true);
            _objLogDevice = Devices.CreateLogDevice(Path.Combine(_storePath, "DATA"), recoverDevice: true);

            var logSettings = new LogSettings()
            {
                LogDevice = _logDevice,
                ObjectLogDevice = _objLogDevice,
            };

            _store = new
            (
                1L << 20,
                logSettings,
                checkpointSettings: new CheckpointSettings()
                {
                    CheckpointDir = Path.Combine(_storePath, "data", NeoExpressConfigurationDefaults.CheckpointDirectoryName),
                    RemoveOutdated = false
                },
                tryRecoverLatest: true
            );

            _sessionPool = new AsyncPool<ClientSession<byte[], byte[], byte[], byte[], Empty, SimpleFunctions<byte[], byte[]>>>
            (
                _logDevice.ThrottleLimit,
                () => _store.For(new SimpleFunctions<byte[], byte[]>()).NewSession<SimpleFunctions<byte[], byte[]>>()
            );
        }

        public void Dispose()
        {
            _ = _store.TryInitiateFullCheckpoint(out _, CheckpointType.FoldOver);
            _store.CompleteCheckpointAsync().GetAwaiter().GetResult();
            _store.Dispose();
            _logDevice.Dispose();
            _objLogDevice.Dispose();
            _sessionPool.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Restore(Guid checkpointToken)
        {
            _store.Recover(checkpointToken);
        }

        public void Reset()
        {
            _store.Reset();
        }

        public bool Contains(byte[] key)
        {
            return TryGet(key) != null;
        }

        public void Delete(byte[] key)
        {
            if (_sessionPool.TryGet(out var session) == false)
                session = _sessionPool.GetAsync().GetAwaiter().GetResult();

            var status = session.Delete(key);

            if (status.IsPending)
                session.CompletePending(true);
            _sessionPool.Return(session);
        }

        public ISnapshot GetSnapshot()
        {
            if (_store.TryInitiateHybridLogCheckpoint(out var snapshotId, CheckpointType.FoldOver))
                _store.CompleteCheckpointAsync().GetAwaiter().GetResult();
            return new FasterSnapshot(this, _storePath, snapshotId, _sessionPool);
        }

        public void Put(byte[] key, byte[] value)
        {
            if (_sessionPool.TryGet(out var session) == false)
                session = _sessionPool.GetAsync().GetAwaiter().GetResult();

            var status = session.Upsert(key, value);

            if (status.IsPending)
                session.CompletePending(true);
            _sessionPool.Return(session);
        }

        public IEnumerable<(byte[] Key, byte[] Value)> Seek(byte[] keyOrPrefix, SeekDirection direction)
        {
            if (_sessionPool.TryGet(out var session) == false)
                session = _sessionPool.GetAsync().GetAwaiter().GetResult();

            var keyComparer = direction == SeekDirection.Forward ? ByteArrayComparer.Default : ByteArrayComparer.Reverse;
            var list = new List<(byte[] Key, byte[] Value)>();

            using var iter = session.Iterate();
            while (iter.GetNext(out _))
            {
                var key = iter.GetKey();
                var value = iter.GetValue();

                if (keyOrPrefix?.Length > 0)
                {
                    if (keyComparer.Compare(key, keyOrPrefix) >= 0)
                        list.Add((key, value));
                }
            }

            _sessionPool.Return(session);

            return list.OrderBy(o => o.Key, keyComparer);
        }

        public byte[]? TryGet(byte[] key)
        {
            if (_sessionPool.TryGet(out var session) == false)
                session = _sessionPool.GetAsync().GetAwaiter().GetResult();

            var (status, output) = session.Read(key);
            byte[]? value = null;

            if (status.Found)
                value = output;
            else if (status.IsPending && session.CompletePendingWithOutputs(out var iter, true, true))
            {
                using (iter)
                {
                    while (iter.Next())
                    {
                        if (iter.Current.Key.SequenceEqual(key))
                        {
                            value = iter.Current.Output;
                            break;
                        }
                    }
                }
            }

            _sessionPool.Return(session);
            return value;
        }

        public IEnumerator<KeyValuePair<byte[], byte[]>> GetEnumerator()
        {
            if (_sessionPool.TryGet(out var session) == false)
                session = _sessionPool.GetAsync().GetAwaiter().GetResult();

            using var iter = session.Iterate();
            while (iter.GetNext(out _))
            {
                var key = iter.GetKey();
                var value = iter.GetValue();

                yield return new(key, value);
            }

            _sessionPool.Return(session);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
