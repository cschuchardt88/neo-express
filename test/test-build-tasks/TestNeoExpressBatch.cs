// Copyright (C) 2015-2026 The Neo Project.
//
// TestNeoExpressBatch.cs file belongs to neo-express project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or https://opensource.org/license/MIT for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.BuildTasks;
using System.Collections.Generic;
using Xunit;

namespace build_tasks
{
    public class TestNeoExpressBatch
    {
        [Fact]
        public void parse_deploy_commands_skips_comments_and_other_lines()
        {
            var batch = """
                # setup
                transfer 1 gas genesis node1
                contract deploy ./bin/sc/Contract.nef genesis
                contract deploy ./bin/sc/Nep17Contract.nef owner
                checkpoint create ./checkpoints/deployed -f
                // done
                """;

            var deploys = NeoExpressBatch.ParseDeployCommands(batch);
            Assert.Equal(2, deploys.Count);
            Assert.Equal("./bin/sc/Contract.nef", deploys[0].NefFile);
            Assert.Equal("genesis", deploys[0].Account);
            Assert.Equal("./bin/sc/Nep17Contract.nef", deploys[1].NefFile);
            Assert.Equal("owner", deploys[1].Account);
        }

        [Fact]
        public void chain_running_failure_is_detected_from_stderr()
        {
            var results = new ProcessResults(
                1,
                new List<string>(),
                new List<string> { "Cannot run batch command while blockchain is running" });
            Assert.True(NeoExpressBatch.IsChainRunningFailure(results));
        }

        [Fact]
        public void unrelated_batch_failure_is_not_treated_as_running_chain()
        {
            var results = new ProcessResults(
                1,
                new List<string> { "Batch file missing" },
                new List<string>());
            Assert.False(NeoExpressBatch.IsChainRunningFailure(results));
        }
    }
}
