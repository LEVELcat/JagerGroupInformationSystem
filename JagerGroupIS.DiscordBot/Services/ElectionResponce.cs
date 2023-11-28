using DSharpPlus.Entities;
using DSharpPlus;
using DSharpPlus.EventArgs;
using JagerGroupIS.DatabaseContext;
using JagerGroupIS.Models.Database;
using JagerGroupIS.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Interactivity.Extensions;

namespace JagerGroupIS.DiscordBot.Services
{
    public class ElectionResponce
    {
        DiscordBotDbContext dbContext { get; }

        public ElectionResponce(DiscordBotDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task Responce(ComponentInteractionCreateEventArgs componentInteraction)
        {
            var guidIDlong = unchecked((long)componentInteraction.Guild.Id);
            var channelIDlong = unchecked((long)componentInteraction.Channel.Id);
            var messageIDlong = unchecked((long)componentInteraction.Message.Id);
            var userIDlong = unchecked((long)componentInteraction.User.Id);

            Election election = dbContext.Elections.FirstOrDefault(e => e.GuildID == guidIDlong &&
                                                                        e.ChanelID == channelIDlong &&
                                                                        e.MessageID == messageIDlong);

            if (election == null)
                return;

            var includedRolesID = election.RoleSetups.Where(x => x.IsTakingPart == true).Select(x => x.DisordRoleID).Select(x => unchecked((ulong)x));
            var excludedRolesID = election.RoleSetups.Where(x => x.IsTakingPart == false).Select(x => x.DisordRoleID).Select(x => unchecked((ulong)x));



            Vote? lastVote = election.Votes.LastOrDefault(v => v.User.DiscordUserID == userIDlong);

            var member = await componentInteraction.Guild.GetMemberAsync(componentInteraction.User.Id);

            bool isAllowed = false;

            var idRoles = member.Roles.Select(x => x.Id).ToArray();

            if(Array.Exists(idRoles, r => includedRolesID.Contains(r)) == true)
                if (Array.Exists(idRoles, r => excludedRolesID.Contains(r)) == false)
                    isAllowed = true;

            if (isAllowed == false)
                return;

            User user = await dbContext.Users.FirstOrDefaultAsync(x => x.DiscordUserID == userIDlong);

            if (user == null)
            {
                user = new User()
                {
                    DiscordUserID = userIDlong,
                };
                await dbContext.Users.AddAsync(user);
                await dbContext.SaveChangesAsync();
            }

            switch (componentInteraction.Id)
            {
                case "EL_APROVE":
                    if(lastVote == null || lastVote.VoteType == VoteType.Reject || lastVote.VoteType == VoteType.None)
                    {
                        await dbContext.Votes.AddAsync(new Vote()
                        {
                            Election = election,
                            VoteTimeUTC = DateTime.UtcNow,
                            VoteType = VoteType.Agree,
                            User = user
                        });
                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        await dbContext.Votes.AddAsync(new Vote()
                        {
                            Election = election,
                            VoteTimeUTC = DateTime.UtcNow,
                            VoteType = VoteType.None,
                            User = user
                        });
                        await dbContext.SaveChangesAsync();
                    }
                    break;
                case "EL_DENY":
                    if (lastVote == null || lastVote.VoteType == VoteType.Agree || lastVote.VoteType == VoteType.None)
                    {
                        await dbContext.Votes.AddAsync(new Vote()
                        {
                            Election = election,
                            VoteTimeUTC = DateTime.UtcNow,
                            VoteType = VoteType.Reject,
                            User = user
                        });
                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        await dbContext.Votes.AddAsync(new Vote()
                        {
                            Election = election,
                            VoteTimeUTC = DateTime.UtcNow,
                            VoteType = VoteType.None,
                            User = user
                        });
                        await dbContext.SaveChangesAsync();
                    }
                    break;
                case "EL_UPDATE":
                    break;
                case "EL_DAYOFF":
                    try
                    {
                        componentInteraction.Interaction.CreateResponseAsync(InteractionResponseType.Modal, _GetBuilder());

                        var input = DiscordBot.Client.GetInteractivity();

                        var txtResponce = await input.WaitForModalAsync("el_responce4", TimeSpan.FromMinutes(5));

                        if (txtResponce.TimedOut)
                            return;

                        var values = txtResponce.Result.Values;
                        var txt = values["text"];

                        txtResponce.Result.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage);

                        var chanel = componentInteraction.Guild.GetChannel(1143478141071929364);

                        await chanel.SendMessageAsync(_GetDayOffBuilderMessage());

                        var vote = await dbContext.Votes.FirstOrDefaultAsync(x => x.ElectionID == election.ID && x.UserID == userIDlong);

                        if (vote == null || vote.VoteType == VoteType.None)
                            goto case "EL_DENY";
                        else if (vote.VoteType == VoteType.Agree)
                            goto case "EL_APROVE";

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
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.ToString());
                    }
                    break;
            }

            DiscordMessageBuilder messageBuilder = new DiscordMessageBuilder(componentInteraction.Message);

            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder(messageBuilder.Embed);

            embedBuilder.ClearFields();
            embedBuilder.AddField("<:emoji_134:941666424324239430> ", "ㅤ", true);
            embedBuilder.AddField("<:1_:941666407513473054> ", "ㅤ", true);
            embedBuilder.AddField("<a:load:1112311359548444713>  ", "ㅤ", true);

            embedBuilder.AddField("ㅤ", "ㅤ", true);
            embedBuilder.AddField("ㅤ", "ㅤ", true);
            embedBuilder.AddField("ㅤ", "ㅤ", true);

            embedBuilder.AddField("ㅤ", "ㅤ", true);
            embedBuilder.AddField("ㅤ", "ㅤ", true);
            embedBuilder.AddField("ㅤ", "ㅤ", true);

            byte columnIndex = 0;
            const byte maxRows = 3;
            const byte mentionsInField = 40;

            var fullMembers = componentInteraction.Guild.Members.ToArray();

            var members = (from m in fullMembers
                           let rolesId = m.Value.Roles.Select(r => r.Id).ToArray()
                           where
                               (Array.Exists(rolesId, r => includedRolesID.Contains(r)) == true) &&
                               (Array.Exists(rolesId, r => excludedRolesID.Contains(r)) == false)
                           select new { Id = unchecked((long)m.Value.Id) , m.Value.Mention}).ToList();

            var votes = (from v in election.Votes
                         orderby v.VoteTimeUTC
                         group v by v.UserID).ToArray();

            if (election.Settings.HasFlag(ElectionSettingsBitMask.AgreeList))
            {
                var yesList = (from v in votes
                               let vL = v.Last()
                               where vL.VoteType == VoteType.Agree
                               orderby vL.VoteTimeUTC
                               join m in members on vL.User.DiscordUserID equals m.Id
                               select new { m.Id, m.Mention }).ToArray();

                foreach (var v in yesList)
                    members.RemoveAll(m => m.Id == v.Id);

                embedBuilder.Fields[columnIndex].Name = "<:emoji_134:941666424324239430> " + yesList.Length;

                for (int i = 0; i < maxRows; i++)
                {
                    embedBuilder.Fields[columnIndex + (i * 3)].Value = string.Join("\n", yesList.Skip(i * mentionsInField)
                                                                                                .Take(mentionsInField)
                                                                                                .Select(x => x.Mention));
                }

                columnIndex++;
            }

            if (election.Settings.HasFlag(ElectionSettingsBitMask.RejectList))
            {
                var noList = (from v in votes
                              let vL = v.Last()
                              where vL.VoteType == VoteType.Reject
                              orderby vL.VoteTimeUTC
                              join m in members on vL.User.DiscordUserID equals m.Id
                              select new { m.Id, m.Mention }).ToArray();

                foreach (var v in noList)
                    members.RemoveAll(m => m.Id == v.Id);

                embedBuilder.Fields[columnIndex].Name = "<:1_:941666407513473054> " + noList.Length;

                for (int i = 0; i < maxRows; i++)
                {
                    embedBuilder.Fields[columnIndex + (i * 3)].Value = string.Join("\n", noList.Skip(i * mentionsInField)
                                                                                               .Take(mentionsInField)
                                                                                               .Select(x => x.Mention));
                }

                columnIndex++;
            }

            if (election.Settings.HasFlag(ElectionSettingsBitMask.NotVotedList))
            {
                embedBuilder.Fields[columnIndex].Name = "<a:load:1112311359548444713> " + members.Count;

                for (int i = 0; i < maxRows; i++)
                {
                    embedBuilder.Fields[columnIndex + (i * 3)].Value = string.Join("\n", members.Skip(i * mentionsInField)
                                                                                                .Take(mentionsInField)
                                                                                                .Select(x => x.Mention));
                }
            }

            messageBuilder.Embed = embedBuilder;

            await componentInteraction.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage);

            componentInteraction.Message.ModifyAsync(messageBuilder);

            dbContext.DisposeAsync();
            GC.Collect();
        }
    }
}
