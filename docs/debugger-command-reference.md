<!-- markdownlint-enable -->
# Neo Smart Contract Debugger

The [Neo Smart Contract Debugger](../extensions/neodebug-vscode/README.md) VS Code extension
registers the `neo-contract` debug type and launches [`neodebug`](../src/neodebug) (`Neo.Debug`
global tool) as a Debug Adapter.

Getting started: [getting-started.md](getting-started.md#debugger).

## Install

```shell
dotnet tool install Neo.Debug -g
```

The extension runs `neodebug` from `PATH`. Override with
`neo-contract.debugAdapterPath` in VS Code settings.

Requires [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) and VS Code 1.104+.

## Launch configuration

Add a configuration to `.vscode/launch.json`. `program` and `invocation` are required.

| Property | Meaning |
| -------- | ------- |
| `program` | Absolute path to the compiled `.nef`. Sibling `.manifest.json` and `.nefdbgnfo` / `.debug.json` are loaded automatically. |
| `invocation` | Either `{ "trace-file": "<path>" }` to replay a `.neo-trace`, or `{ "operation": "<method>", "args": [ ... ] }` to deploy and run live. |
| `return-types` | Optional cast hints for return values: `int`, `bool`, `string`, `hex`, `byte[]`, `addr`. |
| `sourceFileMap` | Optional map from paths stored in debug info to paths on this machine. |
| `debug-view` | `source` (default) or `disassembly`. |

### Replay a recorded trace (step backward)

Produce a `.neo-trace` with `neoxp contract invoke --trace` (or `neoxp run --trace`) locally,
or with `neotrace` for a public-chain transaction ([trace-command-reference.md](trace-command-reference.md)).

```jsonc
{
  "name": "Debug Neo contract (trace)",
  "type": "neo-contract",
  "request": "launch",
  "program": "${workspaceFolder}/src/bin/sc/Nep17Contract.nef",
  "invocation": { "trace-file": "${workspaceFolder}/traces/transaction.neo-trace" },
  "return-types": [ "string" ]
}
```

### Live invocation

```jsonc
{
  "name": "Debug Neo contract (live)",
  "type": "neo-contract",
  "request": "launch",
  "program": "${workspaceFolder}/src/bin/sc/Nep17Contract.nef",
  "invocation": { "operation": "symbol", "args": [] }
}
```

Set breakpoints in the C# contract source, then start debugging.

Example contracts that emit `.nef` + `.nefdbgnfo` on `dotnet build` are under
[`samples/examples/`](../samples/examples/README.md).
