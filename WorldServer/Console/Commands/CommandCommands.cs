using System.Reflection;
using Framework.Console.Commands;
using Framework.Logging;

namespace WorldServer.Console.Commands
{
    public sealed class Command : CommandBase
    {
        [CommandAttribute]
        public Command()
        {
            base.LoadSubCommands(this.GetType());
        }

        [SubCommandAttribute]
        public static bool List(string[] args)
        {
            Log.Message(LogType.Init, "Available commands: ");
            foreach (string Command in CommandManager.CommandHandlers.Keys)
            {
                Log.Message(LogType.Init, "{0}", Command);
            }
            return true;
        }
    }
}
