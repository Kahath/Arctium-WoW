using Framework.Constants;
using Framework.Database;
using Framework.Logging;
using Framework.Singleton;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorldServer.Game.WorldEntities;

namespace WorldServer.Game.Managers
{
    public class GroupManager : SingletonBase<GroupManager>
    {

        Timer update;
        private ulong GUID = 0;
        private ulong newGUID = 0;
        private uint interval = 5 * 60 * 1000; // 5min
        private bool dbSave = false;
        private bool immediateUpdate = true;
        public Group.Member Member;
        ConcurrentDictionary<ulong, Group> Groups; 

        GroupManager()
        {
            Groups = new ConcurrentDictionary<ulong, Group>();

            if (dbSave)
                update = new Timer(TimerCallback, null, interval, interval);
            
            Initialize();
        }

        public void Initialize()
        {
            LoadGroups();
        }

        public Group GetGroup(ulong GUID)
        {
            Group group;
            if(!Groups.TryGetValue(GUID, out group))
                return null;
            return group;
        }

        public ulong GetGUID()
        {
            return GUID++;
        }

        public void SaveGroup(Group g, bool force = false)
        {
            Groups.TryAdd(g.Guid, g);

            if (dbSave && (immediateUpdate || force))
            {
                DB.Characters.Execute("INSERT INTO `groups` VALUES (?, ?, ?, ?, ?, ?, ?, ?)",
                    g.Guid, g.LeaderGUID, g.LootMethod, g.LooterGUID,
                    g.LootThreshold, g.Type, g.DungeonDifficulty, g.RaidDifficulty);
                g.Saved = true;
            }
        }

        public void RemoveGroupFromDB(Group g)
        {
            Group group;
            Groups.TryRemove(g.Guid, out group);

            if (dbSave)
                DB.Characters.Execute("DELETE FROM `groups` WHERE `guid` = ?",
                    GUID);
        }

        public void AddMemberToDB(Group.Member m, ulong GUID)
        {
            if (dbSave)
                DB.Characters.Execute("INSERT INTO `group_member` VALUES(?, ?, ?, ?, ?, ?, ?)",
                    GUID, m.GUID, m.MemberRole, m.RaidRole, m.RaidGroup, m.Name, m.Flags);
        }

        public void RemoveMemberFromDB(ulong GUID)
        {
            if (dbSave)
                DB.Characters.Execute("DELETE FROM `group_member` WHERE `memberGuid` = ?",
                    GUID);
        }

        public void UpdateGroupMemberInfo<T>(ulong GUID, string columnName, T result, bool force = false)
        {
            result = (T)Convert.ChangeType(result, typeof(T));

            if (dbSave && (immediateUpdate || force))
            {
                DB.Characters.Execute("UPDATE `group_member` SET `" + columnName + "` = ? WHERE `memberGuid` = ?",
                    columnName, result, GUID);
                Log.Message(LogType.DB, "Immediate update of {0}", result.GetType());
            }
        }

        public void UpdateGroupInfo<T>(ulong GUID, string columnName, T result, bool force = false)
        {
            result = (T)Convert.ChangeType(result, typeof(T));

            if (dbSave && (immediateUpdate || force))
            {
                DB.Characters.Execute("UPDATE `groups` SET `" + columnName + "` = ? WHERE `guid` = ?",
                    columnName, result, GUID);
                Log.Message(LogType.DB, "Immediate update of {0}", result.GetType());
            }
        }

        public void LoadGroups()
        {
            Log.Message(LogType.DB, "Loading groups...");

            if (!dbSave)
            {
                Log.Message(LogType.DB, "Groups saving turned off. Deleting all groups and members...");
                DB.Characters.Execute("DELETE FROM `groups`");
                DB.Characters.Execute("DELETE FROM `group_member`");
                return;
            }

            SQLResult result = DB.Characters.Select("SELECT * FROM `groups`");
            Parallel.For(0, result.Count, i =>
            {
                Group Group = new Group();               
                ulong tempGUID = result.Read<ulong>(i, "guid");
                Group.Saved = true;
                Group.LeaderGUID = result.Read<ulong>(i, "leaderGuid");
                Group.Guid = GUID++;
                Group.LootMethod = (GroupLootMethod)result.Read<byte>(i, "lootMethod");
                Group.LootThreshold = (GroupLootThreshold)result.Read<byte>(i, "lootThreshold");
                Group.LooterGUID = result.Read<ulong>(i, "looterGuid");
                Group.Type = (GroupType)result.Read<byte>(i, "groupType");
                Group.DungeonDifficulty = (GroupDungeonDifficulty)result.Read<byte>(i, "dungeonDifficulty");
                Group.RaidDifficulty = (GroupDungeonDifficulty)result.Read<byte>(i, "raidDifficulty");

                DB.Characters.Execute("UPDATE `groups` SET `guid` = ? WHERE `guid` = ?",
                    Group.Guid, tempGUID);

                SQLResult res = DB.Characters.Select("SELECT * FROM `group_member` WHERE `guid` = ?", tempGUID);

                for (int j = 0; j < res.Count; j++)
                {
                    Member = new Group.Member();
                    Member.GUID = res.Read<ulong>(j, "memberGuid");
                    Member.MemberRole = (GroupMemberRole)res.Read<byte>(j, "memberRole");
                    Member.RaidRole = res.Read<GroupRaidRole>(j, "raidRole");
                    Member.RaidGroup = (GroupRaidGroups)res.Read<byte>(j, "raidGroup");
                    Member.Name = res.Read<string>(j, "name");
                    Member.Flags = res.Read<byte>(j, "flags");
                    Group.AddLoad(Member);
                }

                Groups.TryAdd(Group.Guid, Group);
            });
            Log.Message(LogType.DB, "Loaded {0} groups from database", result.Count);
            newGUID = GUID;
            
        }

        public Group CharacterGroup(Character pChar)
        {
            foreach (var key in Groups)
                foreach (var pMember in key.Value.Members)
                    if (pMember.GUID == pChar.Guid)
                        return key.Value;
            return null;
        }

        private void TimerCallback(object o)
        {
            uint s = 0;
            Log.Message(LogType.DB, "Saving groups to database...");
            Parallel.ForEach(Groups, (KeyValuePair<ulong, Group> keyval) =>
                {
                    if (!keyval.Value.IsOnline())
                    {
                        ++s;
                        return;
                    }
                    if (keyval.Value.Saved)
                    {
                        DB.Characters.Execute("UPDATE `groups` SET " +
                            "`guid` = ?, " +
                            "`leaderGuid` = ?, " +
                            "`lootMethod` = ?, " +
                            "`looterGuid` = ?, " +
                            "`lootThreshold` = ?, " +
                            "`groupType` = ?, " +
                            "`dungeonDifficulty` = ?, " +
                            "`raidDifficulty` = ? " +
                            "WHERE `guid` = ?",
                            keyval.Value.Guid,
                            keyval.Value.LeaderGUID,
                            keyval.Value.LootMethod,
                            keyval.Value.LooterGUID,
                            keyval.Value.LootThreshold,
                            keyval.Value.Type,
                            keyval.Value.DungeonDifficulty,
                            keyval.Value.RaidDifficulty,
                            keyval.Value.Guid);

                        for (int i = 0; i < keyval.Value.Members.Count; i++)
                        {
                            DB.Characters.Execute("UPDATE `group_member` SET " +
                                "`guid` = ?, " +
                                "`memberGuid` = ?, " +
                                "`memberRole` = ?, " +
                                "`raidRole`= ?, " +
                                "`raidGroup`  = ? " +
                                "WHERE `memberGuid` = ?",
                                keyval.Value.Guid,
                                keyval.Value.Members[i].GUID,
                                keyval.Value.Members[i].MemberRole,
                                keyval.Value.Members[i].RaidRole,
                                keyval.Value.Members[i].RaidGroup,
                                keyval.Value.Members[i].GUID);
                        }
                    }
                    else
                    {
                        DB.Characters.Execute("INSERT INTO `groups` VALUES (?, ?, ?, ?, ?, ?, ?, ?)",
                            keyval.Value.Guid,
                            keyval.Value.LeaderGUID,
                            keyval.Value.LootMethod,
                            keyval.Value.LooterGUID,
                            keyval.Value.LootThreshold,
                            keyval.Value.Type,
                            keyval.Value.DungeonDifficulty,
                            keyval.Value.RaidDifficulty);

                        for(int i = 0; i < keyval.Value.Members.Count; i++)
                        {
                            DB.Characters.Execute("INSERT INTO `group_member` VALUES(?, ?, ?, ?, ?)",
                                keyval.Value.Guid,
                                keyval.Value.Members[i].GUID,
                                keyval.Value.Members[i].MemberRole,
                                keyval.Value.Members[i].RaidRole,
                                keyval.Value.Members[i].RaidGroup);
                        }

                        keyval.Value.Saved = true;
                    }
                });
            Log.Message(LogType.DB, "{0} groups saved", Groups.Count - s);
        }
    }
}
