// Copyright (C) 2015-2026 The Neo Project.
//
// NeoExpressBatch.cs file belongs to neo-express project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or https://opensource.org/license/MIT for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Neo.BuildTasks
{
    public class NeoExpressBatch : DotNetToolTask
    {
        const string PACKAGE_ID = "Neo.Express";
        const string COMMAND = "neoxp";
        const string RunningChainMessage = "Cannot run batch command while blockchain is running";

        protected override string Command => COMMAND;
        protected override string PackageId => PACKAGE_ID;

        [Required]
        public ITaskItem? BatchFile { get; set; }

        public ITaskItem? InputFile { get; set; }

        public bool Reset { get; set; }

        public ITaskItem? Checkpoint { get; set; }

        public bool Trace { get; set; }

        public bool StackTrace { get; set; }

        public override bool Execute()
        {
            if (BatchFile is null)
                throw new Exception("Missing BatchFile Property");

            var directory = WorkingDirectory;
            if (!FindTool(PackageId, directory, out var resolvedType, out var version))
            {
                Log.LogError("tool package {0} not found", PackageId);
                return false;
            }

            SetResolvedToolType(resolvedType);
            ToolVersion = version.ToString();
            Log.LogMessage(MessageImportance.High, "Found {0} tool package {1} version {2}",
                resolvedType, PackageId, version);

            var command = resolvedType == DotNetToolType.Global ? Command : "dotnet";
            var batchArguments = resolvedType == DotNetToolType.Global
                ? GetArguments()
                : Command + " " + GetArguments();
            CommandLine = $"{command} {batchArguments}";
            Log.LogMessage(MessageImportance.High, "Running {0} {1}", command, batchArguments);

            if (TryExecute(command, batchArguments, directory, out var output, logFailure: false))
            {
                ExecutionSuccess(output);
                return true;
            }

            if (IsChainRunningFailure(LastProcessResults))
            {
                Log.LogMessage(MessageImportance.High,
                    "Neo Express is running; deploying contracts from {0} without resetting the chain.",
                    BatchFile.ItemSpec);
                return DeployContractsWhileRunning(command, resolvedType, directory);
            }

            Log.LogError("{0} returned {1}", Command, LastProcessResults.ExitCode);
            foreach (var err in LastProcessResults.Error)
            {
                Log.LogError(err);
            }
            foreach (var @out in LastProcessResults.Output)
            {
                Log.LogWarning(@out);
            }
            return false;
        }

        protected override string GetArguments()
        {
            if (BatchFile is null)
                throw new Exception("Missing BatchFile Property");

            var builder = new StringBuilder("batch ");
            builder.AppendFormat("\"{0}\"", BatchFile.ItemSpec);

            if (!(InputFile is null))
            {
                builder.AppendFormat(" --input \"{0}\"", InputFile.ItemSpec);
            }

            if (Reset)
            {
                builder.Append(" --reset");
                if (!(Checkpoint is null))
                {
                    builder.AppendFormat(":\"{0}\"", Checkpoint.ItemSpec);
                }
            }

            if (Trace)
            { builder.Append(" --trace"); }
            if (StackTrace)
            { builder.Append(" --stack-trace"); }

            return builder.ToString();
        }

        internal static bool IsChainRunningFailure(ProcessResults results)
        {
            return results.Error.Concat(results.Output)
                .Any(line => line.IndexOf(RunningChainMessage, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        internal static IReadOnlyList<(string NefFile, string Account)> ParseDeployCommands(string batchText)
        {
            var deploys = new List<(string, string)>();
            using var reader = new StringReader(batchText);
            string? line;
            while ((line = reader.ReadLine()) is not null)
            {
                var trimmed = line.Trim();
                if (trimmed.Length == 0 || trimmed.StartsWith("#") || trimmed.StartsWith("//"))
                    continue;
                var parts = trimmed.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 4
                    && parts[0].Equals("contract", StringComparison.OrdinalIgnoreCase)
                    && parts[1].Equals("deploy", StringComparison.OrdinalIgnoreCase))
                {
                    deploys.Add((parts[2], parts[3]));
                }
            }
            return deploys;
        }

        bool DeployContractsWhileRunning(string command, DotNetToolType resolvedType, ITaskItem? directory)
        {
            var batchPath = BatchFile!.ItemSpec;
            if (!Path.IsPathRooted(batchPath) && directory is not null && !string.IsNullOrEmpty(directory.ItemSpec))
            {
                batchPath = Path.Combine(directory.ItemSpec, batchPath);
            }
            if (!File.Exists(batchPath))
            {
                Log.LogError("Batch file \"{0}\" could not be found", batchPath);
                return false;
            }

            var deploys = ParseDeployCommands(File.ReadAllText(batchPath));
            if (deploys.Count == 0)
            {
                Log.LogError("Neo Express is running, and {0} has no 'contract deploy' lines to apply.", batchPath);
                return false;
            }

            foreach (var (nefFile, account) in deploys)
            {
                var builder = new StringBuilder();
                if (resolvedType != DotNetToolType.Global)
                {
                    builder.Append(Command);
                    builder.Append(' ');
                }
                builder.AppendFormat("contract deploy \"{0}\" {1} --force", nefFile, account);
                if (InputFile is not null)
                {
                    builder.AppendFormat(" --input \"{0}\"", InputFile.ItemSpec);
                }
                if (Trace)
                {
                    builder.Append(" --trace");
                }

                var arguments = builder.ToString();
                Log.LogMessage(MessageImportance.High, "Running {0} {1}", command, arguments);
                if (!TryExecute(command, arguments, directory, out var output))
                {
                    return false;
                }
                ExecutionSuccess(output);
            }

            return true;
        }
    }
}
