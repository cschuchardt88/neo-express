import assert from "node:assert/strict";
import { readFileSync } from "node:fs";
import { join } from "node:path";
import test from "node:test";

test("accountChoices reads wallets from the selected Express identifier", () => {
  const source = readFileSync(join(__dirname, "neoExpressCommands.ts"), "utf8");
  assert.match(
    source,
    /export async function accountChoices\(\s*identifier: BlockchainIdentifier\s*\)/
  );
  assert.match(
    source,
    /identifier\.getWalletAddresses\(\)/
  );
  assert.doesNotMatch(
    source,
    /accountChoices\([\s\S]*autoComplete\?\.data\.wellKnownAddresses/
  );
});
