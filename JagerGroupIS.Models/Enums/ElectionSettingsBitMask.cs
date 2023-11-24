namespace JagerGroupIS.Models.Enums
{
    [Flags]
    public enum ElectionSettingsBitMask : ulong
    {
        None = 0x0, //0
        AgreeList = 0x1, // 1,
        RejectList = 0x2, // 2,
        NotVotedList = 0x4, // 4,
        NotificationForAgree = 0x8, // 8,
        NotificationFroNotVoted = 0x10, // 16,


        NotificationBefore_1Mounth = 0x20000000000000, // 9007199254740992,
        NotificationBefore_2Week = 0x40000000000000, // 18014398509481984,
        NotificationBefore_1Week = 0x80000000000000, // 36028797018963968,
        NotificationBefore_48Hour = 0x100000000000000, // 72057594037927936,
        NotificationBefore_24Hour = 0x200000000000000, // 144115188075855872,
        NotificationBefore_12Hour = 0x400000000000000, // 288230376151711744,
        NotificationBefore_6Hour = 0x800000000000000, // 576460752303423488,
        NotificationBefore_2Hour = 0x1000000000000000, // 1152921504606846976,
        NotificationBefore_1Hour = 0x2000000000000000, // 2305843009213693952,
        NotificationBefore_15Minutes = 0x4000000000000000, // 4611686018427387904,
    }
}
