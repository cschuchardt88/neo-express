import assert from "node:assert/strict";
import { readFileSync } from "node:fs";
import { join } from "node:path";
import test from "node:test";

const templatesSource = readFileSync(join(__dirname, "templates.ts"), "utf8");
const createContractSource = readFileSync(
  join(__dirname, "../../panel/components/quickStart/CreateContract.tsx"),
  "utf8"
);

test("New contract wizard asks for a C# starter after language", () => {
  assert.match(templatesSource, /languageCode === "csharp"/);
  assert.match(templatesSource, /Contract template/);
  assert.match(templatesSource, /csharpStarterLabels\(\)/);
  assert.match(templatesSource, /csharpStarter\?\.overlay/);
  assert.match(
    templatesSource,
    /resources",\s+"new-contract",\s+"csharp-starters"/
  );
});

test("Quick Start new-contract copy mentions official C# starters", () => {
  assert.match(createContractSource, /NEP-17/);
  assert.match(createContractSource, /NEP-11/);
  assert.match(createContractSource, /Oracle/);
  assert.match(createContractSource, /Ownable/);
});
