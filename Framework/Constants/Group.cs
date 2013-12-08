using System;

namespace Framework.Constants
{
    public enum GroupType
    {
        Normal = 0x00,
        BG = 0x01,
        Raid = 0x02,
        BGRaid = BG | Raid,       // mask
        Unk1 = 0x04,
        LFG = 0x08,
        Leave = 0x10, //leave/change group?, I saw this flag when leaving group and after leaving BG while in group
    };
    
    public enum GroupLootType : byte
    {
        FreeForAll = 0x00,
        RoundRobin = 0x01,
        MasterLoot = 0x02,
        GroupLoot = 0x03,
        NeedBeforeGreed = 0x04,
    };

    public enum GroupLootThreshold : byte
    {
        Poor = 0x00,
        Common = 0x01,
        Uncommon = 0x02,
        Rare = 0x03,
        Epic = 0x04
    };

    public enum GroupLootMethod : byte
    {
        FreeForAll = 0x00,
        RoundRobin = 0x01,
        MasterLoot = 0x02,
        GroupLoot = 0x03,
        NeedBeforeGreed = 0x04,
    };

    public enum GroupDungeonDifficulty : uint
    {
        None = 0x00,
        FivePlayer = 0x01,
        FivePlayerHeroic = 0x02,
        TenPlayer = 0x03,
        TwentyFivePlayer = 0x04,
        TenPlayerHeroic = 0x05,
        TwentyFivePlayerHeroic = 0x06,
        LookingForRaid = 0x07,
        ChallengeMode = 0x08,
        FourtyPlayer = 0x09,
        ScenarioHeroic = 0xB,
        Scenario = 0xC,
        Flexible = 0x0E,
    };

    public enum GroupInviteResponse : byte
    {
        Accept = 0x40,
        Decline = 0x00,
    };

    public enum GroupMemberRole : byte
    {
        None = 0x00,
        Tank = 0x02,
        Healer = 0x04,
        Damage = 0x08
    };

}
