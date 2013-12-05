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
using System.Globalization;
using Framework.Logging;
using System.Reflection;
using System.Collections.Generic;

namespace Framework.Console.Commands
{
    public abstract class CommandBase
    {
        private Dictionary<string, HandleCommand> subCommands;

        public virtual void LoadSubCommands(Type type)
        {
            subCommands = new Dictionary<string, HandleCommand>();
            foreach (var method in type.GetMethods())
            {
                foreach (var attr in method.GetCustomAttributes<SubCommandAttribute>())
                {
                    if (attr != null)
                    {
                        subCommands[method.Name.ToLower()] =
                            (HandleCommand)Delegate.CreateDelegate(typeof(HandleCommand), method);
                    }
                }
            }
            CommandManager.CommandHandlers[type.Name.ToLower()] = subCommands;
        }

        public static T Read<T>(string[] args, int index)
        {
            try
            {
                return (T)Convert.ChangeType(args[index], typeof(T), CultureInfo.GetCultureInfo("en-US").NumberFormat);
            }
            catch
            {
                //Log.Message(LogType.Error, "Wrong arguments for the current command!!!");
            }

            return default(T);
        }

        public static void GetSubCommands(string command)
        {
            Log.Message(LogType.Init, "Available subcommands for command \"{0}\":", command);
            Dictionary<string, HandleCommand> subCommands;
            CommandManager.CommandHandlers.TryGetValue(command, out subCommands);
            foreach (string key in subCommands.Keys)
                Log.Message(LogType.Init, "\"{0}\"", key);
        }
    }
}

