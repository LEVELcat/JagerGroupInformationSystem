using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JagerGroupIS.DiscordBot.Modules.Election
{
    public class ElectionBuilder
    {
        public async Task<(Models.Database.Election, DiscordMessageBuilder)> BuildElectionAsync(InteractionContext context)
        {
            ElectionSettings electionSettings = new ElectionSettings(context);

            electionSettings.ShowViewMessage(context);
            var menuMessage = await context.Channel.SendMessageAsync(ElectionSettings.DefaultMenuPanelMessage);

            bool isExit = false;

            for (bool isFinished = false; isFinished == false;)
            {
                var respond = await menuMessage.WaitForButtonAsync(TimeSpan.FromMinutes(30));

                if (respond.TimedOut == true)
                    isExit = true;
                else
                {

                    switch (respond.Result.Id)
                    {
                        case "menu1":
                            electionSettings.ShowMainSettingInteraction(context, respond);
                            break;
                        case "menu2":
                            electionSettings.ShowOtherSettingInteraction(context, respond);
                            break;
                        case "menu3":
                            electionSettings.ShowRoleSelectSettingMessage(context, respond);
                            break;
                        case "menu4":
                            electionSettings.ShowElectionListSettingMessage(context, respond);
                            break;
                        case "menu5":
                            electionSettings.ShowChanelSelectSettingMessage(context, respond);
                            break;
                        case "menu8":
                            isFinished = true;
                            respond.Result.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage);
                            break;
                        case "menu9":
                            isExit = true;
                            respond.Result.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage);
                            break;

                    }
                }
                if (isExit == true)
                    break;
            }

            electionSettings.DeleteConstructorAsync();
            menuMessage.DeleteAsync();

            if (isExit == true)
                return new(null, null);
            else
                return new(electionSettings.Election, electionSettings.MessageBuilder);
        }
    }
}
