// Copyright (C) 2015-2024 The Neo Project.
//
// NeoExpressConfigurationDefaults.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.Express.Hosting
{
    internal class NeoExpressConfigurationDefaults
    {
        public static readonly string EnvironmentVariablePrefix = "NEOEXPRESS_";
        public static readonly string StoreProviderName = "RocksDbStore";
        public static readonly string ExpressConfigFilename = "default.neo-express";
        public static readonly string ProgramConfigFilename = "config.json";
        public static readonly string BaseDirectory = @$"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\.neo-express";
        public static readonly string BlockChainDirectoryName = "blockchain-nodes";

        public static readonly uint[] Networks =
        [
            /* Neo 2 MainNet */ 7630401,
            /* Neo 2 TestNet */ 1953787457,
            /* Neo 3 MainNet */ 860833102,
            /* Neo 3 T5 TestNet */ 894710606,
            /* Neo 3 T4 TestNet */ 877933390,
            /* Neo 3 RC3 TestNet */ 844378958,
            /* Neo 3 RC1 TestNet */ 827601742,
            /* Neo 3 Preview5 TestNet */ 894448462,
        ];
    }
}
