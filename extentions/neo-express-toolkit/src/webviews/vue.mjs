import * as vscode from 'vscode';
import * as path from 'path';

/**
 * @param {vscode.ExtensionContext} context
 */
function prepareWebView(context) {
	const panel = vscode.window.createWebviewPanel(
		'vueWebview',
		'vue webview',
		vscode.ViewColumn.One,
		{
			enableScripts: true,
			localResourceRoots: [
				vscode.Uri.file(
					path.join(context.extensionPath, 'dist', 'controls', 'assets')
				),
			],
		},
	);

	const depsNameList = [
		'index.js',
		'index.css',
	];

	const depsList = depsNameList.map((item) =>
		panel.webview.asWebviewUri(
			vscode.Uri.file(
				path.join(context.extensionPath, 'dist', 'controls', 'assets', item)
			)
		)
	);

	const html = `<!DOCTYPE html>
	<html lang="en">
	<head>
	  <meta charset="UTF-8" />
	  <link rel="icon" href="/favicon.ico" />
	  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
	  <title>Vite App</title>
	  <script>
			const vscode = acquireVsCodeApi();
	  </script>
	  <script type="module" crossorigin src="${depsList[0]}"></script>
	  <link rel="modulepreload" href="${depsList[0]}">
	  <link rel="stylesheet" href="${depsList[1]}">
	</head>
	<body>
	  <div id="app"></div>
	</body>
	</html>`;

	panel.webview.html = html;
	return panel;
}

export {
	prepareWebView,
}
