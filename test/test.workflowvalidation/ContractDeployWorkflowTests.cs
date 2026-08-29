// Copyright (C) 2015-2026 The Neo Project.
//
// ContractDeployWorkflowTests.cs file belongs to neo-express project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or https://opensource.org/license/MIT for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using FluentAssertions;
using Neo;
using Neo.BlockchainToolkit;
using Neo.BlockchainToolkit.Models;
using Neo.Extensions;
using Neo.IO;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Native;
using Neo.VM;
using Neo.Wallets;
using NeoExpress;
using NeoExpress.Node;
using System.IO.Abstractions;
using System.Text;
using Xunit;

namespace test.workflowvalidation;

[Collection("OfflineNodeDispose")]
public class ContractDeployWorkflowTests
{
    [Fact]
    public async Task padded_deploy_persists_contract_state()
    {
        using var workspace = Workspace.Create();
        var (nef, manifest) = LoadRegistrar();

        var padded = await ExpressNodeExtensions.DeployAsync(
            workspace.Node,
            nef,
            manifest,
            workspace.Wallet,
            workspace.AccountHash,
            WitnessScope.CalledByEntry,
            data: null,
            additionalGas: 1m);
        await workspace.Node.EnsureTransactionSucceededAsync(padded);

        var (_, log) = await workspace.Node.GetTransactionAsync(padded);
        log!.Executions.Should().Contain(e => e.VMState == VMState.HALT);

        var deployed = await workspace.Node.ListContractsAsync();
        deployed.Should().Contain(c => c.manifest.Name == manifest.Name);
    }

    [Fact]
    public async Task force_update_on_the_same_hash_persists()
    {
        var nefPath = WriteContract();
        var fileSystem = new FileSystem();
        var chain = ExpressChainManagerFactory.CreateChain(1, null);
        var manager = new ExpressChainManager(fileSystem, chain);
        var nodePath = fileSystem.GetNodePath(chain.ConsensusNodes[0]);
        using var writer = new StringWriter();
        try
        {
            using var txExec = new TransactionExecutor(fileSystem, manager, false, false, writer);

            await txExec.ContractDeployAsync(
                nefPath,
                "genesis",
                password: string.Empty,
                WitnessScope.CalledByEntry,
                data: string.Empty,
                force: false,
                additionalGas: 1m);

            await txExec.ContractDeployAsync(
                nefPath,
                "genesis",
                password: string.Empty,
                WitnessScope.CalledByEntry,
                data: string.Empty,
                force: true,
                additionalGas: 1m);

            var output = writer.ToString();
            output.Should().Contain("Deployment");
            output.Should().Contain("Update");
            output.Should().Contain("confirmed");
            output.Should().NotContain("submitted");

            var deployed = await txExec.ExpressNode.ListContractsAsync();
            deployed.Count(c => c.manifest.Name == "DevHawk Registrar").Should().Be(1);
        }
        finally
        {
            if (Directory.Exists(nodePath))
            {
                try
                { Directory.Delete(nodePath, true); }
                catch { }
            }
        }
    }

    sealed class Workspace : IDisposable
    {
        public OfflineNode Node { get; }
        public Wallet Wallet { get; }
        public UInt160 AccountHash { get; }
        readonly string nodePath;

        Workspace(OfflineNode node, Wallet wallet, UInt160 accountHash, string nodePath)
        {
            Node = node;
            Wallet = wallet;
            AccountHash = accountHash;
            this.nodePath = nodePath;
        }

        public static Workspace Create()
        {
            var chain = ExpressChainManagerFactory.CreateChain(1, null);
            var settings = chain.GetProtocolSettings();
            var nodePath = Path.Combine(Path.GetTempPath(), $"neo-express-deploy-workflow-{Guid.NewGuid():N}");
            Directory.CreateDirectory(nodePath);
            var node = new OfflineNode(
                settings,
                new RocksDbExpressStorage(nodePath),
                chain.ConsensusNodes[0].Wallet,
                chain,
                enableTrace: false);
            var (wallet, accountHash) = chain.GetGenesisAccount(settings);
            return new Workspace(node, wallet, accountHash, nodePath);
        }

        public void Dispose()
        {
            Node.Dispose();
            if (Directory.Exists(nodePath))
            {
                try
                { Directory.Delete(nodePath, true); }
                catch { }
            }
        }
    }

    static (NefFile nef, ContractManifest manifest) LoadRegistrar()
    {
        var root = FindRepositoryRoot(AppContext.BaseDirectory);
        var nefBytes = File.ReadAllBytes(Path.Combine(root, "test", "test.bctklib", "_testFiles", "registrar.nef"));
        var nef = nefBytes.AsSerializable<NefFile>();
        var manifest = ContractManifest.Parse(
            File.ReadAllText(Path.Combine(root, "test", "test.bctklib", "_testFiles", "registrar.manifest.json")));
        return (nef, manifest);
    }

    static string WriteContract()
    {
        var root = FindRepositoryRoot(AppContext.BaseDirectory);
        var dir = Path.Combine(Path.GetTempPath(), $"neo-express-contract-{Guid.NewGuid():N}");
        Directory.CreateDirectory(dir);
        var nefPath = Path.Combine(dir, "registrar.nef");
        File.Copy(Path.Combine(root, "test", "test.bctklib", "_testFiles", "registrar.nef"), nefPath);
        File.Copy(
            Path.Combine(root, "test", "test.bctklib", "_testFiles", "registrar.manifest.json"),
            Path.ChangeExtension(nefPath, ".manifest.json"));
        return nefPath;
    }

    static string FindRepositoryRoot(string startPath)
    {
        var directory = new DirectoryInfo(startPath);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "neo-express.sln")))
            {
                return directory.FullName;
            }
            directory = directory.Parent;
        }
        throw new InvalidOperationException($"Could not find neo-express.sln starting from {startPath}.");
    }
}
