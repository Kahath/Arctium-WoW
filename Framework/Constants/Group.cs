using System;

namespace Framework.Constants
{

    public enum GroupType : byte
    {
        Normal = 0x00,
        BG = 0x01,
        Raid = 0x02,
        BGRaid = BG | Raid,       // mask
        Unk1 = 0x04,
        LFG = 0x08,
        Leave = 0x10, //leave/change group?, I saw this flag when leaving group and after leaving BG while in group
    };
    
    public enum GroupLootMethod : byte
    {
        FreeForAll              = 0x00,
        RoundRobin              = 0x01,
        MasterLoot              = 0x02,
        GroupLoot               = 0x03,
        NeedBeforeGreed         = 0x04,
    };

    public enum GroupLootThreshold : byte
    {
        Poor                    = 0x00,
        Common                  = 0x01,
        Uncommon                = 0x02,
        Rare                    = 0x03,
        Epic                    = 0x04
    };

    public enum GroupDungeonDifficulty : byte
    {
        None                    = 0x00,
        FivePlayer              = 0x01,
        FivePlayerHeroic        = 0x02,
        TenPlayer               = 0x03,
        TwentyFivePlayer        = 0x04,
        TenPlayerHeroic         = 0x05,
        TwentyFivePlayerHeroic  = 0x06,
        LookingForRaid          = 0x07,
        ChallengeMode           = 0x08,
        FourtyPlayer            = 0x09,
        ScenarioHeroic          = 0x0B,
        Scenario                = 0x0C,
        Flexible                = 0x0E,
    };

    public enum GroupInviteResponse : byte
    {
        Accept                  = 0x40,
        Decline                 = 0x00,
    };

    // test
    public enum GroupMemberMark : byte
    {
        None = 0x00,
        Unk1 = 0x01,
        Unk2 = 0x02,
        Unk3 = 0x03,
        Unk4 = 0x04,
        Unk5 = 0x05,
        Unk6 = 0x06,
        Unk7 = 0x07,
        Skull = 0x08,
    };

    public enum GroupMemberRole : byte
    {
        None                    = 0x00,
        Tank                    = 0x02,
        Healer                  = 0x04,
        Damage                  = 0x08
    };

    public enum GroupRaidGroups
    {
        GroupOne = 0x00,
        GroupTwo = 0x01,
        GroupThree = 0x02,
        GroupFour = 0x03,
        GroupFive = 0x04,
        GroupSix = 0x05,
        GroupSeven = 0x06,
        GroupEight = 0x04,
    };

}
