// Copyright (C) 2015-2024 The Neo Project.
//
// NeoExpressConfigurationProvider.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.Extensions.Configuration;
using System.Collections;

namespace Neo.Express.Hosting.Providers
{
    internal sealed class NeoExpressConfigurationProvider : ConfigurationProvider
    {
        public override void Load()
        {
            Data = CreateDefaultKeys();
            Load(Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Machine));
            Load(Environment.GetEnvironmentVariables(EnvironmentVariableTarget.User));
            Load(Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Process));
        }

        private void Load(IDictionary envVariables)
        {
            var e = envVariables.GetEnumerator();

            try
            {
                while (e.MoveNext())
                {
                    var key = (string)e.Entry.Key;
                    var value = (string?)e.Entry.Value;

                    AddIfNormalizedKeyMatchesPrefix(Data, key, value);
                }
            }
            finally
            {
                (e as IDisposable)?.Dispose();
            }
        }

        private static Dictionary<string, string?> CreateDefaultKeys() =>
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["blockchain:storage:path"] = @$"{NeoExpressConfigurationDefaults.BaseDirectory}\{NeoExpressConfigurationDefaults.BlockChainDirectoryName}",
            };

        private static void AddIfNormalizedKeyMatchesPrefix(IDictionary<string, string?> data, string normalizedKey, string? value)
        {
            var normalizedPrefix = NeoExpressConfigurationDefaults.EnvironmentVariablePrefix;

            if (normalizedKey.StartsWith(normalizedPrefix, StringComparison.OrdinalIgnoreCase))
                data[Normalize(normalizedKey[normalizedPrefix.Length..])] = value;
        }

        private static string Normalize(string key) =>
            key.Replace("_", ConfigurationPath.KeyDelimiter);
    }
}
