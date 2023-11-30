using DSharpPlus.Entities;
using JagerGroupIS.DatabaseContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JagerGroupIS.DiscordBot.Services
{
    //public class AccessChecker
    //{
    //    DiscordBotDbContext dbContext;

    //    public AccessChecker(DiscordBotDbContext dbContext)
    //    {
    //        this.dbContext = dbContext;
    //    }

    //    public bool CheckAccess(DiscordMember discordMember)
    //    {
    //        if (discordMember.IsOwner)
    //            return true;

    //        long guildID = unchecked((long)discordMember.Guild.Id);

    //        var rolesIDs = discordMember.Roles.Select(x => x.Id);

    //        var access = dbContext.AccessTables.Where(x => x.GuildID == guildID)
    //                                           .Select(x => x.RoleID)
    //                                           .Select(x => unchecked((ulong)x));

    //        if (rolesIDs.Any(x => rolesIDs.Contains(x)))
    //            return true;

    //        return false;
    //    }
    //}
}
