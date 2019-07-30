﻿using McMaster.Extensions.CommandLineUtils;
using Neo.SmartContract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Neo.Express.Commands
{
    internal partial class ContractCommand
    {
        [Command(Name = "invoke")]
        private class Invoke
        {
            [Argument(0)]
            [Required]
            string Contract { get; }

            [Argument(1)]
            string[] Arguments { get; }

            [Option]
            private string Input { get; }

            [Option]
            private string Account { get; }

            [Option]
            private string Function { get; }

            static IEnumerable<ContractParameter> ParseArguments(DevContractFunction function, IEnumerable<string> arguments)
            {
                if (function.Parameters.Count != arguments.Count())
                {
                    throw new ApplicationException($"Invalid number of arguments. Expecting {function.Parameters.Count} received {arguments.Count()}");
                }

                return function.Parameters.Zip(arguments,
                    (paramTuple, argValue) =>
                    {
                        var p = new ContractParameter(paramTuple.type);
                        p.SetValue(argValue);
                        return p;
                    });
            }

            IEnumerable<ContractParameter> ParseArguments(DevContract contract)
            {
                var arguments = Arguments ?? Enumerable.Empty<string>();

                if (string.IsNullOrEmpty(Function))
                {
                    var entrypoint = contract.Functions.Single(f => f.Name == contract.EntryPoint);
                    return ParseArguments(entrypoint, arguments);
                }
                else
                {
                    var function = contract.Functions.SingleOrDefault(f => f.Name == Function);

                    if (function == null)
                    {
                        throw new Exception($"Could not find function {Function}");
                    }

                    return new ContractParameter[2]
                    {
                        new ContractParameter(Function),
                        new ContractParameter(ParseArguments(function, arguments))
                    };
                }
            }

            async Task<int> OnExecuteAsync(CommandLineApplication app, IConsole console)
            {
                try
                {
                    var (devChain, _) = DevChain.Load(Input);
                    var contract = devChain.Contracts.SingleOrDefault(c => c.Name == Contract);
                    if (contract == default)
                    {
                        throw new Exception($"Contract {Contract} not found.");
                    }

                    var account = devChain.GetAccount(Account);

                    var args = ParseArguments(contract);
                    var uri = devChain.GetUri();
                    var result = await NeoRpcClient.ExpressInvokeContract(uri, contract.Hash, args, account?.ScriptHash);
                    console.WriteLine(result.ToString(Formatting.Indented));

                    return 0;
                }
                catch (Exception ex)
                {
                    console.WriteLine(ex.Message);
                    app.ShowHelp();
                    return 1;
                }
            }
        }
    }
}
