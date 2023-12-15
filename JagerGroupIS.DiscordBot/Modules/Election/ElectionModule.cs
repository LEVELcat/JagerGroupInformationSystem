using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JagerGroupIS.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JagerGroupIS.DiscordBot.Modules.Election
{
    [SlashCommandGroup("голосование", "меню управления голосованиями")]
    [SlashCommandPermissions(DSharpPlus.Permissions.Administrator)]
    public class ElectionModule : ApplicationCommandModule
    {
        readonly DiscordBotDbContext dbContext;
        
        public ElectionModule(DiscordBotDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [SlashCommand("создать", "откроет меню создания голосования")]
        public async Task CreateElectionAsync(InteractionContext context)
        {
            new ElectionFactory(dbContext).CreateElectionAsync(context);
        }

        //[SlashCommand("история", "получить историю по голосованию")]
        [ContextMenu(DSharpPlus.ApplicationCommandType.MessageContextMenu, "election history")]
        public async Task GetElectionHistory(ContextMenuContext context)
        {
            new ElectionHistory(dbContext).GetHistoryOfElection(context);
        }
    }

    public class ElectionHistory
    {
        readonly DiscordBotDbContext dbContext;

        public ElectionHistory(DiscordBotDbContext dbContext)
        {
            this.dbContext= dbContext;
        }

        public async Task GetHistoryOfElection(ContextMenuContext context)
        {
            var guildId = unchecked((long)context.Guild.Id);
            var messageId = unchecked((long)context.TargetMessage.Id);

            if (await dbContext.Elections.FirstOrDefaultAsync(x => x.GuildID == guildId && x.MessageID == messageId) is not Models.Database.Election election)
            {
                context.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent("Голосование не найдено").AsEphemeral());
                return;
            }

            var values = election.Votes.Select(x => $"{x.User.DiscordUserID}\t{x.VoteTypeString}\t{x.VoteTimeUTC}");

            var result = string.Join("\n", values);

            context.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent(result).AsEphemeral());
        }
    }
}
