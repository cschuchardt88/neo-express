import * as vscode from 'vscode';

class ExpressWalletTreeDataProvider {
    constructor() {
        this.a = [
            new vscode.TreeItem('HELLO1', vscode.TreeItemCollapsibleState.Collapsed),
            new vscode.TreeItem('HELLO2', vscode.TreeItemCollapsibleState.Collapsed),
            new vscode.TreeItem('HELLO3', vscode.TreeItemCollapsibleState.Collapsed),
            new vscode.TreeItem('HELLO4', vscode.TreeItemCollapsibleState.Collapsed),
            new vscode.TreeItem('HELLO5', vscode.TreeItemCollapsibleState.Collapsed),
            new vscode.TreeItem('HELLO6', vscode.TreeItemCollapsibleState.Collapsed),
            new vscode.TreeItem('HELLO7', vscode.TreeItemCollapsibleState.Collapsed),
        ];
    }

    getTreeItem(element) {
        return {
            command: {
                command: 'neo-express-toolkit.helloWorld',
                title: `Say123`,
            },
            description: `Say Hello World`,
            label: `Hello World`,
            tooltip: `Hello\nWorld`,
        };
    }

    getChildren(element) {
        return element ? [] : this.a;
    }
}

export {
    ExpressWalletTreeDataProvider,
}
