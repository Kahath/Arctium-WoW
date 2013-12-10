﻿using Framework.Constants;
using Framework.Constants.NetMessage;
using Framework.Database;
using Framework.Network.Packets;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using WorldServer.Network;

namespace WorldServer.Game.WorldEntities
{
    public class Group
    {
        public class Member
        {
            public ulong GUID { get; set; }
            public string Name { get; set; }
            public byte RaidRole { get; set; }
            public byte Flags { get; set; }
            public GroupMemberMark MemberMark { get; set; }
            public GroupMemberRole MemberRole { get; set; }
            public GroupRaidGroups RaidGroup { get; set; }
        };

        private readonly object sync = new object();
        private bool disband = false;
        public List<Member> Members;
        public ulong Guid { get; set; }
        public ulong LeaderGUID { get; set; }
        public ulong LooterGUID { get; set; }
        public uint counter = 0;
        public GroupLootMethod LootMethod { get; set; }
        public GroupLootThreshold LootThreshold { get; set; }
        public GroupDungeonDifficulty DungeonDifficulty { get; set; }
        public GroupDungeonDifficulty RaidDifficulty { get; set; }
        public GroupType Type { get; set; }

        public Group(ulong groupGuid, Character leader, GroupType type = GroupType.Normal)
        {
            this.Guid = groupGuid;
            this.LeaderGUID = leader.Guid;
            this.Type = type;
            LooterGUID = 0;
            LootMethod = GroupLootMethod.GroupLoot;
            LootThreshold = GroupLootThreshold.Uncommon;
            DungeonDifficulty = GroupDungeonDifficulty.FivePlayer;
            RaidDifficulty = GroupDungeonDifficulty.TenPlayer;
            if (this.Type != GroupType.Normal)
                Members = new List<Member>(40);
            else
                Members = new List<Member>(5);
            this.Add(leader);

            DB.Characters.Execute("INSERT INTO `groups` VALUES (?, ?, ?, ?, ?, ?, ?, ?)",
                this.Guid, this.LeaderGUID, this.LootMethod, this.LooterGUID,
                this.LootThreshold, this.Type, this.DungeonDifficulty, this.RaidDifficulty);
        }

        public Group()
        {
            Members = new List<Member>();
        }

        public void AddLoad(Member m)
        {
            //lock (sync)
                Members.Add(m);
        }

        public void Add(Character pChar)
        {
            Member m = new Member();
            m.Name = pChar.Name;
            m.GUID = pChar.Guid;
            m.MemberRole = GroupMemberRole.None;
            m.RaidGroup = GroupRaidGroups.GroupOne;
            m.RaidRole = 0x00;
            m.Flags = 0x01;
            Members.Add(m);
            pChar.Group = this;

            DB.Characters.Execute("INSERT INTO `group_member` VALUES(?, ?, ?, ?, ?, ?, ?)",
                Guid, m.GUID, m.MemberRole, m.RaidRole, m.RaidGroup, m.Name, m.Flags);
        }

        public bool Remove(ulong GUID)
        {
            Member m = GetMemberFromGuid(GUID);   

            if (m == null)
                return false;

            Character c = GetCharFromMemberGUID(m.GUID);

            c.Group = null;
            Members.Remove(m);

            DB.Characters.Execute("DELETE FROM `group_member` WHERE `memberGuid` = ?",
                m.GUID);

            if (m.GUID == this.LeaderGUID)
            {
                if (this.Members.Count > 1)
                {
                    this.LeaderGUID = GetNewRandomLeader();
                    DB.Characters.Execute("UPDATE `groups` SET `leaderGuid` = ? WHERE `guid` = ?",
                        this.LeaderGUID, this.Guid);
                    return true;
                }
            }
            return false;
        }

        public Character GetCharFromMemberGUID(ulong GUID)
        {
            var session = Globals.WorldMgr.GetSession(GUID);
            return session != null ? session.Character : null;
        }

        public ulong GetNewRandomLeader()
        {
            return Members[new Random().Next(Members.Count)].GUID;
        }

        public void Disband()
        {
            this.disband = true;
            for(int i = 0; i < Members.Count; i++)
                Uninvite(Members[i].GUID);

            DB.Characters.Execute("DELETE FROM `groups` WHERE `guid` = ?",
                this.Guid);
        }

        public IEnumerable<Member> GetGroupMembers(Member pChar = null)
        {
            foreach (Member m in Members)
                if (m != pChar)
                    yield return m;
        }

        public bool IsFull()
        {
            return Members.Count == 5 ? true : false;
        }

        public bool IsLeader(ulong GUID)
        {
            return GUID == LeaderGUID ? true : false;
        }

        public Member GetMemberFromGuid(ulong GUID)
        {
            foreach (Member m in Members)
                if (m.GUID == GUID)
                    return m;
            return null;
        }

        public void ChangeGroupMemberRole(ulong GUID, GroupMemberRole role)
        {    
            GetMemberFromGuid(GUID).MemberRole = role;
            DB.Characters.Execute("UPDATE `group_member` SET `memberRole` = ? WHERE `memberGuid` = ?",
                role, GUID);
        }

        public void ChangeRaidGroup(ulong GUID, GroupRaidGroups group)
        {
            GetMemberFromGuid(GUID).RaidGroup = group;
            DB.Characters.Execute("UPDATE `group_member` SET `raidGroup` = ? WHERE `memberGuid` = ?",
                group, GUID);
        }

        public void ChangeRaidRole(ulong GUID, byte role)
        {
            GetMemberFromGuid(GUID).RaidRole = role;
            DB.Characters.Execute("UPDATE `group_member` SET `raidRole` = ? WHERE `memberGuid` = ?",
                role, GUID);
        }

        public void ChangeGroupLeader(ulong GUID)
        {
            this.LeaderGUID = GUID;
            DB.Characters.Execute("UPDATE `groups` SET `leaderGuid` = ? WHERE `guid` = ?",
                this.LeaderGUID, this.Guid);
        }

        public void ChangeGroupLootMethod(GroupLootMethod method)
        {
            this.LootMethod = method;

            DB.Characters.Execute("UPDATE `groups` SET `lootMethod` = ? WHERE `guid` = ?",
                this.LootMethod, this.Guid);
        }

        public void ChangeGroupLootThreshold(GroupLootThreshold threshold)
        {
            this.LootThreshold = threshold;

            DB.Characters.Execute("UPDATE `groups` SET `lootThreshold` = ? WHERE `guid` = ?",
                this.LootThreshold, this.Guid);
        }

        public void ChangeGroupDungeonDifficulty(GroupDungeonDifficulty difficulty)
        {
            this.DungeonDifficulty = difficulty;

            DB.Characters.Execute("UPDATE `groups` SET `dungeonDifficulty` = ? WHERE `guid` = ?",
                this.DungeonDifficulty, this.Guid);
        }

        public void ChangeGroupRaidDifficulty(GroupDungeonDifficulty difficulty)
        {
            this.RaidDifficulty = difficulty;

            DB.Characters.Execute("UPDATE `groups` SET `raidDifficulty` = ? WHERE `guid` = ?",
                this.RaidDifficulty, this.Guid);
        }

        public void ChangeGroupType(GroupType type)
        {
            this.Type = type;
            DB.Characters.Execute("UPDATE `groups` SET `groupType` = ? WHERE `guid` = ?",
                this.Type, this.Guid);
        }

        public void ChangeGroupLooterGuid(ulong guid)
        {
            this.LooterGUID = guid;

            DB.Characters.Execute("UPDATE `groups` SET `looterGuid` = ? WHERE `guid` = ?",
                this.LooterGUID, this.Guid);
        }

        public void Update()
        {
            counter++;
            foreach (Member pMember in Members)
            {
                PacketWriter writer = new PacketWriter(ServerMessage.GroupUpdate);
                BitPack BitPack = new BitPack(writer, pMember.GUID);

                writer.WriteUInt32(counter);

                // unk
                writer.WriteUInt8(0x01);
                writer.WriteUInt8(0x00);

                writer.WriteUInt8((byte)Type);

                // unk
                writer.WriteUInt32(0x01);

                // Leader guid mask
                BitPack.WriteGuidMask(LeaderGUID, 3, 6);

                // Group GUID mask
                BitPack.WriteGuidMask(Guid, 6, 7, 2, 5, 3);

                // Leader guid mask
                BitPack.WriteGuidMask(LeaderGUID, 0, 5);

                //unk Must be 1 if Group??
                BitPack.Write(1);

                // Group GUID mask
                BitPack.WriteGuidMask(Guid, 4);

                //unk
                BitPack.Write((byte)Members.Count, 21);

                //unk
                BitPack.WriteGuidMask(LooterGUID, 4, 6, 5, 7, 0, 1, 2, 3);

                WriteGroupMembersGuidMask(GetGroupMembers(pMember), ref BitPack, ref writer, pMember);

                // unk -- Party? Raid? Loot?
                //byte test = (byte)(group.Type == GroupType.Normal ? 0 : 1);
                byte test = 0;
                BitPack.Write(0);
                //BitPack.Write(test);

                // Group GUID mask
                BitPack.WriteGuidMask(Guid, 1);

                // Group Dungeon Difficulty
                BitPack.Write(1); // 1

                // Leader guid mask
                BitPack.WriteGuidMask(LeaderGUID, 4);

                // Group GUID mask
                BitPack.WriteGuidMask(Guid, 0);

                // Leader guid mask
                BitPack.WriteGuidMask(LeaderGUID, 2);

                if (test == 1)
                {
                    BitPack.Write(0);
                    BitPack.Write(0);
                }

                BitPack.WriteGuidMask(LeaderGUID, 7, 1);

                BitPack.Flush();

                if (test == 1)
                {
                    writer.WriteUInt32(0x00);
                    writer.WriteUInt8(0x00);
                    writer.WriteUInt8(0x00);
                    writer.WriteUInt8(0x00);
                    writer.WriteUInt8(0x00);
                    writer.WriteUInt8(0x00);
                    writer.WriteFloat(0);
                    writer.WriteUInt32(0x00);
                }

                WriteGroupMembersGuidBytes(GetGroupMembers(pMember), ref BitPack, ref writer, pMember);

                // Group Loot Threshold
                writer.WriteUInt8((byte)LootThreshold);

                // unk
                BitPack.WriteGuidBytes(LooterGUID, 5, 4);

                // Group Loot Type
                writer.WriteUInt8((byte)LootMethod);

                // unk
                BitPack.WriteGuidBytes(LooterGUID, 3, 1, 0, 6, 2, 7);

                // Group GUID Data
                BitPack.WriteGuidBytes(Guid, 2);

                // Raid Difficulty 
                writer.WriteUInt32((uint)RaidDifficulty);

                // Dungeon Difficulty -- UInt32 wtf?
                writer.WriteUInt32((uint)DungeonDifficulty);

                // Group GUID Data
                BitPack.WriteGuidBytes(Guid, 5, 3, 1, 0);

                //Leader GUID
                BitPack.WriteGuidBytes(LeaderGUID, 7, 2, 0, 1);


                // Group GUID Data
                BitPack.WriteGuidBytes(Guid, 7);

                //Leader GUID
                BitPack.WriteGuidBytes(LeaderGUID, 6, 4, 5);

                // Group GUID Data
                BitPack.WriteGuidBytes(Guid, 6, 4);

                //Leader GUID
                BitPack.WriteGuidBytes(LeaderGUID, 3);

                var session = Globals.WorldMgr.GetSession(pMember.GUID);
                if(session != null)
                    session.Send(ref writer);
            }
        }

        void WriteGroupMembersGuidMask(IEnumerable<Member> pMembers, ref BitPack BitPack, ref PacketWriter writer, Member pChar)
        {
            bool hit = false;
            foreach (Member m in pMembers)
            {
                BitPack.Write((byte)m.Name.Length, 6);
                BitPack.WriteGuidMask(m.GUID, 4, 3, 7, 0, 1, 2, 6, 5);

                // Player must always be 2nd -- wtf Blizzard. Tested on 3m group
                if (!hit)
                {
                    BitPack.Write((byte)pChar.Name.Length, 6);
                    BitPack.WriteGuidMask(4, 3, 7, 0, 1, 2, 6, 5);
                    hit = true;
                }
            }
        }

        void WriteGroupMembersGuidBytes(IEnumerable<Member> pMembers, ref BitPack BitPack, ref PacketWriter writer, Member pChar)
        {
            bool hit = false;
            foreach (Member m in pMembers)
            {
                // unk prolly member flags
                writer.WriteUInt8(m.Flags);

                // Raid assist
                writer.WriteUInt8(m.RaidRole);
                BitPack.WriteGuidBytes(m.GUID, 2, 7, 4, 0);

                //Raid Group number
                writer.WriteUInt8((byte)m.RaidGroup);

                BitPack.WriteGuidBytes(m.GUID, 6, 1, 5, 3);

                // Party Role
                writer.WriteUInt8((byte)m.MemberRole);

                writer.WriteString(m.Name);

                // Player must always be 2nd -- wtf Blizzard. Tested on 3m group
                if (!hit)
                {
                    // Player flags prolly
                    writer.WriteUInt8(pChar.Flags);

                    // Raid assist
                    writer.WriteUInt8((byte)pChar.RaidRole);

                    BitPack.WriteGuidBytes(2, 7, 4, 0);

                    //Raid Group number
                    writer.WriteUInt8((byte)pChar.RaidGroup);

                    BitPack.WriteGuidBytes(6, 1, 5, 3);

                    // Group Role
                    writer.WriteUInt8((byte)pChar.MemberRole);

                    writer.WriteString(pChar.Name);

                    hit = true;
                }
            }
        }

        public void Uninvite(ulong GUID)
        {
            Remove(GUID);
            
            var session = Globals.WorldMgr.GetSession(GUID);
            
            if (session == null)
            {
                UpdateIf(2);
                return;
            }

            PacketWriter writer = new PacketWriter(ServerMessage.GroupUpdate);
            BitPack BitPack = new BitPack(writer);


            writer.WriteUInt32(counter);
            writer.WriteUInt8(0x00);
            writer.WriteUInt8(0x00);
            writer.WriteUInt8((byte)GroupType.Leave);
            writer.WriteUInt32(0xFFFFFFFF);

            BitPack.WriteGuidMask(0);
            BitPack.WriteGuidMask(0);

            // Group GUID mask
            BitPack.WriteGuidMask(Guid, 6, 7, 2, 5, 3);

            // Leader guid mask
            BitPack.WriteGuidMask(0);
            BitPack.WriteGuidMask(0);

            //unk Must be 1 if Group??
            BitPack.Write(0);

            // Group GUID mask
            BitPack.WriteGuidMask(Guid, 4);

            BitPack.Write(0, 21);

            BitPack.Write(0);

            // Group GUID mask
            BitPack.WriteGuidMask(Guid, 1);

            //unk 1 if party?
            BitPack.Write(0); // 1

            // Leader guid mask
            BitPack.WriteGuidMask(0);

            // Group GUID mask
            BitPack.WriteGuidMask(Guid, 0);

            // Leader guid mask
            BitPack.WriteGuidMask(0);
            BitPack.WriteGuidMask(0);
            BitPack.WriteGuidMask(0);

            BitPack.Flush();

            BitPack.WriteGuidBytes(Guid, 2, 5, 3, 1, 0, 7, 6, 4);

            session.Send(ref writer);

            UpdateIf(2);
        }

        public void UpdateIf(byte size)
        {
            if (Members.Count >= size)
                Update();
            else if (Members.Count < size && !disband)
                Disband();
        }
    }
}
