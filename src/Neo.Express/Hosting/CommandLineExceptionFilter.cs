// Copyright (C) 2015-2024 The Neo Project.
//
// CommandLineExceptionFilter.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Express.Extensions;
using System.CommandLine.Invocation;

namespace Neo.Express.Hosting
{
    internal class CommandLineExceptionFilter
    {
        public static void Handler(Exception exception, InvocationContext context)
        {
            if (exception is not OperationCanceledException)
            {
                context.Console.ErrorMessage(exception, false);
            }

            context.ExitCode = exception.HResult;
        }
    }
}
