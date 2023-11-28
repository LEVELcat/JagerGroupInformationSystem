using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
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
    [SlashCommandGroup("голосование", "меню управления голосованиями")]
    [SlashCommandPermissions(DSharpPlus.Permissions.Administrator)]
    public class ElectionModule : ApplicationCommandModule
    {
        DiscordBotDbContext dbContext { get; }
        
        public ElectionModule(DiscordBotDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [SlashCommand("создать", "откроет меню создания голосования")]
        public async Task CreateElectionAsync(InteractionContext context)
        {
            new ElectionFactory(dbContext).CreateElectionAsync(context);
        }
    }
}
