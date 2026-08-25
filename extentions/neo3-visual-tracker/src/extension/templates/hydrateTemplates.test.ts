import assert from "node:assert/strict";
import { mkdtemp, readFile, rm } from "node:fs/promises";
import { tmpdir } from "node:os";
import { join } from "node:path";
import test from "node:test";

import { hydrateFiles, substituteParameters } from "./hydrateTemplates";

const packageRoot = join(__dirname, "../../..");

test("substituteParameters replaces every placeholder", () => {
  assert.equal(
    substituteParameters("src/$_CLASSNAME_$.cs in $_CONTRACTNAME_$", {
      $_CLASSNAME_$: "TokenEscrowContract",
      $_CONTRACTNAME_$: "TokenEscrow",
    }),
    "src/TokenEscrowContract.cs in TokenEscrow"
  );
});

test("hydrateFiles overlays a C# starter onto the scaffold", async () => {
  const destination = await mkdtemp(join(tmpdir(), "neoxp-template-"));
  const parameters = {
    $_CLASSNAME_$: "TokenEscrowContract",
    $_CONTRACTNAME_$: "TokenEscrow",
    $_MAINFILE_$: "src/TokenEscrowContract.cs",
  };

  try {
    await hydrateFiles(
      join(packageRoot, "resources/new-contract/csharp"),
      destination,
      parameters
    );
    await hydrateFiles(
      join(packageRoot, "resources/new-contract/csharp-starters/nep17"),
      destination,
      parameters
    );

    const contract = await readFile(
      join(destination, "src/TokenEscrowContract.cs"),
      "utf8"
    );
    assert.match(contract, /class TokenEscrowContract : Nep17Token/);
    assert.match(contract, /namespace TokenEscrow/);
    assert.doesNotMatch(contract, /\$_CLASSNAME_\$/);
    assert.doesNotMatch(contract, /ChangeNumber/);

    const tests = await readFile(
      join(destination, "test/TokenEscrowContractTests.cs"),
      "utf8"
    );
    assert.match(tests, /c\.symbol\(\)/);
    assert.match(tests, /EXAMPLE/);

    const csproj = await readFile(
      join(destination, "src/TokenEscrowContract.csproj"),
      "utf8"
    );
    assert.match(csproj, /Neo\.BuildTasks/);
    assert.match(csproj, /NeoContractName>TokenEscrowContract/);

    const tools = JSON.parse(
      await readFile(join(destination, ".config/dotnet-tools.json"), "utf8")
    );
    assert.equal(tools.tools["neo.express"].commands[0], "neoxp");
  } finally {
    await rm(destination, { recursive: true, force: true });
  }
});

test("hydrateFiles can create a blank official starter", async () => {
  const destination = await mkdtemp(join(tmpdir(), "neoxp-blank-"));
  const parameters = {
    $_CLASSNAME_$: "HelloContract",
    $_CONTRACTNAME_$: "Hello",
  };

  try {
    await hydrateFiles(
      join(packageRoot, "resources/new-contract/csharp"),
      destination,
      parameters
    );
    await hydrateFiles(
      join(packageRoot, "resources/new-contract/csharp-starters/blank"),
      destination,
      parameters
    );
    const contract = await readFile(
      join(destination, "src/HelloContract.cs"),
      "utf8"
    );
    assert.match(contract, /class HelloContract : SmartContract/);
    assert.match(contract, /MyMethod/);
    assert.doesNotMatch(contract, /ChangeNumber/);
  } finally {
    await rm(destination, { recursive: true, force: true });
  }
});
