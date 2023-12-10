using DSharpPlus;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Interactivity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.AsyncEvents;
using DSharpPlus.EventArgs;
using System.Reflection;
using JagerGroupIS.DiscordBot.Modules.Election;
using JagerGroupIS.DatabaseContext;
using JagerGroupIS.DiscordBot.Services;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace JagerGroupIS.DiscordBot
{
    public static class DiscordBot
    {
        public static DiscordClient Client { get ; private set; }

        //public static void Main(string[] args) => AsyncMain(args).GetAwaiter().GetResult() ;

        public static void Close() => Client.DisconnectAsync().GetAwaiter().GetResult();

        public static async Task AsyncMain(ConfigurationManager configurationManager)
        {
            try
            {
                var config = new DiscordConfiguration()
                {
                    Token = configurationManager.GetValue<string>("DiscordToken"),

                    TokenType = TokenType.Bot,
                    Intents = DiscordIntents.All,
                    MinimumLogLevel = LogLevel.Debug,
                    //LogTimestampFormat = "MMM dd yyyy - hh:mm:ss tt",
                };

                var discord = new DiscordClient(config);


                IServiceCollection serviceCollection = new ServiceCollection();

                serviceCollection.AddTransient<DiscordBotDbContext>();
                serviceCollection.AddTransient<ElectionResponce>();

                var services = serviceCollection.BuildServiceProvider();

                var shash = discord.UseSlashCommands(new SlashCommandsConfiguration()
                {
                    Services = services
                });

                shash.RegisterCommands<ElectionModule>();

                discord.UseInteractivity(new InteractivityConfiguration()
                {
                });

                discord.ComponentInteractionCreated += RegistrateInteractionEvent(services);

                await discord.ConnectAsync();

                Client = discord;

                await Task.Delay(-1);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.ToString());
            }
        }

        private static AsyncEventHandler<DiscordClient, ComponentInteractionCreateEventArgs> RegistrateInteractionEvent(ServiceProvider services)
        {
            return async (ctx, itteraction) =>
            {
                if (itteraction.Id.StartsWith("EL_"))
                {
                    if (services.GetService<ElectionResponce>() is ElectionResponce electionResponce)
                        electionResponce.Responce(itteraction);
                }
                else
                {

                }
            };
        }
    }
}
