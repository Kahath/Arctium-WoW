using Framework.Constants;
using Framework.Constants.NetMessage;
using Framework.Network.Packets;
using Framework.ObjectDefines;
using WorldServer.Network;
using Framework.Logging;
using Framework.Constants;
using WorldServer.Game.WorldEntities;
using System.Collections.Generic;

namespace WorldServer.Game.Packets.PacketHandler
{
    public class GroupHandler : Globals
    {
        static ulong PartyGUID = 1;

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
                session.Character.Group.Leader = newLeader.Character;

            GroupUpdate(session.Character.Group);
        }

        [Opcode(ClientMessage.GroupLootUpdate, "17538")]
        public static void HandleGroupLootUpdate
             (ref PacketReader packet, WorldClass session)
        {
            // unk
            packet.Skip(1); // 7F

            session.Character.Group.LootMethod = (GroupLootMethod)packet.ReadByte();
            session.Character.Group.LootThreshold = (GroupLootThreshold)packet.ReadUInt32();

            BitUnpack BitUnpack = new BitUnpack(packet);

            ulong GUID = BitUnpack.GetPackedValue(
                new byte[] { 0, 2, 5, 3, 6, 7, 4, 1 },
                new byte[] { 4, 7, 6, 3, 5, 1, 2, 0 });

            var pChar = WorldMgr.GetSession(GUID);

            if (!(pChar == null))
            {
                session.Character.Group.LooterGUID = pChar.Character.Guid;
            }
            else if (session.Character.Group.LootMethod != GroupLootMethod.MasterLoot)
            {
                session.Character.Group.LooterGUID = 0;
            }


            GroupUpdate(session.Character.Group);
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

            WorldMgr.GetSession(GUID).Character.GroupRole = (GroupMemberRole)groupRole;

            GroupUpdate(session.Character.Group);
        }

        [Opcode(ClientMessage.GroupLeave, "17538")]
        public static void HandleGroupLeave(
            ref PacketReader packet, WorldClass session)
        {
            GroupLeave(ref session);
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
            packet.Skip(1);
            GroupInviteResponse response = (GroupInviteResponse)packet.ReadByte();

            switch (response)
            {
                case GroupInviteResponse.Accept:
                    {
                        Group group;
                        var pChar = session.Character;
                        var pLeader = WorldMgr.GetSession(pChar.PendingInvite);
                        pChar.PendingInvite = null;

                        if (pLeader == null)
                            return;

                        if (!pLeader.Character.IsInGroup())
                        {
                            group = new Group(PartyGUID++, pLeader.Character);
                            group.Add(pLeader.Character);
                            group.LootMethod = GroupLootType.GroupLoot;
                            group.LootThreshold = GroupLootThreshold.Uncommon;
                            group.DungeonDifficulty = GroupDungeonDifficulty.FivePlayer;
                            group.Type = GroupType.Normal;
                        }
                        else
                            group = pLeader.Character.Group;

                        if (group.IsFull())
                            return;

                        group.Add(pChar);

                        GroupUpdate(group);

                        break;
                    }
                case GroupInviteResponse.Decline:
                    {
                        break;
                    }

            }
        }

        static void WriteGroupMembersGuidMask(IEnumerable<Character> pMembers, ref BitPack BitPack, ref PacketWriter writer, Character pChar)
        {
            bool hit = false;
            foreach (Character c in pMembers)
            {
                BitPack.Write((byte)c.Name.Length, 6);
                BitPack.WriteGuidMask(c.Guid, 4, 3, 7, 0, 1, 2, 6, 5);

                // Player must always be 2nd -- wtf Blizzard. Tested on 3m group
                if (!hit)
                {
                    BitPack.Write((byte)pChar.Name.Length, 6);
                    BitPack.WriteGuidMask(4, 3, 7, 0, 1, 2, 6, 5);
                    hit = true;
                }
            }
        }

        static void WriteGroupMembersGuidBytes(IEnumerable<Character> pMembers, ref BitPack BitPack, ref PacketWriter writer, Character pChar)
        {
            bool hit = false;
            foreach (Character c in pMembers)
            {
                // unk
                writer.WriteUInt8(0x01);
                writer.WriteUInt8(0x00);

                BitPack.WriteGuidBytes(c.Guid, 2, 7, 4, 0);

                //unk
                writer.WriteUInt8(0x00);

                BitPack.WriteGuidBytes(c.Guid, 6, 1, 5, 3);

                //unk
                writer.WriteUInt8(0x00);

                writer.WriteString(c.Name);

                // Player must always be 2nd -- wtf Blizzard. Tested on 3m group
                if (!hit)
                {
                    // unk
                    writer.WriteUInt8(0x01);
                    writer.WriteUInt8(0x00);

                    BitPack.WriteGuidBytes(2, 7, 4, 0);

                    // unk
                    writer.WriteUInt8(0x00);

                    BitPack.WriteGuidBytes(6, 1, 5, 3);

                    // unk
                    writer.WriteUInt8(0x00);

                    writer.WriteString(pChar.Name);

                    hit = true;
                }
            }
        }

        static void GroupUpdate(Group group, ulong unkGUID = 0)
        {
            group.counter++;
            foreach (Character pMember in group.Members)
            {
                if (!(group.Members.Count > 1))
                {
                    var session = WorldMgr.GetSession(pMember.Guid);
                    GroupLeave(ref session);
                    return;
                }

                PacketWriter writer = new PacketWriter(ServerMessage.GroupUpdate);
                BitPack BitPack = new BitPack(writer, pMember.Guid);

                //unk
                writer.WriteUInt32(group.counter);
                writer.WriteUInt8(0x01);
                writer.WriteUInt8(0x00);
                writer.WriteUInt8((byte)group.Type);
                writer.WriteUInt32(0x01);

                // Leader guid mask
                BitPack.WriteGuidMask(group.Leader.Guid, 3, 6);

                // Group GUID mask
                BitPack.WriteGuidMask(group.Guid, 6, 7, 2, 5, 3);

                // Leader guid mask
                BitPack.WriteGuidMask(group.Leader.Guid, 0, 5);

                //unk Must be 1 if Group??
                BitPack.Write(1);

                // Group GUID mask
                BitPack.WriteGuidMask(group.Guid, 4);

                //unk
                BitPack.Write((byte)group.Members.Count, 21);

                // Looter GUID
                BitPack.WriteGuidMask(group.LooterGUID, 4, 6, 5, 7, 0, 1, 2, 3);

                WriteGroupMembersGuidMask(group.GetGroupMembers(pMember), ref BitPack, ref writer, pMember);

                // unk -- Party Raid?
                BitPack.Write(0);

                // Group GUID mask
                BitPack.WriteGuidMask(group.Guid, 1);

                //unk
                BitPack.Write(1); // 1

                // Leader guid mask
                BitPack.WriteGuidMask(group.Leader.Guid, 4);

                // Group GUID mask
                BitPack.WriteGuidMask(group.Guid, 0);

                // Leader guid mask
                BitPack.WriteGuidMask(group.Leader.Guid, 2, 7, 1);

                BitPack.Flush();

                WriteGroupMembersGuidBytes(group.GetGroupMembers(pMember), ref BitPack, ref writer, pMember);

                // Group Loot Threshold
                writer.WriteUInt8((byte)group.LootThreshold);

                // Looter GUID
                BitPack.WriteGuidBytes(group.LooterGUID, 5, 4);

                // Group Loot Type
                writer.WriteUInt8((byte)group.LootMethod);

                // Looter GUID
                BitPack.WriteGuidBytes(group.LooterGUID, 3, 1, 0, 6, 2, 7);

                // Group GUID Data
                BitPack.WriteGuidBytes(group.Guid, 2);

                //unk 
                writer.WriteUInt32(0x03);

                // Dungeon Difficulty -- UInt32 wtf?
                writer.WriteUInt32((uint)group.DungeonDifficulty);

                // Group GUID Data
                BitPack.WriteGuidBytes(group.Guid, 5, 3, 1, 0);

                //Leader GUID
                BitPack.WriteGuidBytes(group.Leader.Guid, 7, 2, 0, 1);


                // Group GUID Data
                BitPack.WriteGuidBytes(group.Guid, 7);

                //Leader GUID
                BitPack.WriteGuidBytes(group.Leader.Guid, 6, 4, 5);

                // Group GUID Data
                BitPack.WriteGuidBytes(group.Guid, 6, 4);

                //Leader GUID
                BitPack.WriteGuidBytes(group.Leader.Guid, 3);

                WorldMgr.GetSession(pMember.Guid).Send(ref writer);
            }
        }

        public static void GroupLeave(ref WorldClass session)
        {
            Group group = session.Character.Group;
            group.Remove(session.Character);
            //ulong unkGUID = 0;
            PacketWriter writer = new PacketWriter(ServerMessage.GroupUpdate);
            BitPack BitPack = new BitPack(writer);

            //unk
            writer.WriteUInt32(0x03);
            writer.WriteUInt8(0x00);
            writer.WriteUInt8(0x00);
            writer.WriteUInt8((byte)GroupType.Leave);
            writer.WriteUInt32(0xFFFFFFFF);

            BitPack.WriteGuidMask(0);
            BitPack.WriteGuidMask(0);

            // Group GUID mask
            BitPack.WriteGuidMask(group.Guid, 6, 7, 2, 5, 3);

            // Leader guid mask
            BitPack.WriteGuidMask(0);
            BitPack.WriteGuidMask(0);

            //unk Must be 1 if Group??
            BitPack.Write(0);

            // Group GUID mask
            BitPack.WriteGuidMask(group.Guid, 4);

            BitPack.Write(0, 8);
            BitPack.Write(0, 8);
            BitPack.Write(0);

            BitPack.Write(0);

            // Group GUID mask
            BitPack.WriteGuidMask(group.Guid, 1);

            //unk
            BitPack.Write(0); // 1

            // Leader guid mask
            BitPack.WriteGuidMask(0);

            // Group GUID mask
            BitPack.WriteGuidMask(group.Guid, 0);

            // Leader guid mask
            BitPack.WriteGuidMask(0);
            BitPack.WriteGuidMask(0);
            BitPack.WriteGuidMask(0);

            BitPack.Flush();

            BitPack.WriteGuidBytes(group.Guid, 2, 5, 3, 1, 0, 7, 6, 4);

            session.Send(ref writer);

            if (group.Members.Count > 0)
                GroupUpdate(group);
            else
                group.Disband();
        }
    }
}
