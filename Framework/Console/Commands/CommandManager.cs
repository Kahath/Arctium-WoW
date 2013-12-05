/*
 * Copyright (C) 2012-2013 Arctium <http://arctium.org>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Framework.Logging;
using System.Linq;

namespace Framework.Console.Commands
{
    public delegate bool HandleCommand(string[] args);

    public class CommandManager
    {
        public static Dictionary<string, Dictionary<string, HandleCommand>> CommandHandlers = new Dictionary<string, Dictionary<string, HandleCommand>>();

        public static Dictionary<string, string> CommandHelper = new Dictionary<string, string>();

        public static void DefineCommands()
        {
            var currentAsm = Assembly.GetEntryAssembly();
            foreach (var type in currentAsm.GetTypes())
            {
                foreach (var constructor in type.GetConstructors())
                {
                    foreach (var commandAttr in constructor.GetCustomAttributes<CommandAttribute>())
                    {
                        if (commandAttr != null)
                        {
                            constructor.Invoke(null);
                        }
                    }

                }
            }
        }

        static void LoadDBCommands()
        {
            Log.Message(LogType.Normal, "Loading Commands from DB");
            using (var result = Database.DB.World.Select("SELECT * FROM `commands`"))
            {
                for (int i = 0; i < result.Count; i++)
                {
                    CommandHelper[result.Read<string>(i, "name")] = result.Read<string>(i, "help");
                }
                if (result.Count > 0)
                    Log.Message(LogType.Normal, "Loaded {0} commands", result.Count);
                else
                    Log.Message(LogType.Error, "Loaded 0 commands from database");
            }
        }

        public static void InitCommands()
        {
            DefineCommands();
            LoadDBCommands();

            while (true)
            {
                Thread.Sleep(1);
                Log.Message(LogType.Cmd, "World >> ");
                string[] line = System.Console.ReadLine().Split(new string[] { " " }, StringSplitOptions.None);
                string[] args = new string[line.Length - 1];
                Array.Copy(line, 1, args, 0, line.Length - 1);
                string[] newArray = args.Where(str => str != "").ToArray();

                if (line.Count() > 0)
                    if (line[0].Trim() != "")
                        InvokeHandler(line[0].ToLower(), newArray);
            }
        }

        static void InvokeHandler(string command, params string[] args)
        {
            Dictionary<string, HandleCommand> value;
            foreach (string com in CommandHandlers.Keys)
            {
                if (com.StartsWith(command))
                {
                    command = com;
                    break;
                }
            }
            if (CommandHandlers.ContainsKey(command))
            {
                if (args.Length < 1)
                {
                    CommandBase.GetSubCommands(command);
                    return;
                }
                if (args[0].Trim() == "")
                {
                    CommandBase.GetSubCommands(command);
                    return;
                }
                value = new Dictionary<string, HandleCommand>();
                CommandHandlers.TryGetValue(command, out value);
                foreach (string subcom in value.Keys)
                {
                    if (subcom.StartsWith(args[0]))
                    {
                        args[0] = subcom;
                        break;
                    }
                }
                if (value.ContainsKey(args[0]))
                {
                    if (!value[args[0]].Invoke(args))
                    {
                        if (CommandHelper.ContainsKey(command + " " + args[0]))
                            System.Console.WriteLine("Wrong Syntax!! Use: {0}", CommandHelper[command + " " + args[0]]);
                    }
                }
                else
                {
                    Log.Message(LogType.Error, "\"{0}\" is not valid console sub command for command \"{1}\"", args[0], command);
                    CommandBase.GetSubCommands(command);
                }
            }


            else
                Log.Message(LogType.Error, "\"{0}\" is not valid console command", command);
        }
    }
}
