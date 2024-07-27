// Copyright (C) 2015-2024 The Neo Project.
//
// ExpressApplicationOptions.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.Express.Models.Options
{
    internal sealed class ExpressApplicationOptions
    {
        internal sealed class BlockchainOptions
        {
            internal sealed class BlockchainStorageOptions
            {
                public string? Path { get; set; }
            }

            public BlockchainStorageOptions Storage { get; set; } = new();
            public uint MillisecondsPerBlock { get; set; }
        }

        public BlockchainOptions Blockchain { get; set; } = new();
    }
}
