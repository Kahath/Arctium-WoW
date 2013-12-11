using Framework.Constants;
using Framework.Constants.NetMessage;
using Framework.Network.Packets;
using Framework.ObjectDefines;
using WorldServer.Network;
using Framework.Logging;
using WorldServer.Game.WorldEntities;
using System.Collections.Generic;

namespace WorldServer.Game.Packets.PacketHandler
{
    public class GroupHandler : Globals
    {
        [Opcode(ClientMessage.GroupUninvite, "17538")]
        public static void HandleGroupUninvite(
            ref PacketReader packet, WorldClass session)
        {
            bool[] guidMask = new bool[8];
            packet.Skip(1);

            BitUnpack BitUnpack = new BitUnpack(packet);

            guidMask[0] = BitUnpack.GetBit();

            BitUnpack.GetBits<byte>(8);

            for (int i = 1; i < 8; i++)
                guidMask[i] = BitUnpack.GetBit();

            ulong GUID = BitUnpack.GetPackedValue(guidMask,
                new byte[] { 3, 4, 1, 7, 2, 0, 5, 6 },
                new byte[] { 0, 4, 5, 7, 2, 3, 1, 6 });

            session.Character.Group.Uninvite(GUID);
        }

        [Opcode(ClientMessage.GroupChangeRaidDifficulty, "17538")]
        public static void HandleGroupChangeRaidDifficulty(
            ref PacketReader packet, WorldClass session)
        {
            session.Character.Group.ChangeGroupRaidDifficulty((GroupDungeonDifficulty)packet.ReadUInt32());
            session.Character.Group.Update();
        }

        [Opcode(ClientMessage.GroupMoveRaidMember, "17538")]
        public static void HandleGroupMoveRaidMember(
            ref PacketReader packet, WorldClass session)
        {
            packet.Skip(1);
            var group = packet.ReadByte();

            BitUnpack BitUnpack = new BitUnpack(packet);

            ulong GUID = BitUnpack.GetPackedValue(
                new byte[] { 3, 4, 2, 5, 1, 6, 7, 0 },
                new byte[] { 6, 3, 7, 5, 1, 4, 2, 0 });

            var pSession = WorldMgr.GetSession(GUID);

            if (pSession == null)
                return;

            pSession.Character.Group.ChangeRaidGroup(pSession.Character.Guid, (GroupRaidGroups)group);
            session.Character.Group.Update();
        }
        
        [Opcode(ClientMessage.GroupPromoteAssist, "17538")]
        public static void HandleGroupRaidRole(
            ref PacketReader packet, WorldClass session)
        {
            bool[] guidMask = new bool[8];
            packet.Skip(1);
            BitUnpack BitUnpack = new BitUnpack(packet);

            for (int i = 0; i < 4; i++)
                guidMask[i] = BitUnpack.GetBit();

            byte raidRole = (byte)(BitUnpack.GetBit() ? 1 : 0);

            for (int i = 4; i < 8; i++)
                guidMask[i] = BitUnpack.GetBit();


            ulong GUID = BitUnpack.GetPackedValue(guidMask,
                new byte[] { 7, 5, 4, 1, 6, 3, 0, 2 },
                new byte[] { 1, 2, 7, 3, 4, 5, 0, 6 });

            Log.Message(LogType.Error, "{0}", GUID);

            var pSession = WorldMgr.GetSession(GUID);

            if (pSession == null)
                return;

            pSession.Character.Group.ChangeRaidRole(pSession.Character.Guid, raidRole);
            session.Character.Group.Update();
        }
        
        // Doesn't work while in raid group
        [Opcode(ClientMessage.GroupChangeDungeonDifficulty, "17538")]
        public static void HandleGroupDifficulty(
            ref PacketReader packet, WorldClass session)
        {
            session.Character.Group.ChangeGroupDungeonDifficulty((GroupDungeonDifficulty)packet.ReadUInt32());
            session.Character.Group.Update();
        }

        [Opcode(ClientMessage.GroupConvert, "17538")]
        public static void HandleGroupConvert(
            ref PacketReader packet, WorldClass session)
        {
            var groupType = packet.ReadByte() == 0x00 ? GroupType.Normal : GroupType.Raid;

            session.Character.Group.ChangeGroupType(groupType);

            // atm
            session.Character.Group.RaidDifficulty = GroupDungeonDifficulty.None;
            session.Character.Group.DungeonDifficulty = GroupDungeonDifficulty.None;

            session.Character.Group.Update();
        }

        [Opcode(ClientMessage.GroupPromoteLeader, "17538")]
        public static void HandleGroupLeaderChange
            (ref PacketReader packet, WorldClass session)
        {
            // unk 
            packet.Skip(1); // 7F

            BitUnpack BitUnpack = new BitUnpack(packet);
            ulong leaderGUID = BitUnpack.GetPackedValue(
                new byte[] { 5, 1, 7, 0, 4, 6, 2, 3 },
                new byte[] { 5, 7, 1, 6, 2, 4, 0, 3 });

            var newLeader = WorldMgr.GetSession(leaderGUID);

            if (!(newLeader == null))
                session.Character.Group.ChangeGroupLeader(newLeader.Character.Guid);

            session.Character.Group.Update();
        }

        [Opcode(ClientMessage.GroupLootUpdate, "17538")]
        public static void HandleGroupLootUpdate
             (ref PacketReader packet, WorldClass session)
        {
            // unk
            packet.Skip(1); // 7F

            session.Character.Group.ChangeGroupLootMethod((GroupLootMethod)packet.ReadByte());
            session.Character.Group.ChangeGroupLootThreshold((GroupLootThreshold)packet.ReadUInt32());

            BitUnpack BitUnpack = new BitUnpack(packet);

            ulong GUID = BitUnpack.GetPackedValue(
                new byte[] { 0, 2, 5, 3, 6, 7, 4, 1 },
                new byte[] { 4, 7, 6, 3, 5, 1, 2, 0 });

            Log.Message(LogType.Error, "GUID: {0}", GUID);
            var pChar = WorldMgr.GetSession(GUID);

            if (!(pChar == null))
            {
                session.Character.Group.ChangeGroupLooterGuid(pChar.Character.Guid);
            }
            else if (session.Character.Group.LootMethod != GroupLootMethod.MasterLoot)
            {
                session.Character.Group.ChangeGroupLooterGuid(0);
            }

            session.Character.Group.Update();
        }

        [Opcode(ClientMessage.GroupMemberRole, "17538")]
        public static void HandleGroupMemberRole(
            ref PacketReader packet, WorldClass session)
        {
            // unk
            packet.Skip(1); // 7F

            var groupRole = (GroupMemberRole)packet.ReadUInt32();
            BitUnpack BitUnpack = new BitUnpack(packet);

            ulong GUID = BitUnpack.GetPackedValue(
                new byte[] { 0, 5, 2, 4, 7, 3, 1, 6 },
                new byte[] { 4, 6, 2, 5, 3, 7, 0, 1 });

            session.Character.Group.ChangeGroupMemberRole(GUID, groupRole);
            session.Character.Group.Update();
        }

        [Opcode(ClientMessage.GroupLeave, "17538")]
        public static void HandleGroupLeave(
            ref PacketReader packet, WorldClass session)
        {
            session.Character.Group.Uninvite(session.Character.Guid);
        }

        [Opcode(ClientMessage.GroupInvite, "17538")]
        public static void HandleGroupRequest(ref PacketReader packet, WorldClass session)
        {
            var pChar = session.Character;
            BitUnpack BitUnpack = new BitUnpack(packet);
            packet.Skip(11);
            BitUnpack.GetBits<byte>(4);
            var length = BitUnpack.GetBits<byte>(6);
            var message = packet.ReadString(length);
            var pTarget = WorldMgr.GetSession(message);
            string Realm = "Outland"; // hardcoded atm

            PacketWriter writer = new PacketWriter(ServerMessage.GroupInvite);
            BitPack BitPack = new BitPack(writer, pChar.Guid);


            BitPack.WriteGuidMask(3);

            // upk 
            BitPack.Write(1);

            BitPack.WriteGuidMask(5, 7, 2, 0);

            // unk
            BitPack.Write(0, 22);

            // Char name Length
            BitPack.Write((byte)pChar.Name.Length, 6);

            // Is char in group
            BitPack.Write(pTarget.Character.IsInGroup() ? 0 : 1);

            BitPack.WriteGuidMask(4);

            // unk
            BitPack.Write(0);
            BitPack.Write(0);

            BitPack.WriteRealmLength((byte)Realm.Length); // 9 bit

            BitPack.WriteGuidMask(6, 2);

            // unk
            BitPack.Write(0, 5);

            BitPack.Flush();

            // unk
            writer.WriteUInt32(0);

            BitPack.WriteGuidBytes(6);
            writer.WriteString(Realm);
            BitPack.WriteGuidBytes(1, 2);

            // unk
            writer.WriteUInt32(0);

            writer.WriteString(pChar.Name);
            BitPack.WriteGuidBytes(0);
            writer.WriteUInt32(0x00);
            BitPack.WriteGuidBytes(7, 3, 5, 4);

            // unk
            writer.WriteHexStringBytes("EC0700000000");


            pTarget.Character.PendingInvite = pChar.Name;
            pTarget.Send(ref writer);
        }

        [Opcode(ClientMessage.GroupInviteResponse, "17538")]
        public static void HandleGroupRequestResponse
            (ref PacketReader packet, WorldClass session)
        {
            // unk
            packet.Skip(1); // 7F

            GroupInviteResponse response = (GroupInviteResponse)packet.ReadByte();

            switch (response)
            {
                case GroupInviteResponse.Accept:
                    {
                        Group group;
                        var pChar = session.Character;
                        var pSession = WorldMgr.GetSession(pChar.PendingInvite);
                        pChar.PendingInvite = null;

                        if (pSession == null)
                            return;

                        if (!pSession.Character.IsInGroup())
                            group = new Group(GroupMgr.GetGUID(), pSession.Character);
                        else
                            group = pSession.Character.Group;

                        if (group.IsFull())
                            return;

                        group.Add(pChar);
                        group.Update();

                        break;
                    }
                case GroupInviteResponse.Decline:
                    {
                        break;
                    }
            }
        }
    }
}
