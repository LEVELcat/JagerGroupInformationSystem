using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JagerGroupIS.DatabaseContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JagerGroupIS.DiscordBot.Modules.Election
{
    public class ElectionFactory
    {
        DiscordBotDbContext dbContext { get; }

        public ElectionFactory(DiscordBotDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task CreateElectionAsync(InteractionContext ctx)
        {
            var factoryResult = await new ElectionBuilder().BuildElectionAsync(ctx);

            if (factoryResult.Item1 == null || factoryResult.Item2 == null)
                return;

            var election = factoryResult.Item1;
            var messageBuilder = factoryResult.Item2;
            messageBuilder.AddComponents(ReturnButtonComponents());

            messageBuilder.Content = string.Empty;

            var Roles = ctx.Guild.Roles.Values.Where(x => election.RoleSetups.Where(r => r.IsTakingPart == true)
                                                                             .Select(r => unchecked((ulong)r.DisordRoleID))
                                                                             .Contains(x.Id))
                                              .ToArray();

            messageBuilder.Content = String.Join('\n', Roles.Select(r => r.Mention));
            messageBuilder.WithAllowedMentions(Roles.Select(x => new RoleMention(x) as IMention));

            //foreach (var roleSetup in election.RoleSetups)
            //    roleSetup.

            DiscordChannel channel = ctx.Guild.Channels[unchecked((ulong)election.ChanelID)];

            if (channel.IsCategory)
                channel = await ctx.Guild.CreateChannelAsync(messageBuilder.Embed.Title, ChannelType.Text, channel);

            var electionMessage = await channel.SendMessageAsync(messageBuilder);

            election.ChanelID = unchecked((long)channel.Id);
            election.MessageID = unchecked((long)electionMessage.Id);

            election.StartTime = DateTimeOffset.UtcNow;

            dbContext.Elections.Add(election);

            await dbContext.SaveChangesAsync();

            dbContext.DisposeAsync();
        }

        public static DiscordComponent[] ReturnButtonComponents() => new DiscordComponent[]
        {
            new DiscordButtonComponent(ButtonStyle.Success, $"EL_APROVE", string.Empty, emoji: new DiscordComponentEmoji(941666424324239430)),
            new DiscordButtonComponent(ButtonStyle.Danger, $"EL_DENY", string.Empty, emoji: new DiscordComponentEmoji(941666407513473054)),
            new DiscordButtonComponent(ButtonStyle.Secondary, $"EL_UPDATE", "Обновить список"),
            //new DiscordButtonComponent(ButtonStyle.Secondary, $"EL_EDIT", "Редактировать событие"),
            new DiscordButtonComponent(ButtonStyle.Secondary, $"EL_DAYOFF", "Оформить отгул")
        };



    }
}
