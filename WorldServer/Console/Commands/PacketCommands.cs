using System.Reflection;
using Framework.Console.Commands;
using Framework.Logging;
using WorldServer.Game.Managers;
using Framework.Network.Packets;

namespace WorldServer.Console.Commands
{
    public sealed class Packet : CommandBase
    {
        [CommandAttribute]
        public Packet()
        {
            base.LoadSubCommands(this.GetType());
        }

        [SubCommandAttribute]
        public static bool SendString(string[] args)
        {
            var pTarget = Game.Globals.WorldMgr.GetSession(Read<string>(args, 1));
            if (pTarget == null || args.Length < 3)
                return false;

            string hexValue = string.Format("{0:X}", Read<string>(args, 2));
            ushort opc = ushort.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
            PacketWriter packet = new PacketWriter((ushort)opc);
            string packetContent = string.Format("{0:X}", Read<string>(args, 3));
            packet.WriteHexStringBytes(packetContent);
            pTarget.Send(ref packet);

            return true;
        }
    }
}
