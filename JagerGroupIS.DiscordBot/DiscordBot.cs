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

namespace JagerGroupIS.DiscordBot
{
    public static class DiscordBot
    {
        public static DiscordClient Client { get ; private set; }

        public static void Main(string[] args) => AsyncMain(args).GetAwaiter().GetResult() ;

        public static void Close() => Client.DisconnectAsync().GetAwaiter().GetResult();

        public static async Task AsyncMain(string[] args)
        {
            try
            {
                //TODO: Put Into Config file
                var config = new DiscordConfiguration()
                {
                    Token = "token",

                    TokenType = TokenType.Bot,
                    Intents = DiscordIntents.All,
                    MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,
                    LogTimestampFormat = "MMM dd yyyy - hh:mm:ss tt"
                };

                var discord = new DiscordClient(config);
                discord.UseInteractivity(new InteractivityConfiguration());

                IServiceCollection serviceCollection = new ServiceCollection();
                //serviceCollection.AddTransient<ElectionResponce>();
                //serviceCollection.AddSingleton<ElectionSingleton>();
                var services = serviceCollection.BuildServiceProvider();


                var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
                {
                    StringPrefixes = new[] { "!" },
                    Services = services
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
                    //if (services.GetService<Modules.ElectionModuleClasses.ElectionResponce>() is Modules.ElectionModuleClasses.ElectionResponce electionResponce)
                    //    electionResponce.Responce(itteraction);
                }
                else
                {

                }
            };
        }
    }
}
