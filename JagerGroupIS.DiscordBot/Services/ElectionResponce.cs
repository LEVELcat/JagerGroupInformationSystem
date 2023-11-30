using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity.Extensions;
using JagerGroupIS.DatabaseContext;
using JagerGroupIS.Models.Database;
using JagerGroupIS.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JagerGroupIS.DiscordBot.Services
{
    public class ElectionResponce
    {
        DiscordBotDbContext dbContext { get; }

        ILogger logger { get; }

        public ElectionResponce(DiscordBotDbContext dbContext, ILogger logger )
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task Responce(ComponentInteractionCreateEventArgs componentInteraction)
        {
            int inde = 1;

            try
            {
                logger.LogDebug("ER#" + inde++);

                var guidIDlong = unchecked((long)componentInteraction.Guild.Id);
                var channelIDlong = unchecked((long)componentInteraction.Channel.Id);
                var messageIDlong = unchecked((long)componentInteraction.Message.Id);
                var userIDlong = unchecked((long)componentInteraction.User.Id);

                logger.LogDebug("ER#" + inde++);

                Election election = await dbContext.Elections.FirstOrDefaultAsync(e => e.GuildID == guidIDlong &&
                                                                                       e.ChanelID == channelIDlong &&
                                                                                       e.MessageID == messageIDlong);

                logger.LogDebug("ER#" + inde++);

                if (election == null)
                    return;

                logger.LogDebug("ER#" + inde++);

                if (election.EndTimeUTC < DateTime.UtcNow)
                    return;

                logger.LogDebug("ER#" + inde++);

                var includedRolesID = election.RoleSetups.Where(x => x.IsTakingPart == true).Select(x => x.DisordRoleID).Select(x => unchecked((ulong)x));
                var excludedRolesID = election.RoleSetups.Where(x => x.IsTakingPart == false).Select(x => x.DisordRoleID).Select(x => unchecked((ulong)x));

                logger.LogDebug("ER#" + inde++);

                var member = await componentInteraction.Guild.GetMemberAsync(componentInteraction.User.Id);

                logger.LogDebug("ER#" + inde++);

                bool isAllowed = false;

                var idRoles = member.Roles.Select(x => x.Id).ToArray();

                logger.LogDebug("ER#" + inde++);

                if (Array.Exists(idRoles, r => includedRolesID.Contains(r)) == true)
                    if (Array.Exists(idRoles, r => excludedRolesID.Contains(r)) == false)
                        isAllowed = true;

                logger.LogDebug("ER#" + inde++);

                if (isAllowed == false)
                    return;

                logger.LogDebug("ER#" + inde++);

                User user = await dbContext.Users.FirstOrDefaultAsync(x => x.DiscordUserID == userIDlong);

                logger.LogDebug("ER#" + inde++);

                if (user == null)
                {
                    logger.LogDebug("ER#" + inde++);

                    user = new User()
                    {
                        DiscordUserID = userIDlong,
                    };
                    await dbContext.Users.AddAsync(user);
                    await dbContext.SaveChangesAsync();

                    logger.LogDebug("ER#" + inde++);
                }


                logger.LogDebug("ER#" + inde++);

                //Vote? lastVote = election.Votes.OrderByDescending(x => x.ID).FirstOrDefault(x => x.UserID == user.ID);

                Vote? lastVote = election.Votes.LastOrDefault(v => v.User.ID == user.ID);

                logger.LogDebug("ER#" + inde++);

                switch (componentInteraction.Id)
                {
                    case "EL_APROVE":
                        logger.LogDebug("ER#" + inde++);
                        if (lastVote == null || lastVote.VoteType == VoteType.Reject || lastVote.VoteType == VoteType.None)
                        {
                            await dbContext.Votes.AddAsync(new Vote()
                            {
                                ElectionID = election.ID,
                                VoteTimeUTC = DateTime.UtcNow,
                                VoteType = VoteType.Agree,
                                UserID = user.ID
                            });
                            await dbContext.SaveChangesAsync();
                        }
                        else
                        {
                            await dbContext.Votes.AddAsync(new Vote()
                            {
                                ElectionID = election.ID,
                                VoteTimeUTC = DateTime.UtcNow,
                                VoteType = VoteType.None,
                                UserID = user.ID
                            });
                            await dbContext.SaveChangesAsync();
                        }
                        logger.LogDebug("ER#" + inde++);
                        break;
                    case "EL_DENY":
                        logger.LogDebug("ER#" + inde++);
                        if (lastVote == null || lastVote.VoteType == VoteType.Agree || lastVote.VoteType == VoteType.None)
                        {
                            await dbContext.Votes.AddAsync(new Vote()
                            {
                                ElectionID = election.ID,
                                VoteTimeUTC = DateTime.UtcNow,
                                VoteType = VoteType.Reject,
                                UserID = user.ID
                            });
                            await dbContext.SaveChangesAsync();
                        }
                        else
                        {
                            logger.LogDebug("ER#" + inde++);
                            await dbContext.Votes.AddAsync(new Vote()
                            {
                                ElectionID = election.ID,
                                VoteTimeUTC = DateTime.UtcNow,
                                VoteType = VoteType.None,
                                UserID = user.ID
                            });
                            await dbContext.SaveChangesAsync();
                        }
                        logger.LogDebug("ER#" + inde++);
                        break;
                    case "EL_UPDATE":
                        logger.LogDebug("ER#" + inde++);
                        break;
                    case "EL_DAYOFF":
                        try
                        {
                            logger.LogDebug("ER#" + inde++);
                            componentInteraction.Interaction.CreateResponseAsync(InteractionResponseType.Modal, _GetBuilder());

                            var input = DiscordBot.Client.GetInteractivity();

                            var txtResponce = await input.WaitForModalAsync("el_responce4", TimeSpan.FromMinutes(5));

                            if (txtResponce.TimedOut)
                                return;

                            var values = txtResponce.Result.Values;
                            var txt = values["text"];

                            txtResponce.Result.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage);

                            var chanel = componentInteraction.Guild.GetChannel(1161324095070290021);

                            await chanel.SendMessageAsync(_GetDayOffBuilderMessage());

                            if (lastVote == null || lastVote.VoteType == VoteType.None || lastVote.VoteType == VoteType.Agree)
                                goto case "EL_DENY";

                            DiscordMessageBuilder _GetDayOffBuilderMessage()
                            {
                                DiscordMessageBuilder messageBuilder = new DiscordMessageBuilder();

                                DiscordEmbedBuilder discordEmbed = new DiscordEmbedBuilder();

                                discordEmbed.Title = "Отгул";

                                discordEmbed.WithDescription(string.Empty);

                                discordEmbed.Description += componentInteraction.User.Mention + "\n";
                                discordEmbed.Description += "На событие: " + componentInteraction.Channel.Mention + "\n";
                                discordEmbed.Description += "По причине:\n";
                                discordEmbed.Description += txt;
                                discordEmbed.WithColor(new DiscordColor("#2E8B57"));

                                messageBuilder.AddEmbed(discordEmbed);

                                return messageBuilder;
                            }

                            DiscordInteractionResponseBuilder _GetBuilder()
                            {
                                DiscordInteractionResponseBuilder responseBuilder = new DiscordInteractionResponseBuilder();

                                responseBuilder.WithTitle("Оформить отгул");
                                responseBuilder.WithCustomId("el_responce4");

                                responseBuilder.AddComponents(new TextInputComponent("Причина для отгула", "text", value: string.Empty));

                                return responseBuilder;
                            }
                            logger.LogDebug("ER#" + inde++);
                        }
                        catch (Exception ex)
                        {
                            logger.LogDebug("ER#" + inde++);
                            Console.WriteLine(ex.Message);
                            Console.WriteLine(ex.ToString());
                        }
                        logger.LogDebug("ER#" + inde++);
                        break;
                }
                logger.LogDebug("ER#" + inde++);
                DiscordMessageBuilder messageBuilder = new DiscordMessageBuilder(componentInteraction.Message);
                logger.LogDebug("ER#" + inde++);
                DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder(messageBuilder.Embed);
                logger.LogDebug("ER#" + inde++);
                embedBuilder.ClearFields();
                embedBuilder.AddField("<:emoji_134:941666424324239430> ", "ㅤ", true);
                embedBuilder.AddField("<:1_:941666407513473054> ", "ㅤ", true);
                embedBuilder.AddField("<a:load:1112311359548444713>  ", "ㅤ", true);
                logger.LogDebug("ER#" + inde++);
                embedBuilder.AddField("ㅤ", "ㅤ", true);
                embedBuilder.AddField("ㅤ", "ㅤ", true);
                embedBuilder.AddField("ㅤ", "ㅤ", true);
                logger.LogDebug("ER#" + inde++);
                embedBuilder.AddField("ㅤ", "ㅤ", true);
                embedBuilder.AddField("ㅤ", "ㅤ", true);
                embedBuilder.AddField("ㅤ", "ㅤ", true);
                logger.LogDebug("ER#" + inde++);
                byte columnIndex = 0;
                const byte maxRows = 3;
                const byte mentionsInField = 40;
                logger.LogDebug("ER#" + inde++);
                var fullMembers = componentInteraction.Guild.Members.ToArray();
                logger.LogDebug("ER#" + inde++);
                var members = (from m in fullMembers
                               let rolesId = m.Value.Roles.Select(r => r.Id).ToArray()
                               where
                                   (Array.Exists(rolesId, r => includedRolesID.Contains(r)) == true) &&
                                   (Array.Exists(rolesId, r => excludedRolesID.Contains(r)) == false)
                               select new { Id = unchecked((long)m.Value.Id), m.Value.Mention }).ToList();
                logger.LogDebug("ER#" + inde++);
                var votes = await dbContext.Votes.Where(v => v.ElectionID == election.ID)
                                                 .OrderBy(v => v.ID)
                                                 .GroupBy(x => x.UserID)
                                                 .ToArrayAsync();
                logger.LogDebug("ER#" + inde++);
                if (election.Settings.HasFlag(ElectionSettingsBitMask.AgreeList))
                {
                    logger.LogDebug("ER#" + inde++);
                    var yesList = (from v in votes
                                   let vL = v.Last()
                                   where vL.VoteType == VoteType.Agree
                                   orderby vL.VoteTimeUTC
                                   join m in members on vL.User.DiscordUserID equals m.Id
                                   select new { m.Id, m.Mention }).ToArray();

                    logger.LogDebug("ER#" + inde++);

                    foreach (var v in yesList)
                        members.RemoveAll(m => m.Id == v.Id);

                    embedBuilder.Fields[columnIndex].Name = "<:emoji_134:941666424324239430> " + yesList.Count();

                    logger.LogDebug("ER#" + inde++);

                    for (int i = 0; i < maxRows; i++)
                    {
                        embedBuilder.Fields[columnIndex + (i * 3)].Value = string.Join("\n", yesList.Skip(i * mentionsInField)
                                                                                                    .Take(mentionsInField)
                                                                                                    .Select(x => x.Mention));
                    }

                    logger.LogDebug("ER#" + inde++);

                    columnIndex++;
                }
                logger.LogDebug("ER#" + inde++);
                if (election.Settings.HasFlag(ElectionSettingsBitMask.RejectList))
                {
                    logger.LogDebug("ER#" + inde++);
                    var noList = (from v in votes
                                  let vL = v.Last()
                                  where vL.VoteType == VoteType.Reject
                                  orderby vL.VoteTimeUTC
                                  join m in members on vL.User.DiscordUserID equals m.Id
                                  select new { m.Id, m.Mention }).ToArray();
                    logger.LogDebug("ER#" + inde++);
                    foreach (var v in noList)
                        members.RemoveAll(m => m.Id == v.Id);
                    logger.LogDebug("ER#" + inde++);
                    embedBuilder.Fields[columnIndex].Name = "<:1_:941666407513473054> " + noList.Length;
                    logger.LogDebug("ER#" + inde++);
                    for (int i = 0; i < maxRows; i++)
                    {
                        embedBuilder.Fields[columnIndex + (i * 3)].Value = string.Join("\n", noList.Skip(i * mentionsInField)
                                                                                                   .Take(mentionsInField)
                                                                                                   .Select(x => x.Mention));
                    }
                    logger.LogDebug("ER#" + inde++);
                    columnIndex++;
                }

                logger.LogDebug("ER#" + inde++);
                if (election.Settings.HasFlag(ElectionSettingsBitMask.NotVotedList))
                {
                    logger.LogDebug("ER#" + inde++);
                    embedBuilder.Fields[columnIndex].Name = "<a:load:1112311359548444713> " + members.Count;
                    logger.LogDebug("ER#" + inde++);
                    for (int i = 0; i < maxRows; i++)
                    {
                        embedBuilder.Fields[columnIndex + (i * 3)].Value = string.Join("\n", members.Skip(i * mentionsInField)
                                                                                                    .Take(mentionsInField)
                                                                                                    .Select(x => x.Mention));
                    }
                    logger.LogDebug("ER#" + inde++);
                }
                logger.LogDebug("ER#" + inde++);
                messageBuilder.Embed = embedBuilder;
                logger.LogDebug("ER#" + inde++);
                await componentInteraction.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(messageBuilder));
                logger.LogDebug("ER#" + inde++);
                //componentInteraction.Message.ModifyAsync(messageBuilder);
                logger.LogDebug("ER#" + inde++);
                GC.Collect();
                logger.LogDebug("ER#" + inde++);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.ToString());
            }
        }
    }
}
