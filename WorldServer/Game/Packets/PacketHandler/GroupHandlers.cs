﻿using Framework.Constants;
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

        [Opcode(ClientMessage.GroupInvite, "17538")]
        public static void HandleGroupRequest(ref PacketReader packet, WorldClass session)
        {
            var pChar = session.Character;
            BitUnpack BitUnpack = new BitUnpack(packet);
            packet.Skip(11);
            BitUnpack.GetBits<byte>(5);
            var length = BitUnpack.GetBits<byte>(6);
            var message = packet.ReadString(length);
            var pTarget = WorldMgr.GetSession(message);
            //ulong unkGUID = 0;
            string Realm = "Outland"; // hardcoded atm

            PacketWriter writer = new PacketWriter(ServerMessage.GroupInvite);
            BitPack BitPack = new BitPack(writer, pChar.Guid);


            BitPack.WriteGuidMask(3);

            // upk 
            BitPack.Write(1);

            BitPack.WriteGuidMask(5);
            BitPack.WriteGuidMask(7);
            BitPack.WriteGuidMask(2);
            BitPack.WriteGuidMask(0);
            //BitPack.Write(1);

            // unk
            BitPack.Write(0, 8);
            BitPack.Write(0, 8);
            BitPack.Write(0, 6);

            // Char name Length
            BitPack.Write((byte)pChar.Name.Length, 6);

            // Is char in group
            BitPack.Write(pTarget.Character.IsInGroup() ? 0 : 1);

            BitPack.WriteGuidMask(4);

            // unk
            BitPack.Write(0);
            BitPack.Write(0);

            //BitPack.Write(0, 4);
            BitPack.WriteRealmLength((byte)Realm.Length); // 9 bit
            //writer.WriteUInt8(0x0E);

            BitPack.WriteGuidMask(6);
            BitPack.WriteGuidMask(2);

            // unk
            BitPack.Write(0, 5);

            BitPack.Flush();

            //writer.WriteUInt8(0x80);

            // unk
            writer.WriteUInt32(0);

            BitPack.WriteGuidBytes(6);
            writer.WriteString(Realm);
            BitPack.WriteGuidBytes(1);
            BitPack.WriteGuidBytes(2);
            // writer.WriteUInt8(0xF2);
            // writer.WriteUInt8(0xBB);

            // unk
            writer.WriteUInt32(0);

            writer.WriteString(pChar.Name);
            BitPack.WriteGuidBytes(0);
            writer.WriteUInt32(0x00);
            BitPack.WriteGuidBytes(7);
            BitPack.WriteGuidBytes(3);
            BitPack.WriteGuidBytes(5);
            BitPack.WriteGuidBytes(4);

            //writer.WriteStringBytes("FC150500000405A205EC0700000000");
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
                        ulong unkGUID = 0;
                        var pChar = session.Character;
                        var pLeader = WorldMgr.GetSession(pChar.PendingInvite);
                        pChar.PendingInvite = null;

                        if (pLeader == null)
                            return;

                        if (!pLeader.Character.IsInGroup())
                        {
                            group = new Group(PartyGUID++, pLeader.Character, pLeader.Character.Name);
                            pLeader.Character.Group = group;
                            group.Add(pLeader.Character);
                        }
                        else
                            group = pLeader.Character.Group;

                        if (group.IsFull())
                            return;

                        group.Add(pChar);
                        pChar.Group = group;

                        foreach (Character pMember in group.MembersList)
                        {
                            PacketWriter writer = new PacketWriter(ServerMessage.GroupUpdate);
                            BitPack BitPack = new BitPack(writer, pMember.Guid);

                            //unk
                            writer.WriteUInt32((uint)group.MembersList.Count);
                            writer.WriteUInt8(0x01);
                            writer.WriteUInt8(0x00);
                            writer.WriteUInt8(0x00);
                            writer.WriteUInt32(0x01); // 1

                            // Leader guid mask
                            BitPack.WriteGuidMask(group.Leader.Guid, 3);
                            BitPack.WriteGuidMask(group.Leader.Guid, 6);

                            // Group GUID mask
                            BitPack.WriteGuidMask(group.Guid, 6);
                            BitPack.WriteGuidMask(group.Guid, 7);
                            BitPack.WriteGuidMask(group.Guid, 2);
                            BitPack.WriteGuidMask(group.Guid, 5);
                            BitPack.WriteGuidMask(group.Guid, 3);

                            // Leader guid mask
                            BitPack.WriteGuidMask(group.Leader.Guid, 0);
                            BitPack.WriteGuidMask(group.Leader.Guid, 5);

                            //unk Must be 1 if Group??
                            BitPack.Write(1);

                            // Group GUID mask
                            BitPack.WriteGuidMask(group.Guid, 4);

                            //unk
                            BitPack.Write(0, 8);
                            BitPack.Write(0, 8);
                            BitPack.Write((byte)group.MembersList.Count, 5);


                            //unk group flags prolly triggered by unk Must be 1
                            BitPack.WriteGuidMask(unkGUID, 4);
                            BitPack.WriteGuidMask(unkGUID, 6);
                            BitPack.WriteGuidMask(unkGUID, 5);
                            BitPack.WriteGuidMask(unkGUID, 7);
                            BitPack.WriteGuidMask(unkGUID, 0);
                            BitPack.WriteGuidMask(unkGUID, 1);
                            BitPack.WriteGuidMask(unkGUID, 2);
                            BitPack.WriteGuidMask(unkGUID, 3);

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
                            BitPack.WriteGuidMask(group.Leader.Guid, 2);
                            BitPack.WriteGuidMask(group.Leader.Guid, 7);
                            BitPack.WriteGuidMask(group.Leader.Guid, 1);

                            BitPack.Flush();

                            WriteGroupMembersGuidBytes(group.GetGroupMembers(pMember), ref BitPack, ref writer, pMember);

                            // Group Loot Threshold
                            writer.WriteUInt8((byte)GroupLootThreshold.Uncommon);

                            // unk flags triggered by unk Must be 1
                            BitPack.WriteGuidBytes(unkGUID, 5);
                            BitPack.WriteGuidBytes(unkGUID, 4);

                            // Group Loot Type
                            writer.WriteUInt8((byte)GroupLootType.GroupLoot);

                            // unk flags triggered by unk Must be 1
                            BitPack.WriteGuidBytes(unkGUID, 3);
                            BitPack.WriteGuidBytes(unkGUID, 1);
                            BitPack.WriteGuidBytes(unkGUID, 0);
                            BitPack.WriteGuidBytes(unkGUID, 6);
                            BitPack.WriteGuidBytes(unkGUID, 2);
                            BitPack.WriteGuidBytes(unkGUID, 7);

                            // Group GUID Data
                            BitPack.WriteGuidBytes(group.Guid, 2);

                            //unk
                            writer.WriteUInt32(0x01); // 3

                            // Dungeon Difficulty -- UInt32 wtf?
                            writer.WriteUInt32((uint)GroupDungeonDifficulty.FivePlayer);

                            // Group GUID Data
                            BitPack.WriteGuidBytes(group.Guid, 5);
                            BitPack.WriteGuidBytes(group.Guid, 3);
                            BitPack.WriteGuidBytes(group.Guid, 1);
                            BitPack.WriteGuidBytes(group.Guid, 0);

                            //Leader GUID
                            BitPack.WriteGuidBytes(group.Leader.Guid, 7);
                            BitPack.WriteGuidBytes(group.Leader.Guid, 2);
                            BitPack.WriteGuidBytes(group.Leader.Guid, 0);
                            BitPack.WriteGuidBytes(group.Leader.Guid, 1);

                            // Group GUID Data
                            BitPack.WriteGuidBytes(group.Guid, 7);

                            //Leader GUID
                            BitPack.WriteGuidBytes(group.Leader.Guid, 6);
                            BitPack.WriteGuidBytes(group.Leader.Guid, 4);
                            BitPack.WriteGuidBytes(group.Leader.Guid, 5);

                            // Group GUID Data
                            BitPack.WriteGuidBytes(group.Guid, 6);
                            BitPack.WriteGuidBytes(group.Guid, 4);

                            //Leader GUID
                            BitPack.WriteGuidBytes(group.Leader.Guid, 3);

                            //session.Send(ref writer);
                            WorldMgr.GetSession(pMember.Guid).Send(ref writer);
                        }

                        break;
                    }
                case GroupInviteResponse.Decline:
                    {
                        break;
                    }

            }
        }

        public static void HandleGroupRequestDecline(ref PacketReader packet, WorldClass session)
        {

        }

        public static void HandleGroupLoot(ref PacketReader packet, WorldClass session)
        {

        }

        public static void HandleGroupChange(ref PacketReader packet, WorldClass session)
        {

        }

        static void WriteGroupMembersGuidMask(IEnumerable<Character> pMembers, ref BitPack BitPack, ref PacketWriter writer, Character pChar)
        {
            bool hit = false;
            foreach (Character c in pMembers)
            {
                BitPack.Write((byte)c.Name.Length, 6);
                BitPack.WriteGuidMask(c.Guid, 4, 3, 7, 0, 1, 2, 6, 5);
                //hit = true;

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
    }
}
