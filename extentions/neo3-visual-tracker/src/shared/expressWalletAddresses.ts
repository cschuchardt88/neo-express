export function isHiddenExpressConfig(filePath: string): boolean {
  const normalized = filePath.replace(/\\/g, "/").toLowerCase();
  return (
    normalized.includes("/test/") ||
    normalized.includes("/tests/") ||
    normalized.includes("/obj/") ||
    normalized.includes("/bin/") ||
    normalized.endsWith("tests.neo-express")
  );
}

export function parseExpressWalletAddresses(config: {
  wallets?: Array<{
    name?: string;
    accounts?: Array<{
      label?: string | null;
      "is-default"?: boolean;
      "script-hash"?: string;
    }>;
  }>;
  "consensus-nodes"?: Array<{
    wallet?: {
      name?: string;
      accounts?: Array<{
        label?: string | null;
        "is-default"?: boolean;
        "script-hash"?: string;
      }>;
    };
  }>;
}): { [name: string]: string } {
  const result: { [name: string]: string } = {};
  const add = (name: string | undefined, scriptHash: string | undefined) => {
    if (!name || !scriptHash || result[name]) {
      return;
    }
    result[name] = scriptHash;
  };

  for (const wallet of config.wallets ?? []) {
    const accounts = wallet.accounts ?? [];
    const defaultAccount =
      accounts.find((account) => account["is-default"]) ?? accounts[0];
    add(wallet.name, defaultAccount?.["script-hash"]);
    for (const account of accounts) {
      if (account.label) {
        add(account.label, account["script-hash"]);
      }
    }
  }

  for (const node of config["consensus-nodes"] ?? []) {
    const wallet = node.wallet;
    const accounts = wallet?.accounts ?? [];
    for (const account of accounts) {
      if (account.label === "Consensus MultiSigContract") {
        add("genesis", account["script-hash"]);
      } else if (account["is-default"] || !account.label) {
        add(wallet?.name, account["script-hash"]);
      } else {
        add(account.label, account["script-hash"]);
      }
    }
  }

  return result;
}

export function workspaceWalletDisplayName(
  filePath: string,
  accountLabel?: string | null
): string {
  const slash = Math.max(filePath.lastIndexOf("/"), filePath.lastIndexOf("\\"));
  const fileName = slash >= 0 ? filePath.slice(slash + 1) : filePath;
  const base = fileName
    .replace(/\.neo-wallet\.json$/i, "")
    .replace(/\.json$/i, "");
  if (accountLabel && accountLabel !== "Default account") {
    return accountLabel;
  }
  return base || accountLabel || "wallet";
}

export function contractsWithStandard(
  manifests: { [name: string]: { supportedstandards?: string[] } },
  standard: string
): string[] {
  const needle = standard.replace("-", "").toUpperCase();
  const names = new Set<string>();
  for (const [name, manifest] of Object.entries(manifests)) {
    if (name.startsWith("0x")) {
      continue;
    }
    const standards = manifest.supportedstandards ?? [];
    if (
      standards.some(
        (entry) => entry.replace("-", "").toUpperCase() === needle
      )
    ) {
      names.add(name);
    }
  }
  return [...names].sort((a, b) => a.localeCompare(b));
}

export function workspaceNep6AccountNames(accountSigners?: {
  [name: string]: string;
}): string[] {
  if (!accountSigners) {
    return [];
  }
  return Object.keys(accountSigners).filter(
    (name) => accountSigners[name] !== name
  );
}

export function resolveAccountForIdentifier(
  displayName: string,
  signer: string,
  identifierWallets: { [name: string]: string }
): string {
  return identifierWallets[displayName] || signer;
}
