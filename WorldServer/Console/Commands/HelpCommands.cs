using Framework.Console.Commands;
using System;
using System.Collections.Generic;

namespace WorldServer.Console.Commands
{
    public class Help : CommandBase
    {
        [CommandAttribute]
        public Help()
        {
            base.LoadSubCommands(this.GetType());
        }

        [SubCommand]
        public static bool Command(string[] args)
        {
            string command = Read<string>(args, 1);

            if (command == null)
                return false;

            foreach (var key in CommandManager.CommandHandlers.Keys)
                if (key.StartsWith(command))
                    command = key;

            if (!CommandManager.CommandHandlers.ContainsKey(command))
            {
                System.Console.WriteLine("{0} is not valid command", command);
                return false;
            }

            Dictionary<string, HandleCommand> value = new Dictionary<string, HandleCommand>();
            CommandManager.CommandHandlers.TryGetValue(command, out value);
            string subcommand = Read<string>(args, 2);

            if (subcommand == null)
                return false;

            foreach (var key in value.Keys)
                if (key.StartsWith(subcommand))
                    subcommand = key;

            if (!value.ContainsKey(subcommand))
                GetSubCommands(command);

            if (CommandManager.CommandHelper.ContainsKey(command + " " + subcommand))
                System.Console.WriteLine("Command: \"{0}\" Subcommand: \"{1}\" Syntax: \"{2}\"", command, subcommand, CommandManager.CommandHelper[command + " " + subcommand]);
            else
                return false;

            return true;
        }

    }
}
