using System.IO;
using System.IO.Abstractions;
using McMaster.Extensions.CommandLineUtils;

namespace NeoExpress
{
    class TransactionExecutorFactory : ITransactionExecutorFactory
    {
        readonly IFileSystem fileSystem;
        readonly IConsole console;

        public TransactionExecutorFactory(IFileSystem fileSystem, IConsole console)
        {
            this.fileSystem = fileSystem;
            this.console = console;
        }

        public ITransactionExecutor Create(IExpressChainManager chainManager, bool trace, bool json)
        {
            return new TransactionExecutor(fileSystem, chainManager, trace, json, console.Out);
        }
    }
}