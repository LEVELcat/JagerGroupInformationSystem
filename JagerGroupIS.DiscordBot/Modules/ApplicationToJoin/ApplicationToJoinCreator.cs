using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;

namespace JagerGroupIS.DiscordBot.Modules.ApplicationToJoin
{
    internal class ApplicationToJoinCreator
    {
        public async void CreateMessage(InteractionContext context)
        {
            context.Interaction.CreateResponseAsync(DSharpPlus.InteractionResponseType.Modal, GetBuilderForm());

            var input = context.Client.GetInteractivity();

            var menuResponce = await input.WaitForModalAsync("ApplicationToJoinCreator", TimeSpan.FromMinutes(20));

            if (menuResponce.TimedOut)
                return;

            var values = menuResponce.Result.Values;
            var errorResponceStr = string.Empty;

            if (values["title"] == string.Empty)
                errorResponceStr += "Отсуствует название\n";

            if (values["chanel"] == string.Empty)
                errorResponceStr += "Отсуствует выбор канала";

            if (errorResponceStr != string.Empty)
                menuResponce.Result.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage);
            else
            {
                DiscordInteractionResponseBuilder responseBuilder = new DiscordInteractionResponseBuilder();
                responseBuilder.WithContent(errorResponceStr);
                responseBuilder.AsEphemeral();

                menuResponce.Result.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, responseBuilder);

            }
        }

        private DiscordInteractionResponseBuilder GetBuilderForm()
        {
            DiscordInteractionResponseBuilder responseBuilder = new DiscordInteractionResponseBuilder();

            responseBuilder.WithTitle("Настройки сообщения");

            responseBuilder.WithCustomId("ApplicationToJoinCreator");

            responseBuilder.AddComponents(new TextInputComponent("Название", "title"));
            responseBuilder.AddComponents(new TextInputComponent("Описание", "disc", required: false));
            responseBuilder.AddComponents(new TextInputComponent("Картинка снизу", "image", required: false));
            responseBuilder.AddComponents(new DiscordChannelSelectComponent("Канал, для публикации заявок", "chanel",
                                                                             channelTypes: new ChannelType[] { ChannelType.Text },
                                                                             minOptions: 1,
                                                                             maxOptions: 1));

            return responseBuilder;
        }
    }
}
