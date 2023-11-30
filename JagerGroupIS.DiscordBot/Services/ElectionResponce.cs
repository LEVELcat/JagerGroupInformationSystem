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

        public ElectionResponce(DiscordBotDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task Responce(ComponentInteractionCreateEventArgs componentInteraction)
        {
            int inde = 1;

            try
            {
                Console.WriteLine("ER#" + inde++);

                var guidIDlong = unchecked((long)componentInteraction.Guild.Id);
                var channelIDlong = unchecked((long)componentInteraction.Channel.Id);
                var messageIDlong = unchecked((long)componentInteraction.Message.Id);
                var userIDlong = unchecked((long)componentInteraction.User.Id);

                Console.WriteLine("ER#" + inde++);

                Election election = await dbContext.Elections.FirstOrDefaultAsync(e => e.GuildID == guidIDlong &&
                                                                                       e.ChanelID == channelIDlong &&
                                                                                       e.MessageID == messageIDlong);

                Console.WriteLine("ER#" + inde++);

                if (election == null)
                    return;

                Console.WriteLine("ER#" + inde++);

                if (election.EndTimeUTC.CompareTo(DateTimeOffset.UtcNow) > 1)
                    return;

                Console.WriteLine("ER#" + inde++);

                var includedRolesID = election.RoleSetups.Where(x => x.IsTakingPart == true).Select(x => x.DisordRoleID).Select(x => unchecked((ulong)x));
                var excludedRolesID = election.RoleSetups.Where(x => x.IsTakingPart == false).Select(x => x.DisordRoleID).Select(x => unchecked((ulong)x));

                Console.WriteLine("ER#" + inde++);

                var member = await componentInteraction.Guild.GetMemberAsync(componentInteraction.User.Id);

                Console.WriteLine("ER#" + inde++);

                bool isAllowed = false;

                var idRoles = member.Roles.Select(x => x.Id).ToArray();

                Console.WriteLine("ER#" + inde++);

                if (Array.Exists(idRoles, r => includedRolesID.Contains(r)) == true)
                    if (Array.Exists(idRoles, r => excludedRolesID.Contains(r)) == false)
                        isAllowed = true;

                Console.WriteLine("ER#" + inde++);

                if (isAllowed == false)
                    return;

                Console.WriteLine("ER#" + inde++);

                User user = await dbContext.Users.FirstOrDefaultAsync(x => x.DiscordUserID == userIDlong);

                Console.WriteLine("ER#" + inde++);

                if (user == null)
                {
                    Console.WriteLine("ER#" + inde++);

                    user = new User()
                    {
                        DiscordUserID = userIDlong,
                    };
                    await dbContext.Users.AddAsync(user);
                    await dbContext.SaveChangesAsync();

                    Console.WriteLine("ER#" + inde++);
                }


                Console.WriteLine("ER#" + inde++);

                //Vote? lastVote = election.Votes.OrderByDescending(x => x.ID).FirstOrDefault(x => x.UserID == user.ID);

                Vote? lastVote = election.Votes.LastOrDefault(v => v.User.ID == user.ID);

                Console.WriteLine("ER#" + inde++);

                switch (componentInteraction.Id)
                {
                    case "EL_APROVE":
                        Console.WriteLine("ER#" + inde++);
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
                        Console.WriteLine("ER#" + inde++);
                        break;
                    case "EL_DENY":
                        Console.WriteLine("ER#" + inde++);
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
                            Console.WriteLine("ER#" + inde++);
                            await dbContext.Votes.AddAsync(new Vote()
                            {
                                ElectionID = election.ID,
                                VoteTimeUTC = DateTime.UtcNow,
                                VoteType = VoteType.None,
                                UserID = user.ID
                            });
                            await dbContext.SaveChangesAsync();
                        }
                        Console.WriteLine("ER#" + inde++);
                        break;
                    case "EL_UPDATE":
                        Console.WriteLine("ER#" + inde++);
                        break;
                    case "EL_DAYOFF":
                        try
                        {
                            Console.WriteLine("ER#" + inde++);
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
                            Console.WriteLine("ER#" + inde++);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("ER#" + inde++);
                            Console.WriteLine(ex.Message);
                            Console.WriteLine(ex.ToString());
                        }
                        Console.WriteLine("ER#" + inde++);
                        break;
                }
                Console.WriteLine("ER#" + inde++);
                DiscordMessageBuilder messageBuilder = new DiscordMessageBuilder(componentInteraction.Message);
                Console.WriteLine("ER#" + inde++);
                DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder(messageBuilder.Embed);
                Console.WriteLine("ER#" + inde++);
                embedBuilder.ClearFields();
                embedBuilder.AddField("<:emoji_134:941666424324239430> ", "ㅤ", true);
                embedBuilder.AddField("<:1_:941666407513473054> ", "ㅤ", true);
                embedBuilder.AddField("<a:load:1112311359548444713>  ", "ㅤ", true);
                Console.WriteLine("ER#" + inde++);
                embedBuilder.AddField("ㅤ", "ㅤ", true);
                embedBuilder.AddField("ㅤ", "ㅤ", true);
                embedBuilder.AddField("ㅤ", "ㅤ", true);
                Console.WriteLine("ER#" + inde++);
                embedBuilder.AddField("ㅤ", "ㅤ", true);
                embedBuilder.AddField("ㅤ", "ㅤ", true);
                embedBuilder.AddField("ㅤ", "ㅤ", true);
                Console.WriteLine("ER#" + inde++);
                byte columnIndex = 0;
                const byte maxRows = 3;
                const byte mentionsInField = 40;
                Console.WriteLine("ER#" + inde++);
                var fullMembers = componentInteraction.Guild.Members.ToArray();
                Console.WriteLine("ER#" + inde++);
                var members = (from m in fullMembers
                               let rolesId = m.Value.Roles.Select(r => r.Id).ToArray()
                               where
                                   (Array.Exists(rolesId, r => includedRolesID.Contains(r)) == true) &&
                                   (Array.Exists(rolesId, r => excludedRolesID.Contains(r)) == false)
                               select new { Id = unchecked((long)m.Value.Id), m.Value.Mention }).ToList();
                Console.WriteLine("ER#" + inde++);
                var votes = await dbContext.Votes.Where(v => v.ElectionID == election.ID)
                                                 .OrderBy(v => v.ID)
                                                 .GroupBy(x => x.UserID)
                                                 .ToArrayAsync();
                Console.WriteLine("ER#" + inde++);
                if (election.Settings.HasFlag(ElectionSettingsBitMask.AgreeList))
                {
                    Console.WriteLine("ER#" + inde++);
                    var yesList = (from v in votes
                                   let vL = v.Last()
                                   where vL.VoteType == VoteType.Agree
                                   orderby vL.VoteTimeUTC
                                   join m in members on vL.User.DiscordUserID equals m.Id
                                   select new { m.Id, m.Mention }).ToArray();

                    Console.WriteLine("ER#" + inde++);

                    foreach (var v in yesList)
                        members.RemoveAll(m => m.Id == v.Id);

                    embedBuilder.Fields[columnIndex].Name = "<:emoji_134:941666424324239430> " + yesList.Count();

                    Console.WriteLine("ER#" + inde++);

                    for (int i = 0; i < maxRows; i++)
                    {
                        embedBuilder.Fields[columnIndex + (i * 3)].Value = string.Join("\n", yesList.Skip(i * mentionsInField)
                                                                                                    .Take(mentionsInField)
                                                                                                    .Select(x => x.Mention));
                    }

                    Console.WriteLine("ER#" + inde++);

                    columnIndex++;
                }
                Console.WriteLine("ER#" + inde++);
                if (election.Settings.HasFlag(ElectionSettingsBitMask.RejectList))
                {
                    Console.WriteLine("ER#" + inde++);
                    var noList = (from v in votes
                                  let vL = v.Last()
                                  where vL.VoteType == VoteType.Reject
                                  orderby vL.VoteTimeUTC
                                  join m in members on vL.User.DiscordUserID equals m.Id
                                  select new { m.Id, m.Mention }).ToArray();
                    Console.WriteLine("ER#" + inde++);
                    foreach (var v in noList)
                        members.RemoveAll(m => m.Id == v.Id);
                    Console.WriteLine("ER#" + inde++);
                    embedBuilder.Fields[columnIndex].Name = "<:1_:941666407513473054> " + noList.Length;
                    Console.WriteLine("ER#" + inde++);
                    for (int i = 0; i < maxRows; i++)
                    {
                        embedBuilder.Fields[columnIndex + (i * 3)].Value = string.Join("\n", noList.Skip(i * mentionsInField)
                                                                                                   .Take(mentionsInField)
                                                                                                   .Select(x => x.Mention));
                    }
                    Console.WriteLine("ER#" + inde++);
                    columnIndex++;
                }

                Console.WriteLine("ER#" + inde++);
                if (election.Settings.HasFlag(ElectionSettingsBitMask.NotVotedList))
                {
                    Console.WriteLine("ER#" + inde++);
                    embedBuilder.Fields[columnIndex].Name = "<a:load:1112311359548444713> " + members.Count;
                    Console.WriteLine("ER#" + inde++);
                    for (int i = 0; i < maxRows; i++)
                    {
                        embedBuilder.Fields[columnIndex + (i * 3)].Value = string.Join("\n", members.Skip(i * mentionsInField)
                                                                                                    .Take(mentionsInField)
                                                                                                    .Select(x => x.Mention));
                    }
                    Console.WriteLine("ER#" + inde++);
                }
                Console.WriteLine("ER#" + inde++);
                messageBuilder.Embed = embedBuilder;
                Console.WriteLine("ER#" + inde++);
                await componentInteraction.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(messageBuilder));
                Console.WriteLine("ER#" + inde++);
                //componentInteraction.Message.ModifyAsync(messageBuilder);
                Console.WriteLine("ER#" + inde++);
                GC.Collect();
                Console.WriteLine("ER#" + inde++);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.ToString());
            }
        }
    }
}
