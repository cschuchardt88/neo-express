<!-- markdownlint-enable -->
# Neo Express quickstart

A longer CLI walkthrough: install, create a chain, compile a C# contract, deploy, and invoke.
For the shortest path (including VS Code), start at [getting-started.md](getting-started.md).

Works on Windows, macOS, and Ubuntu.

## 1. Install Neo Express

### .NET tool (recommended)

Requires [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).

```shell
dotnet tool install Neo.Express -g
neoxp --version
```

### Release package

1. Download the latest build from [neo-express releases](https://github.com/neo-project/neo-express/releases/latest).
2. Unzip it.
3. Run `neoxp` (`neoxp.exe` on Windows) from that directory.

Platform libraries (RocksDB) are listed in [installation.md](installation.md).

## 2. Create and use a private chain

```shell
neoxp create
neoxp wallet list
neoxp show balances genesis
neoxp transfer 1 gas genesis node1
neoxp run --seconds-per-block 1
```

`genesis` is the consensus multi-sig that holds the genesis NEO and GAS. `node1` is the
default consensus-node wallet.

Leave `neoxp run` going. Other commands use a second terminal in the same folder
(where `default.neo-express` lives).

Full command list: [command-reference.md](command-reference.md).

## 3. Compile a C# contract

This repo already has Express-ready starters. From the repository root:

```shell
dotnet build samples/examples/Nep17
```

That:

- restores local `neoxp` and `nccs` tools
- compiles `Nep17Contract.nef` / `.manifest.json` to `samples/examples/Nep17/bin/sc`
- creates `default.neo-express` next to the example if it is missing
- resets the chain and deploys with `genesis` (`express.batch`)

| Starter | Path |
| ------- | ---- |
| Blank | `samples/examples/Blank` |
| NEP-17 token | `samples/examples/Nep17` |
| NEP-11 NFT | `samples/examples/Nep11` |
| Oracle | `samples/examples/Oracle` |
| Ownable | `samples/examples/Ownable` |

Details: [samples/examples/README.md](../samples/examples/README.md).

### Create your own contract

**VS Code:** Quick Start / Smart contracts → **New contract** → C# → pick Blank, NEP-17,
NEP-11, Oracle, Ownable, or Storage.

**Terminal:** install [Neo.SmartContract.Template](https://www.nuget.org/packages/Neo.SmartContract.Template)
and add `Neo.BuildTasks` like the examples, or copy an example folder:

```shell
dotnet new install Neo.SmartContract.Template
dotnet new neocontractnep17 -n MyToken -o ./MyToken
```

`dotnet build` on an example project runs `nccs` through `Neo.BuildTasks`. You can also run
`nccs` yourself after `dotnet tool install Neo.Compiler.CSharp -g`. Output is `bin/sc/*.nef`.

## 4. Deploy and invoke

If you used `samples/examples/*`, the first `dotnet build` already deployed. With `neoxp run`
in another terminal:

```shell
neoxp contract run Nep17Contract symbol --results
```

`--results` is a dry run. To submit a transaction:

```shell
neoxp contract run Nep17Contract symbol --account genesis
```

Deploy a `.nef` you compiled yourself:

```shell
neoxp contract deploy ./src/bin/sc/MyToken.nef genesis
```

Reusable calls belong in a `.neo-invoke.json` file:

```shell
neoxp contract invoke ./invoke-files/contract.neo-invoke.json genesis
```

See [Neo Express Invocation File](Neo%20Express%20Invocation%20File.md).

## 5. Rebuild so the contract redeploys

`Neo.BuildTasks` skips the Express batch when the `.nef` has not changed. After **Clean**
or **Rebuild**, the stamp is deleted and the next build resets the chain and deploys again:

```shell
dotnet rebuild samples/examples/Nep17
```

Checkpoint-backed `dotnet test` workflow: [contract-testing.md](contract-testing.md).
