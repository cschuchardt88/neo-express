<!-- markdownlint-enable -->
# Getting started

This is the shortest path from a clean machine to a running local Neo N3 chain with a
contract you can invoke. Use [Visual Studio Code](#use-visual-studio-code) or the
[command line](#use-the-command-line). Both use the same tools: Neo Express (`neoxp`),
the C# compiler (`nccs`), and `Neo.BuildTasks`.

## What you need

- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) (`dotnet --list-sdks`
  should list a `10.x` version)
- Optional: [Visual Studio Code](https://code.visualstudio.com/Download) 1.104 or later
- Optional: Node.js 20+ only if you load the Visual DevTracker extension from this repo

On Ubuntu, install RocksDB libraries and **do not** install .NET via Snap (see
[installation](installation.md#ubuntu)). On macOS, `brew install rocksdb`.

## 1. Install Neo Express

```shell
dotnet tool install Neo.Express -g
neoxp --version
```

This repo's samples also pin `neoxp` and `nccs` as local tools. From the `samples/` folder:

```shell
cd samples
dotnet tool restore
dotnet tool run neoxp -- --version
```

Full install options (release zip, Trace, WorkNet) are in [installation.md](installation.md).

## 2. Create and run a local chain

```shell
neoxp create
neoxp wallet list
neoxp run --seconds-per-block 1
```

`neoxp create` writes `default.neo-express` in the current directory (genesis plus `node1`).
Leave `neoxp run` in that terminal. In another terminal:

```shell
neoxp show balances genesis
```

By default a new block is minted every 15 seconds. `--seconds-per-block 1` makes transfers
and deploys show up immediately while you are iterating.

## 3. Build and deploy a contract

Pick one of the following.

### Option A — official templates already laid out for Neo Express

From this repository:

```shell
cd samples/examples/Nep17
dotnet build
```

Or from the repository root: `dotnet build samples/examples/Nep17`.

The first build creates `default.neo-express` if it is missing, compiles
`bin/sc/Nep17Contract.nef`, and deploys with `genesis`. This works while a local
Neo Express instance is running (for example after **Start Neo Express** in VS Code).

That chain file is separate from the `default.neo-express` in [step 2](#2-create-and-run-a-local-chain).

Other starters in [`samples/examples/`](../samples/examples/README.md):

| Folder | What it is |
| ------ | ---------- |
| `Blank` | Owner + `MyMethod` |
| `Nep17` | NEP-17 token |
| `Nep11` | NEP-11 NFT |
| `Oracle` | Oracle request / response |
| `Ownable` | Owner + `Destroy` |

`dotnet clean` or `dotnet rebuild` on a contract that sets `NeoExpressBatchFile` deletes the
batch stamp so the next build resets the chain and redeploys.

The original simple sample (a `TokenContract` plus checkpoint tests) is
[`samples/src`](../samples/src) — see [contract testing](contract-testing.md).

### Option B — New contract wizard in Visual Studio Code

1. Open a folder in VS Code.
2. Open the Neo N3 Visual DevTracker view (Neo logo in the activity bar).
3. **Quick Start** or **Smart contracts** → **New contract**.
4. Choose **C#**, then a template: Blank, NEP-17, NEP-11, Oracle, Ownable, or Storage.
5. Name the contract. Files land under `contracts/<name>/`.
6. The scaffold restores tools and builds. Start the Express instance from **Blockchains**,
   connect, then deploy from **Smart contracts**.

How to load the extension from this repo is in
[Use Visual Studio Code](#use-visual-studio-code).

### Option C — `dotnet new` from Neo.SmartContract.Template

```shell
dotnet new install Neo.SmartContract.Template
dotnet new neocontractnep17 -n MyToken -o ./MyToken
```

That template compiles with `nccs`. To get the Neo Express layout used in this repo
(auto-compile via `Neo.BuildTasks` and deploy on build), copy a `samples/examples/*`
project and replace the `.cs` files, or use the VS Code wizard (option B).

## 4. Invoke the contract

With the example chain running (`neoxp run -i samples/examples/Nep17/default.neo-express --seconds-per-block 1`):

```shell
neoxp contract run -i samples/examples/Nep17/default.neo-express Nep17Contract symbol --results
neoxp contract run -i samples/examples/Nep17/default.neo-express Nep17Contract decimals --results
```

`--results` is a trial run (no transaction). Drop it and pass `--account genesis` to submit.

Or use a `.neo-invoke.json` file:

```shell
neoxp contract invoke transfer.neo-invoke.json genesis
```

File format: [Neo Express Invocation File](Neo%20Express%20Invocation%20File.md).

In VS Code, select the rocket on a workspace contract to open Contract Studio, pick a
method, choose the signing account, and run.

## 5. Reset and rebuild

```shell
# wipe chain state (keeps wallets)
neoxp reset -f

# rebuild contract + reset + redeploy (when NeoExpressBatchFile is set)
dotnet rebuild samples/examples/Nep17
```

`Neo.BuildTasks` records a stamp under `obj/` after a successful batch. **Clean** and
**Rebuild** delete that stamp so the batch runs again (chain reset + deploy). Incremental
`dotnet build` skips the batch when the `.nef` and batch file have not changed.

## Use Visual Studio Code

### Load Visual DevTracker from this repo

```shell
cd extentions/neo3-visual-tracker
npm install
npm run compile
```

Then press **F5** in that folder (Extension Development Host), or:

```shell
code --extensionDevelopmentPath="<repo>/extentions/neo3-visual-tracker" "<your-workspace>"
```

The packaged extension bundles `neoxp`. A source checkout without `deps/nxp` uses `neoxp`
from PATH (the global tool from step 1, or `dotnet tool restore` in `samples/`).

Open **Quick Start** and work through: create/start Express → New contract → deploy → invoke.

### Debugger

Install the debugger tool and the [Neo Smart Contract Debugger](../extensions/neodebug-vscode/README.md)
extension:

```shell
dotnet tool install Neo.Debug -g
```

Launch configurations are documented in [debugger-command-reference.md](debugger-command-reference.md).

## What to read next

| Doc | When |
| --- | ---- |
| [installation.md](installation.md) | Global tools, release zips, Ubuntu/macOS |
| [quickstart.md](quickstart.md) | Longer CLI walkthrough (create, compile, deploy, invoke) |
| [contract-testing.md](contract-testing.md) | `dotnet test` against a checkpoint |
| [samples/examples/README.md](../samples/examples/README.md) | Official C# starters for Express |
| [command-reference.md](command-reference.md) | Every `neoxp` command |
| [settings.md](settings.md) | `.neo-express` settings (block time, AutoMine, …) |
| [Visual DevTracker README](../extentions/neo3-visual-tracker/README.md) | VS Code UI |
| [worknet-command-reference.md](worknet-command-reference.md) | Branch MainNet/TestNet locally |
| [trace-command-reference.md](trace-command-reference.md) | Trace public-chain transactions |
