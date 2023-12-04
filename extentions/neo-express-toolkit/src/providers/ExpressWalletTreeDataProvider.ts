import * as vscode from 'vscode';

export class ExpressWalletTreeDataProvider implements vscode.TreeDataProvider<ExpressWalletTreeItem> {
    private _onDidChangeTreeData: vscode.EventEmitter<ExpressWalletTreeItem | undefined | void> = new vscode.EventEmitter<ExpressWalletTreeItem | undefined | void>();
    readonly onDidChangeTreeData: vscode.Event<ExpressWalletTreeItem | undefined | void> = this._onDidChangeTreeData.event;

    data: ExpressWalletTreeItem[];

    constructor() {
        this.data = [new ExpressWalletTreeItem('Wallet1')];
    }

    register(context: vscode.ExtensionContext): void {
        context.subscriptions.push(
            vscode.window.registerTreeDataProvider(
                'neo-express-toolkit.views.wallet',
                this
            )
        );
    }

    refresh(): void {
        this._onDidChangeTreeData.fire();
    }

    getTreeItem(element: ExpressWalletTreeItem): vscode.TreeItem | Thenable<vscode.TreeItem> {
        return element;
    }

    getChildren(element?: ExpressWalletTreeItem | undefined): vscode.ProviderResult<ExpressWalletTreeItem[]> {
        if (element === undefined) {
            return this.data;
        }
        return element.children;
    }
}

class ExpressWalletTreeItem extends vscode.TreeItem {
    children: ExpressWalletTreeItem[] | undefined;

    constructor(label: string, children?: ExpressWalletTreeItem[]) {
        super(
            label,
            children === undefined ? vscode.TreeItemCollapsibleState.None : vscode.TreeItemCollapsibleState.Expanded
        );

        this.children = children;
    }
}
