// Copyright (C) 2015-2024 The Neo Project.
//
// RpcServerSettingsKeyNames.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.Express.Hosting
{
    internal sealed class RpcServerSettingsKeyNames
    {
        public static readonly string BindAddress = "rpc.BindAddress";
        public static readonly string MaxGasInvoke = "rpc.MaxGasInvoke";
        public static readonly string MaxFee = "rpc.MaxFee";
        public static readonly string MaxIteratorResultItems = "rpc.MaxIteratorResultItems";
        public static readonly string SessionEnabled = "rpc.SessionEnabled";
    }
}
