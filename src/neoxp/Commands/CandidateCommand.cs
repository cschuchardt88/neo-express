// Copyright (C) 2015-2024 The Neo Project.
//
// CandidateCommand.cs file belongs to neo-express project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using McMaster.Extensions.CommandLineUtils;

namespace NeoExpress.Commands
{
    [Command("candidate", Description = "Candidate Commands")]
    [Subcommand(
        typeof(List),
        typeof(Register),
        typeof(UnRegister),
        typeof(UnVote),
        typeof(Vote))]
    partial class CandidateCommand
    {
        internal int OnExecute(CommandLineApplication app, IConsole console)
        {
            console.WriteLine("You must specify at a subcommand.");
            app.ShowHelp(false);
            return 1;
        }
    }
}
