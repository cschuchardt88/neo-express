import assert from "node:assert/strict";
import test from "node:test";

import getContractTreeCommand, {
  ContractTreeItemData,
  getWorkspaceContractPath,
} from "./contractTreeCommand";

test("getContractTreeCommand opens Contract Studio for deployed contracts", () => {
  const contract = { name: "Sample", hash: "0x1234" };
  assert.deepEqual(getContractTreeCommand(contract), {
    command: "neo3-visual-devtracker.neo.openContractStudio",
    arguments: [contract],
    title: "Invoke in Contract Studio",
  });
});

test("getContractTreeCommand opens Contract Studio for workspace contracts", () => {
  const contract = { name: "Sample", path: "/workspace/Sample.nef" };
  assert.deepEqual(getContractTreeCommand(contract), {
    command: "neo3-visual-devtracker.neo.openContractStudio",
    arguments: [contract],
    title: "Invoke in Contract Studio",
  });
});

test("workspace contract tree items expose their trusted file path", () => {
  const item = new ContractTreeItemData(
    "Sample",
    undefined,
    undefined,
    "/workspace/Sample.nef"
  );

  assert.equal(getWorkspaceContractPath(item), "/workspace/Sample.nef");
});

test("plain command arguments cannot supply a trusted workspace path", () => {
  assert.equal(
    getWorkspaceContractPath({
      name: "Sample",
      path: "/outside-workspace/Sample.nef",
    }),
    undefined
  );
});
