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
using Neo.Persistence;
using System.Collections;

namespace Neo.Express.Storage.FasterDb
{
    internal sealed class FasterStore : IStore, IEnumerable<KeyValuePair<byte[], byte[]>>
    {
        private readonly AsyncPool<ClientSession<byte[], byte[], byte[], byte[], Empty, ByteArrayFunctions>> _sessionPool;
        private readonly FasterKV<byte[], byte[]> _store;
        private readonly LogSettings _logSettings;
        private readonly CheckpointSettings _checkpointSettings;

        private readonly LinkedList<byte[]> _linkedList_Keys = new();
        private readonly string _storePath;

        public FasterStore(string dirPath)
        {
            _storePath = Path.GetFullPath(dirPath);

            _logSettings = new LogSettings()
            {
                LogDevice = new ManagedLocalStorageDevice(Path.Combine(_storePath, "LOG")),
                ObjectLogDevice = new ManagedLocalStorageDevice(Path.Combine(_storePath, "DATA")),
            };

            _checkpointSettings = new CheckpointSettings()
            {
                CheckpointManager = new DeviceLogCommitCheckpointManager(
                    new LocalStorageNamedDeviceFactory(),
                    new NeoCheckpointNamingScheme(_storePath),
                    removeOutdated: false),
            };

            _store = new
            (
                1L << 20,
                _logSettings,
                _checkpointSettings,
                serializerSettings: new SerializerSettings<byte[], byte[]>()
                {
                    keySerializer = () => new ByteArrayBinaryObjectSerializer(),
                    valueSerializer = () => new ByteArrayBinaryObjectSerializer(),
                },
                comparer: new ByteArrayFasterEqualityComparer(),
                tryRecoverLatest: true
            );

            _sessionPool = new AsyncPool<ClientSession<byte[], byte[], byte[], byte[], Empty, ByteArrayFunctions>>
            (
                _logSettings.LogDevice.ThrottleLimit,
                () => _store.For(new ByteArrayFunctions()).NewSession<ByteArrayFunctions>()
            );
        }

        public void Dispose()
        {
            TryFullCheckpointAsync().AsTask().GetAwaiter().GetResult();
            _store.Dispose();
            _sessionPool.Dispose();
            GC.SuppressFinalize(this);
        }

        public async ValueTask<bool> TryFullCheckpointAsync(CheckpointType checkpointType = CheckpointType.FoldOver, CancellationToken cancellationToken = default)
        {
            if (_store.TryInitiateFullCheckpoint(out _, checkpointType))
            {
                await _store.CompleteCheckpointAsync(cancellationToken);
                return true;
            }
            return false;
        }

        public void Restore(Guid checkpointToken)
        {
            _store.Recover(checkpointToken);
        }

        public IEnumerable<Guid> GetLogCheckpointTokens()
        {
            var checkpointManager = _checkpointSettings.CheckpointManager;
            return checkpointManager.GetLogCheckpointTokens();
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
                session = _sessionPool.GetAsync().AsTask().GetAwaiter().GetResult();

            var status = session.Delete(key);

            if (status.IsPending)
                session.CompletePending(true, true);
            _sessionPool.Return(session);
        }

        public ISnapshot GetSnapshot()
        {
            if (_store.TryInitiateFullCheckpoint(out var snapshotId, CheckpointType.Snapshot))
                _store.CompleteCheckpointAsync().AsTask().GetAwaiter().GetResult();
            return new FasterSnapshot(this, _storePath, _checkpointSettings, snapshotId, _sessionPool);
        }

        public void Put(byte[] key, byte[] value)
        {
            if (_sessionPool.TryGet(out var session) == false)
                session = _sessionPool.GetAsync().AsTask().GetAwaiter().GetResult();

            var status = session.Upsert(key, value);

            if (status.IsPending)
                session.CompletePending(true, true);
            _sessionPool.Return(session);
        }

        public IEnumerable<(byte[] Key, byte[] Value)> Seek(byte[] keyOrPrefix, SeekDirection direction)
        {
            if (_sessionPool.TryGet(out var session) == false)
                session = _sessionPool.GetAsync().AsTask().GetAwaiter().GetResult();

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
                session = _sessionPool.GetAsync().AsTask().GetAwaiter().GetResult();

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
                session = _sessionPool.GetAsync().AsTask().GetAwaiter().GetResult();

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
