// Copyright (C) 2015-2024 The Neo Project.
//
// FasterDBStore.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Persistence;
using Neo.Plugins;

namespace Neo.Express.Storage.FasterDb
{
    internal sealed class FasterDBStore : Plugin, IStoreProvider
    {
        public override string Description => "Uses Microsoft FASTER to store blockchain data.";

        public FasterDBStore() =>
            StoreFactory.RegisterProvider(this);

        public IStore GetStore(string path) =>
            new FasterStore(path);
    }
}
