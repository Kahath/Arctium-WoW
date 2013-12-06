using Framework.Constants;
using System;
using System.Collections.Generic;

namespace WorldServer.Game.WorldEntities
{
    public class Group
    {
        public List<Character> MembersList = new List<Character>(5);
        public ulong Guid { get; private set; }
        public Character Leader { get; set; }
        //public string LeaderName { get; set; }
        public GroupLootType LootMethod { get; set; }
        public GroupLootThreshold LootThreshold { get; set; }
        public GroupDungeonDifficulty DungeonDifficulty { get; set; }

        public Group(ulong groupGuid, Character leader/*, string leaderName*/)
        {
            this.Guid = groupGuid;
            this.Leader = leader;
            //this.LeaderName = leaderName;
            //this.Leader.Group = this;
            //Add(leader);
        }

        public void Add(Character pChar)
        {
            MembersList.Add(pChar);
            //pChar.Group = this;
        }

        public void Remove(Character pChar)
        {
            MembersList.Remove(pChar);
        }

        public void Disband()
        {
            MembersList.Clear();
            this.Leader = null;
            //this.LeaderName = null;
        }

        public IEnumerable<Character> GetGroupMembers(Character pChar)
        {
            foreach (Character c in MembersList)
            {
                if (c != pChar)
                    yield return c;
                //System.Console.WriteLine(c.Name);
            }

        }

        public bool IsFull()
        {
            if (MembersList.Count == 5)
                return true;
            return false;
        }

        public Character GetMemberByGuid(ulong GUID)
        {
            foreach (Character c in MembersList)
                if (c.Guid == GUID)
                    return c;
            return null;
        }
    }
}
