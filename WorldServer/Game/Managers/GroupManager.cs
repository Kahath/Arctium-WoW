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
        private static ulong GUID = 0;
        public Group Group;
        public Group.Member Member;
        private readonly object sync = new object();

        ConcurrentDictionary<ulong, Group> Groups; 

        GroupManager()
        {
            Groups = new ConcurrentDictionary<ulong, Group>();
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

        public void LoadGroups()
        {
            Log.Message(LogType.DB, "Loading groups...");
            ParallelOptions parallelOptions = new ParallelOptions();

            SQLResult result = DB.Characters.Select("SELECT * FROM `groups`");
            Parallel.For(0, result.Count, i =>
            {
                ulong tempGUID = result.Read<ulong>(i, "guid");
                Group = new Group();
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
                    Member.RaidRole = res.Read<byte>(j, "raidRole");
                    Member.RaidGroup = (GroupRaidGroups)res.Read<byte>(j, "raidGroup");
                    Member.Name = res.Read<string>(j, "name");
                    Member.Flags = res.Read<byte>(j, "flags");
                    Group.AddLoad(Member);
                }

                Groups.TryAdd(Group.Guid, Group);
            });
            Log.Message(LogType.DB, "Loaded {0} groups from database", result.Count);
            
        }

        public Group CharacterGroup(Character pChar)
        {
            foreach (var key in Groups)
                foreach (var pMember in key.Value.Members)
                    if (pMember.GUID == pChar.Guid)
                        return key.Value;
            return null;
        }
    }
}
