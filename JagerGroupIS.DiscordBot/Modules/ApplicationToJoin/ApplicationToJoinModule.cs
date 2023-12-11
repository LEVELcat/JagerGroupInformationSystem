using DSharpPlus.SlashCommands;
using JagerGroupIS.DatabaseContext;
using JagerGroupIS.DiscordBot.Modules.Election;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JagerGroupIS.DiscordBot.Modules.ApplicationToJoin
{
    [SlashCommandGroup("заявки", "меню управления заявками")]
    [SlashCommandPermissions(DSharpPlus.Permissions.Administrator)]
    public class ApplicationToJoinModule : ApplicationCommandModule
    {
        public ApplicationToJoinModule()
        {

        }

        [SlashCommand("создать", "создает сообщение, для подачи заявок")]
        public async Task CreateApplicationToJoinMessageAsync(InteractionContext context)
        {
            new ApplicationToJoinCreator().CreateMessage(context);
        }
    }
}
