#load "common.csx"

using System.Diagnostics;
using System.Runtime.InteropServices;

public static partial class MM
{
    public class Command
    {
        private readonly StringBuilder lastStandardErrorOutput = new StringBuilder();
        private readonly StringBuilder lastStandardOutput = new StringBuilder();
        private readonly Process process = new Process();

        public Command(string commandPath, string arguments, string workingDirectory, LogLevel outputLogLevel = LogLevel.Verbose)
        {
            WriteLine($"{commandPath} {arguments}", LogLevel.Debug);

            process.StartInfo = new ProcessStartInfo(commandPath);
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;

            if (!string.IsNullOrEmpty(workingDirectory))
            {
                process.StartInfo.WorkingDirectory = workingDirectory;
            }

            process.ErrorDataReceived += (s, e) => Log(lastStandardErrorOutput, e.Data, outputLogLevel);
            process.OutputDataReceived += (s, e) => Log(lastStandardOutput, e.Data, outputLogLevel);
            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();

            WriteLine("----------------------", outputLogLevel);
        }

        /// <summary>Wait for the process to end end return the output as string.</summary>
        public void WaitForResult() => process.WaitForExit();

        /// <summary>Gets the exit code.</summary>
        public int ExitCode => process.ExitCode;

        /// <summary>Gets the standard output.</summary>
        public string Output => lastStandardOutput.ToString().Trim();

        /// <summary>Assert the process exit code is not the allowed exit code.</summary>
        public void FailWhenExitCode(int allowedExitCode) =>
            Assert.Truthy(process.ExitCode == allowedExitCode, lastStandardErrorOutput.ToString().Trim(), "Check if exit code is " + allowedExitCode.ToString());

        /// <summary>Execute command and verify the exit code.</summary>
        public static string Execute(string commandPath, string arguments, string workingDirectory = null, LogLevel outputLogLevel = LogLevel.Verbose)
        {
            var command = CreateAndWait(commandPath, arguments, workingDirectory, outputLogLevel);
            command.FailWhenExitCode(0);
            return command.Output;
        }

        /// <summary>Execute command and wait for exit.</summary>
        public static Command CreateAndWait(string commandPath, string arguments, string workingDirectory = null, LogLevel outputLogLevel = LogLevel.Verbose)
        {
            var command = new Command(commandPath, arguments, workingDirectory, outputLogLevel);

            command.WaitForResult();

            WriteLine($"Exit with status code " + command.ExitCode.ToString(), LogLevel.Verbose);

            return command;
        }

        private static void Log(StringBuilder output, string message, LogLevel level)
        {
            var msg = message?.TrimEnd() ?? string.Empty;
            WriteLine(msg, level);
            output.AppendLine(msg);
        }
    }

    /// <summary>Execute a shell command in any OS.</summary>
    public static Command ShellCommand(string command, string workingDirectory = null, LogLevel outputLogLevel = LogLevel.Verbose)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return Command.CreateAndWait("/bin/sh", "-c \"" + command.Replace("\"", "\\\"") + "\"", workingDirectory, outputLogLevel);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return Command.CreateAndWait("cmd.exe", "/C " + command, workingDirectory, outputLogLevel);
        }
        
        throw new Exception("Not supported OS.");
    }

    /// <summary>Execute a shell command in any OS.</summary>
    public static string Shell(string command, string workingDirectory = null, LogLevel outputLogLevel = LogLevel.Verbose)
    {
        var com = ShellCommand(command, workingDirectory, outputLogLevel);
        com.FailWhenExitCode(0);
        return com.Output;
    }

    public static string ShellInstall(string testcommand, string installcommand, string workingDirectory = null, LogLevel outputLogLevel = LogLevel.Verbose)
    {
        var command = ShellCommand(testcommand, workingDirectory, outputLogLevel);        
        if (command.ExitCode == 0)
        {
            return null;
        }

        return Shell(installcommand, workingDirectory);
    }
}