using System.Diagnostics;
using System.Runtime.CompilerServices;

public enum LogLevel
{
	Error = 0,
	Message,
	Info,
	Verbose,
	Debug,
	None
}

public static partial class Globals
{
	public static LogLevel MaxLogLevel = LogLevel.Verbose;
    public static MM.CommandArguments Args = null;
    public static string DefaultCommand = null;
    public static bool ExitWhenAssertFail = true;
    public static IList<MM.ActionItem> Actions = new List<MM.ActionItem>();
    public static Action OnFailAction = null;
}

public static partial class MM
{
    public static void WriteLine(string message, LogLevel logLevel = LogLevel.Message)
    {
        var level = (int)logLevel;
        if ((int)Globals.MaxLogLevel < level) return;
        
        switch (level)
        {
            case 0: Console.ForegroundColor = ConsoleColor.Red; break;  // Error
            case 1: Console.ForegroundColor = ConsoleColor.White; break; // Message
            case 2: Console.ForegroundColor = ConsoleColor.Yellow; break;  // Info
            case 3: Console.ForegroundColor = ConsoleColor.DarkGreen; break; // Verbose
            case 4: Console.ForegroundColor = ConsoleColor.DarkCyan; break; // Debug
            default: return;
        }

        var offset = new String(' ', level * 2);
        Console.WriteLine(offset + message.Replace("[0m", string.Empty).Replace("[32m", string.Empty).Replace("[39m", string.Empty).Replace("[94m", string.Empty).Replace("[96m", string.Empty).Replace(Environment.NewLine, Environment.NewLine + offset));
    }

    public static void EnsureDirectoryExists(string pathToDirectory)
    {
        if (!Directory.Exists(pathToDirectory))
        {
            Directory.CreateDirectory(pathToDirectory);
        }
    }

    public static void CleanDirectory(string pathToDirectory)
    {
        DeleteAssets(pathToDirectory);
        Directory.CreateDirectory(pathToDirectory);
    }

    public static void DeleteAssets(params string[] paths)
    {
        foreach (var path in paths)
        {
            if (File.Exists(path))
            {
                WriteLine($"Delete file {path}.", LogLevel.Verbose);
                File.Delete(path);
            }
            else if (Directory.Exists(path))
            {
                WriteLine($"Delete directory {path}.", LogLevel.Verbose);
                Directory.Delete(path, true);
            }
        }
    }

    public struct ActionItem
    {
        public ActionItem(string name, string description, string[] dependsOn, System.Action execute)
        {
            Name = name;
            Description = description;
            DependsOn = dependsOn;
            Execute = execute;
            Executed = false;
        }

        public string Name;
        public string Description;
        public string[] DependsOn;
        public System.Action Execute;
        public bool Executed;
    }

    public static void Action(string commandName, System.Action action, string description = null, params string[] dependsOn) =>
        Action(new ActionItem(commandName, description, dependsOn, action));

    public static void Action(string commandName, System.Action action, string description = null) =>
        Action(new ActionItem(commandName, description, null, action));

    public static void Action(ActionItem item) => Globals.Actions.Add(item);

    public static void Run(string name = null)
    {
        var commandName = name ?? Globals.Args.Command;
        var item = Globals.Actions.FirstOrDefault(it => it.Name == commandName);
        
        Assert.Truthy(item.Name != null, $"Action with name {commandName} was not found!");

        if (item.Executed) return;
        if (item.DependsOn != null && item.DependsOn.Length > 0)
        {
            foreach (var subname in item.DependsOn)
            {
                Run(subname);
            }
        }

        if (item.Description != null) WriteLine(item.Description);
        Stopwatch watch = new Stopwatch();
        watch.Start();
        try
        {
            item.Execute();
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }

        item.Executed = true;
        watch.Stop();
        WriteLine($"...Done! ({watch.ElapsedMilliseconds} ms)");
    }

    public static string GetScriptFolder([CallerFilePath] string path = null) => Path.GetDirectoryName(path);

    public static class Assert
    {
        public static void Truthy(bool condition, string error, string description = "The provided value must be true.")
        {
            WriteLine(description, LogLevel.Debug);
            if (!condition) Fail(error);
        }

        public static void Fail(string error)
        {
            WriteLine(string.Empty);
            WriteLine(error, LogLevel.Error);
            if (Globals.ExitWhenAssertFail)
            {
                WriteLine("FAIL");
                try 
                {
                    Globals.OnFailAction?.Invoke();
                }
                finally
                {
                    Environment.Exit(1);
                }
            }
        }
    }

    public sealed class CommandArguments
    {
        private string[] _keys;
        private string[] _values;
        private string _command;
        
        public string Command => _command ?? Globals.DefaultCommand;

        public string this[string key]
        {  
            get
            {
                if (_keys == null) return null;
                var index = Array.IndexOf(_keys, key);
                if (index < 0) return null;
                return _values[index];
            }
        }

        public bool Has(string key) => _keys != null && Array.IndexOf(_keys, key) > -1;
        
        public CommandArguments(IList<string> args)
        {
            var count = args.Count;
            if (count > 0)
            {
                _command = args[0];
            }

            if (args.Contains("/debug"))
            {
                Globals.MaxLogLevel = LogLevel.Debug;
            }
            
            if (count > 1)
            {
                var size = count - 1;
                _keys = new string[size];
                _values = new string[size];
                
                for (var index = 1; index < count; index++)
                {
                    var arg = args[index];
                    WriteLine($"Parsing argument {arg}.", LogLevel.Debug);
                    if (arg[0] != '/') { 
                        WriteLine("Invalid argument " + arg + " specify /key:value.", LogLevel.Error);
                        WriteLine("EXIT");
                        Environment.Exit(1);
                    }

                    var indexOfSeparator = arg.IndexOf(':');
                    var hasValue = indexOfSeparator > -1;
                    var key   = _keys[index - 1]   = hasValue ? arg.Substring(1, indexOfSeparator - 1) : arg.Substring(1);
                    var value = _values[index - 1] = hasValue ? arg.Substring(indexOfSeparator + 1)    : null;
                    WriteLine($"Valid argument {key} with value {value}", LogLevel.Debug);
                }
            }
        }
    }
}

Globals.Args = new MM.CommandArguments(Args);