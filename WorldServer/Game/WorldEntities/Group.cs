using Framework.Constants;
using System;
using System.Collections.Generic;

namespace WorldServer.Game.WorldEntities
{
    public class Group
    {
        public List<Character> Members = new List<Character>(5);
        public ulong Guid { get; private set; }
        public Character Leader { get; set; }
        public GroupLootType LootMethod { get; set; }
        public GroupLootThreshold LootThreshold { get; set; }
        public GroupDungeonDifficulty DungeonDifficulty { get; set; }
        public GroupType Type { get; set; }
        public uint counter = 0;

        public Group(ulong groupGuid, Character leader)
        {
            this.Guid = groupGuid;
            this.Leader = leader;
            Add(leader);
        }

        public void Add(Character pChar)
        {
            Members.Add(pChar);
            pChar.Group = this;
        }

        public bool Remove(Character pChar)
        {
            Members.Remove(pChar);
            pChar.Group = null;
            if (pChar == this.Leader)
            {
                if (this.Members.Count > 1)
                {
                    this.Leader = GetNewRandomLeader();
                    return true;
                }
            }

            return false;
        }

        public Character GetNewRandomLeader()
        {
            return Members[new Random().Next(Members.Count)];
        }

        public void Disband()
        {
            Members.Clear();
            this.Leader = null;
        }

        public IEnumerable<Character> GetGroupMembers()
        {
            foreach (Character c in Members)
                yield return c;
        }

        public IEnumerable<Character> GetGroupMembers(Character pChar)
        {
            foreach (Character c in Members)
            {
                if (c != pChar)
                    yield return c;
            }

        }

        public bool IsFull()
        {
            if (Members.Count == 5)
                return true;
            return false;
        }

        public Character GetMemberByGuid(ulong GUID)
        {
            foreach (Character c in Members)
                if (c.Guid == GUID)
                    return c;
            return null;
        }
    }
}
