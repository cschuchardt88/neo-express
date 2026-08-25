# Neo Smart Contract Debugger (VS Code)

A source-level, time-travel debugger for Neo N3 smart contracts. This extension registers the
`neo-contract` debug type in VS Code and drives the [`neodebug`](../../docs/debugger-command-reference.md)
tool (the `Neo.Debug` global tool) as a [Debug Adapter Protocol](https://microsoft.github.io/debug-adapter-protocol/)
host.

Repo-wide walkthrough: [Getting started](../../docs/getting-started.md#debugger).
Launch-configuration schema: [debugger-command-reference.md](../../docs/debugger-command-reference.md).

## Getting started

1. Install [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).
2. `dotnet tool install Neo.Debug -g`
3. Build a contract that emits `.nef` and `.nefdbgnfo` (for example
   `dotnet build samples/examples/Nep17`).
4. Add a `neo-contract` configuration to `.vscode/launch.json` (see below).
5. Set breakpoints in the C# source and start debugging.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Visual Studio Code 1.104 or later

Install the debugger tool:

```shell
dotnet tool install Neo.Debug -g
```

The extension runs `neodebug` from your `PATH`. To point at a different build, set
`neo-contract.debugAdapterPath` in your settings.

## Usage

Add a configuration to `.vscode/launch.json`. Replay a recorded trace (supports stepping backward):

```jsonc
{
  "name": "Debug Neo contract (trace)",
  "type": "neo-contract",
  "request": "launch",
  "program": "${workspaceFolder}/bin/sc/Contract.nef",
  "invocation": { "trace-file": "${workspaceFolder}/traces/transaction.neo-trace" },
  "return-types": [ "int" ]
}
```

…or deploy and run the contract live:

```jsonc
{
  "name": "Debug Neo contract (live)",
  "type": "neo-contract",
  "request": "launch",
  "program": "${workspaceFolder}/bin/sc/Contract.nef",
  "invocation": { "operation": "transfer", "args": [ "@NXV7ZhHiyM1aHXwpVsRZC6BwNFP2jghXAq", 100 ] }
}
```

Set breakpoints in your C# source, then start debugging. See the
[debugger command reference](../../docs/debugger-command-reference.md) for the full launch-configuration
schema (`return-types`, `sourceFileMap`, `debug-view`).

## Packaging

This is a build-free JavaScript extension — it has no `node_modules` and no compile step. Package it
with [`vsce`](https://github.com/microsoft/vscode-vsce):

```shell
npx @vscode/vsce package
```
