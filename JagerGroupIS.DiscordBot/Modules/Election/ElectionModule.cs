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
        [ContextMenu(DSharpPlus.ApplicationCommandType.MessageContextMenu, "история голосования")]
        public async Task GetElectionHistory(ContextMenuContext context)
        {
            context.DeferAsync(true);
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
            try
            {
                var guildId = unchecked((long)context.Guild.Id);
                var messageId = unchecked((long)context.TargetMessage.Id);

                if (await dbContext.Elections.FirstOrDefaultAsync(x => x.GuildID == guildId && x.MessageID == messageId) is not Models.Database.Election election)
                {
                    context.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent("Голосование не найдено").AsEphemeral());
                    return;
                }

                var values = election.Votes.OrderBy(x => x.VoteTimeUTC).Select(x => $"<@{(unchecked((ulong)x.User.DiscordUserID))}>\t{x.VoteTypeString}\t{x.VoteTime}");

                var result = string.Join("\n", values.TakeLast(20));

                context.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent(result).AsEphemeral());
            }
            catch (Exception ex)
            {
                context.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent(ex.Message + "\n" + ex.ToString()).AsEphemeral());
            }

        }
    }
}