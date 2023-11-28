using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JagerGroupIS.Models.Enums;
using DSharpPlus.SlashCommands;
using DSharpPlus.Interactivity.Extensions;
using JagerGroupIS.Models.Database;

namespace JagerGroupIS.DiscordBot.Modules.Election
{
    public class ElectionSettings
    {
        public ElectionSettingsBitMask Settings
        {
            get => Election.Settings;
            set
            {
                Election.Settings = value;

                EmbedBuilder.ClearFields();

                if (Settings.HasFlag(ElectionSettingsBitMask.AgreeList))
                {
                    EmbedBuilder.AddField("<:emoji_134:941666424324239430>", "empty", true);
                    EmbedBuilder.AddField("ㅤ", "empty", true);
                    EmbedBuilder.AddField("ㅤ", "empty", true);
                }

                if (Settings.HasFlag(ElectionSettingsBitMask.RejectList))
                {
                    EmbedBuilder.AddField("<:1_:941666407513473054>", "empty", true);
                    EmbedBuilder.AddField("ㅤ", "empty", true);
                    EmbedBuilder.AddField("ㅤ", "empty", true);
                }

                if (Settings.HasFlag(ElectionSettingsBitMask.NotVotedList))
                {
                    EmbedBuilder.AddField("<a:load:1112311359548444713>", "empty", true);
                    EmbedBuilder.AddField("ㅤ", "empty", true);
                    EmbedBuilder.AddField("ㅤ", "empty", true);
                }
            }
        }

        private string title;
        public string Title
        {
            get => title;
            set
            {
                title = value;
                EmbedBuilder.Title = value;
            }
        }

        private string description;
        public string Description
        {
            get => description;
            set
            {
                description = value;
                EmbedBuilder.Description = FullDescription;
            }
        }

        private DateTime endDate;
        public DateTime EndDate
        {
            get => endDate;
            set
            {
                endDate = value;
                Election.EndTime = value;
                EmbedBuilder.Description = FullDescription;
            }
        }

        private string FullDescription
        {
            get => $"{Description}\n\nНачало: {Formatter.Timestamp(EndDate, TimestampFormat.LongDateTime)} {Formatter.Timestamp(EndDate)}";
        }

        private string? mainPictureURL;
        public string? MainPictureURL
        {
            get => mainPictureURL;
            set
            {
                try
                {
                    EmbedBuilder.ImageUrl = value;
                    mainPictureURL = value;
                }
                catch
                {
                    EmbedBuilder.ImageUrl = mainPictureURL;
                }
            }
        }

        private string? thumbnailPictureURL;
        public string? ThumbnailPictureURL
        {
            get => thumbnailPictureURL;
            set
            {
                try
                {
                    EmbedBuilder.WithThumbnail(value);
                    thumbnailPictureURL = value;

                }
                catch
                {
                    EmbedBuilder.WithThumbnail(thumbnailPictureURL);
                }
            }
        }

        private DiscordColor color;
        public DiscordColor Color
        {
            get => color;
            set
            {
                color = value;
                EmbedBuilder.Color = Color;
            }
        }

        private string? footerText;
        public string? FooterText
        {
            get => footerText;
            set
            {
                footerText = value;
                EmbedBuilder.WithFooter(FooterText);
            }
        }

        private string? footerUrl;
        public string? FooterUrl
        {
            get => footerUrl;
            set
            {
                try
                {
                    if (EmbedBuilder.Footer != null)
                    {
                        EmbedBuilder.WithFooter(FooterText, value);
                        footerUrl = value;
                    }
                }
                catch
                {

                }


            }
        }

        private DateTime? timeStamp;
        public DateTime? TimeStamp
        {
            get => timeStamp;
            set
            {
                timeStamp = value;
                EmbedBuilder.WithTimestamp(timeStamp);
            }
        }

        public Models.Database.Election  Election { get; private set; } = new Models.Database.Election();

        public DiscordEmbedBuilder EmbedBuilder { get; private set; } = new DiscordEmbedBuilder();

        private DiscordMessageBuilder messageBuilder = new DiscordMessageBuilder();
        public DiscordMessageBuilder MessageBuilder
        {
            get
            {
                messageBuilder.Embed = EmbedBuilder;
                return messageBuilder;
            }
            private set => messageBuilder = value;
        }

        private DiscordMessage roleSelectMessage;
        private DiscordMessage RoleSelectMessage
        {
            get => roleSelectMessage;
            set
            {
                if (roleSelectMessage != null)
                    roleSelectMessage.DeleteAsync();

                roleSelectMessage = value;
            }
        }

        private DiscordMessage electionListMessage;
        private DiscordMessage ElectionListMessage
        {
            get => electionListMessage;
            set
            {
                if (electionListMessage != null)
                    electionListMessage.DeleteAsync();

                electionListMessage = value;
            }
        }

        private DiscordMessage chanelSelectMessage;
        private DiscordMessage ChanelSelectMessage
        {
            get => chanelSelectMessage;
            set
            {
                if (chanelSelectMessage != null)
                    chanelSelectMessage.DeleteAsync();

                chanelSelectMessage = value;
            }
        }

        //TODO: Сделать
        //TODO: Че я хотел сделать?
        public bool IsReadyToPublish
        {
            get
            {
                return false;
            }
        }

        DiscordMessage? viewMessage { get; set; }


        public ElectionSettings(InteractionContext ctx)
        {
            Election.GuildID = unchecked((long)ctx.Guild.Id);

            SetDefaultSettings();
        }

        private void SetDefaultSettings()
        {
            Title = "Название";
            Description = "Описание";
            EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 20, 00, 00);
            Settings = ElectionSettingsBitMask.AgreeList | ElectionSettingsBitMask.RejectList | ElectionSettingsBitMask.NotVotedList |
                                   ElectionSettingsBitMask.NotificationBefore_15Minutes | ElectionSettingsBitMask.NotificationBefore_24Hour | ElectionSettingsBitMask.NotificationBefore_48Hour |
                                   ElectionSettingsBitMask.NotificationBefore_1Week;
            Color = new DiscordColor("#FFFFFF");
        }

        private async void EditViewMessage()
        {
            viewMessage?.ModifyAsync(MessageBuilder);
        }

        private void UpdateMessageContent(InteractionContext ctx)
        {
            MessageBuilder.Content = String.Join('\n', Election.RoleSetups.Where(x => x.IsTakingPart == true).Select(x => $"<@&{unchecked((ulong)x.DisordRoleID)}>"));
        }

        public async void ShowViewMessage(InteractionContext ctx)
        {
            if (viewMessage == null)
            {
                viewMessage = ctx.Channel.SendMessageAsync(MessageBuilder).Result;
            }
            else
            {
                EditViewMessage();
            }
        }

        public async void DeleteConstructorAsync()
        {
            viewMessage?.DeleteAsync();
            RoleSelectMessage?.DeleteAsync();
            ChanelSelectMessage?.DeleteAsync();
            ElectionListMessage?.DeleteAsync();
        }

        public async void ShowMainSettingInteraction(InteractionContext ctx, InteractivityResult<ComponentInteractionCreateEventArgs> eventArgs)
        {
            eventArgs.Result.Interaction.CreateResponseAsync(InteractionResponseType.Modal, _GetBulder());

            var input = ctx.Client.GetInteractivity();

            var txtResponce = await input.WaitForModalAsync("menu1", TimeSpan.FromMinutes(20));

            if (txtResponce.TimedOut)
                return;

            var values = txtResponce.Result.Values;
            var errorResponceStr = string.Empty;

            if (values["name"] != string.Empty)
                this.Title = values["name"];

            if (values["desc"] != string.Empty)
                this.Description = values["desc"];

            if (values["date"] != string.Empty)
                if (DateTime.TryParse(values["date"], out DateTime result))
                    this.EndDate = result;
                else
                    errorResponceStr += "Некорректная дата\n";

            //if (values["image"] != string.Empty)
            this.MainPictureURL = values["image"];

            //if (values["thumb"] != string.Empty)
            this.ThumbnailPictureURL = values["thumb"];

            if (errorResponceStr == string.Empty)
                txtResponce.Result.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage);
            else
                CreateErrorResponceAsync(txtResponce, errorResponceStr);

            EditViewMessage();

            DiscordInteractionResponseBuilder _GetBulder()
            {
                DiscordInteractionResponseBuilder responseBuilder = new DiscordInteractionResponseBuilder();

                responseBuilder.WithTitle("Настройки #1");

                responseBuilder.WithCustomId("menu1");

                responseBuilder.AddComponents(new TextInputComponent("Название", "name", value: this.Title));
                responseBuilder.AddComponents(new TextInputComponent("Дата события", "date", value: this.EndDate.ToString()));
                responseBuilder.AddComponents(new TextInputComponent("Описание события", "desc", value: this.Description, style: TextInputStyle.Paragraph));
                responseBuilder.AddComponents(new TextInputComponent("URL большой картинки снизу", "image", required: false,
                                                                     value: MainPictureURL != null ? MainPictureURL : null));
                responseBuilder.AddComponents(new TextInputComponent("URL картинки справа", "thumb", required: false,
                                                                     value: ThumbnailPictureURL != null ? ThumbnailPictureURL : null));

                return responseBuilder;
            }
        }

        public async void ShowOtherSettingInteraction(InteractionContext ctx, InteractivityResult<ComponentInteractionCreateEventArgs> eventArgs)
        {
            eventArgs.Result.Interaction.CreateResponseAsync(InteractionResponseType.Modal, _GetBulder());

            var input = ctx.Client.GetInteractivity();

            var txtResponce = await input.WaitForModalAsync("menu2", TimeSpan.FromMinutes(20));

            if (txtResponce.TimedOut)
                return;

            var values = txtResponce.Result.Values;
            var errorString = string.Empty;

            if (values["footerText"] != string.Empty)
                this.FooterText = values["footerText"];

            if (values["footerIcon"] != string.Empty)
                this.FooterUrl = values["footerIcon"];

            try
            {
                if (values["color"] != string.Empty)
                    Color = new DiscordColor(values["color"]);
            }
            catch
            {
                errorString += "Некорректный Hex код цвета\n";
            }

            if (errorString == string.Empty)
                txtResponce.Result.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage);
            else
                CreateErrorResponceAsync(txtResponce, errorString);


            EditViewMessage();

            DiscordInteractionResponseBuilder _GetBulder()
            {
                DiscordInteractionResponseBuilder responseBuilder = new DiscordInteractionResponseBuilder();

                responseBuilder.WithTitle("Настройки #1");

                responseBuilder.WithCustomId("menu2");

                responseBuilder.AddComponents(new TextInputComponent("Подпись снизу", "footerText", required: false,
                                                                     value: FooterText != null ? FooterText : null));
                responseBuilder.AddComponents(new TextInputComponent("URL Иконки возле подписи", "footerIcon", required: false,
                                                                     value: FooterUrl != null ? FooterUrl : null));
                responseBuilder.AddComponents(new TextInputComponent("Цвета слева", "color", required: false,
                                                                     value: $"#{Color.Value.ToString("X")}"));

                return responseBuilder;
            }
        }

        public async void ShowRoleSelectSettingMessage(InteractionContext ctx, InteractivityResult<ComponentInteractionCreateEventArgs> eventArgs)
        {
            eventArgs.Result.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage);

            RoleSelectMessage = await ctx.Channel.SendMessageAsync(_GetBuilder("Выберите роли участвующие в событии", "Участвуют", "includeList"));

            var responce = await RoleSelectMessage.WaitForSelectAsync("includeList", TimeSpan.FromMinutes(5));

            if (responce.TimedOut)
                RoleSelectMessage = null;
            else
            {
                var includedValues = responce.Result.Values;

                responce.Result.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage);

                RoleSelectMessage = await ctx.Channel.SendMessageAsync(_GetBuilder("Выберите роли исключаемые из голосования", "Исключенные", "excludeList"));

                responce = await RoleSelectMessage.WaitForSelectAsync("excludeList", TimeSpan.FromMinutes(5));

                if (responce.TimedOut)
                    RoleSelectMessage = null;
                else
                {
                    var excludedValues = responce.Result.Values;

                    Election.RoleSetups = new List<RoleElectionSetup>();

                    foreach (var include_Str in includedValues)
                    {
                        Election.RoleSetups.Add(
                            new RoleElectionSetup()
                            {
                                DisordRoleID = unchecked((long)ulong.Parse(include_Str)),
                                IsTakingPart = true,
                            });
                    }

                    foreach (var excluded_Str in excludedValues)
                    {
                        Election.RoleSetups.Add(
                            new RoleElectionSetup()
                            {
                                DisordRoleID = unchecked((long)ulong.Parse(excluded_Str)),
                                IsTakingPart = false,
                            });
                    }

                    UpdateMessageContent(ctx);

                    responce.Result.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage);

                    RoleSelectMessage = null;

                    EditViewMessage();
                }
            }


            DiscordMessageBuilder _GetBuilder(string Message, string PlaceHolder, string Id)
            {
                DiscordMessageBuilder builder = new DiscordMessageBuilder();
                builder.WithContent(Message);
                builder.AddComponents(new DiscordRoleSelectComponent(Id, Message, maxOptions: 5));

                return builder;
            }
        }

        public async void ShowElectionListSettingMessage(InteractionContext ctx, InteractivityResult<ComponentInteractionCreateEventArgs> eventArgs)
        {
            eventArgs.Result.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage);

            ElectionSettingsBitMask oldBitMask = Settings;

            ElectionListMessage = await ctx.Channel.SendMessageAsync(_GetBuilder());

            var responce = await ElectionListMessage.WaitForSelectAsync("listElection", TimeSpan.FromMinutes(5));

            if (responce.TimedOut)
                ElectionListMessage = null;
            else
            {
                responce.Result.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage);

                Settings = (ElectionSettingsBitMask)(responce.Result.Values.Select(x => Convert.ToInt64(x))
                                                                          .Sum());

                EditViewMessage();

                if (Settings.HasFlag(ElectionSettingsBitMask.NotificationForAgree) || Settings.HasFlag(ElectionSettingsBitMask.NotificationForNotVoted))
                {
                    ElectionListMessage = null;

                    ElectionListMessage = await ctx.Channel.SendMessageAsync(_GetBuilder2());

                    var timeResponce = await ElectionListMessage.WaitForSelectAsync("notificationTime", TimeSpan.FromMinutes(5));

                    Settings |= (ElectionSettingsBitMask)(timeResponce.Result.Values.Select(x => Convert.ToInt64(x))
                                                                                   .Sum());

                    Settings |= ElectionSettingsBitMask.NotificationBefore_15Minutes;
                }
            }

            ElectionListMessage = null;

            DiscordMessageBuilder _GetBuilder()
            {
                DiscordMessageBuilder builder = new DiscordMessageBuilder();
                builder.WithContent("Выберите нужные настройки");

                var options = new DiscordSelectComponentOption[]
                {
                        new DiscordSelectComponentOption("Список согласных", ((long)ElectionSettingsBitMask.AgreeList).ToString(),
                                                         isDefault: oldBitMask.HasFlag(ElectionSettingsBitMask.AgreeList) ? true : false),
                        new DiscordSelectComponentOption("Список отказавшихся", ((long)ElectionSettingsBitMask.RejectList).ToString(),
                                                         isDefault: oldBitMask.HasFlag(ElectionSettingsBitMask.RejectList) ? true : false),
                        new DiscordSelectComponentOption("Список воздержавшихся", ((long)ElectionSettingsBitMask.NotVotedList).ToString(),
                                                         isDefault: oldBitMask.HasFlag(ElectionSettingsBitMask.NotVotedList) ? true : false),
                        new DiscordSelectComponentOption("Уведомление для согласившихся", ((long)ElectionSettingsBitMask.NotificationForAgree).ToString(),
                                                         isDefault: oldBitMask.HasFlag(ElectionSettingsBitMask.NotificationForAgree) ? true : false),
                        new DiscordSelectComponentOption("Уведомление для воздежавшихся", ((long)ElectionSettingsBitMask.NotificationForNotVoted).ToString(),
                                                         isDefault : oldBitMask.HasFlag(ElectionSettingsBitMask.NotificationForNotVoted) ? true : false),
                };
                builder.AddComponents(new DiscordSelectComponent("listElection", null, options, maxOptions: options.Length));

                return builder;
            }

            DiscordMessageBuilder _GetBuilder2()
            {
                DiscordMessageBuilder builder = new DiscordMessageBuilder();
                builder.WithContent("Выберите за какое время до начала события оповещать о необходимости проголосовать и о начале события");

                var options = new DiscordSelectComponentOption[]
                {
                        //new DiscordSelectComponentOption("Уведомлять за 15 минут до события", ((ulong)BitMaskElection.NotificationBefore_15Minutes).ToString(),
                        //                                 isDefault: oldBitMask.HasFlag(BitMaskElection.NotificationBefore_15Minutes) ? true : false),
                        new DiscordSelectComponentOption("Уведомлять за час до события", ((long)ElectionSettingsBitMask.NotificationBefore_1Hour).ToString(),
                                                         isDefault: oldBitMask.HasFlag(ElectionSettingsBitMask.NotificationBefore_1Hour) ? true : false),
                        new DiscordSelectComponentOption("Уведомлять за 2 часа до события", ((long)ElectionSettingsBitMask.NotificationBefore_2Hour).ToString(),
                                                         isDefault: oldBitMask.HasFlag(ElectionSettingsBitMask.NotificationBefore_2Hour) ? true : false),
                        new DiscordSelectComponentOption("Уведомлять за 6 часов до события", ((long)ElectionSettingsBitMask.NotificationBefore_6Hour).ToString(),
                                                         isDefault: oldBitMask.HasFlag(ElectionSettingsBitMask.NotificationBefore_6Hour) ? true : false),
                        new DiscordSelectComponentOption("Уведомлять за 12 часов до события", ((long)ElectionSettingsBitMask.NotificationBefore_12Hour).ToString(),
                                                         isDefault : oldBitMask.HasFlag(ElectionSettingsBitMask.NotificationBefore_12Hour) ? true : false),
                        new DiscordSelectComponentOption("Уведомлять за день до события", ((long)ElectionSettingsBitMask.NotificationBefore_24Hour).ToString(),
                                                         isDefault : oldBitMask.HasFlag(ElectionSettingsBitMask.NotificationBefore_24Hour) ? true : false),
                        new DiscordSelectComponentOption("Уведомлять за два дня до события", ((long)ElectionSettingsBitMask.NotificationBefore_48Hour).ToString(),
                                                         isDefault : oldBitMask.HasFlag(ElectionSettingsBitMask.NotificationBefore_48Hour) ? true : false),
                        new DiscordSelectComponentOption("Уведомлять за неделю до события", ((long)ElectionSettingsBitMask.NotificationBefore_1Week).ToString(),
                                                         isDefault : oldBitMask.HasFlag(ElectionSettingsBitMask.NotificationBefore_1Week) ? true : false),
                        new DiscordSelectComponentOption("Уведомлять за 2 недели до события", ((long)ElectionSettingsBitMask.NotificationBefore_2Week).ToString(),
                                                         isDefault : oldBitMask.HasFlag(ElectionSettingsBitMask.NotificationBefore_2Week) ? true : false),
                        new DiscordSelectComponentOption("Уведомлять за месяц до события", ((long)ElectionSettingsBitMask.NotificationBefore_1Mounth).ToString(),
                                                         isDefault : oldBitMask.HasFlag(ElectionSettingsBitMask.NotificationBefore_1Mounth) ? true : false),
                };
                builder.AddComponents(new DiscordSelectComponent("notificationTime", null, options, maxOptions: options.Length));

                return builder;
            }
        }

        public async void ShowChanelSelectSettingMessage(InteractionContext ctx, InteractivityResult<ComponentInteractionCreateEventArgs> eventArgs)
        {
            eventArgs.Result.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage);

            ChanelSelectMessage = await ctx.Channel.SendMessageAsync(_GetBuilder());

            var responce = await ChanelSelectMessage.WaitForSelectAsync("chanels", TimeSpan.FromMinutes(5));

            if (responce.TimedOut)
                ChanelSelectMessage = null;
            else
            {
                responce.Result.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage);

                Election.ChanelID = unchecked((long)ulong.Parse(responce.Result.Values[0]));
            }

            ChanelSelectMessage = null;

            EditViewMessage();

            DiscordMessageBuilder _GetBuilder()
            {
                DiscordMessageBuilder builder = new DiscordMessageBuilder();
                builder.WithContent("Выберите категорию, в которой будет опубликовано событие");
                builder.AddComponents(new DiscordChannelSelectComponent("chanels", "выберите канал",
                                      new ChannelType[] { ChannelType.Text, ChannelType.Category }));

                return builder;
            }
        }

        private async void CreateErrorResponceAsync(InteractivityResult<ModalSubmitEventArgs> txtResponce, string errorString)
        {
            DiscordInteractionResponseBuilder responseBuilder = new DiscordInteractionResponseBuilder();
            responseBuilder.WithContent(errorString);
            responseBuilder.AsEphemeral();

            txtResponce.Result.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, responseBuilder);
        }


        //TODO: Сделать
        //TODO: Это походу про изменение существующих голосовалок

        public static bool TryParse(DiscordMessageBuilder discordMessage, out ElectionSettings electionSettings)
        {
            electionSettings = null;
            return false;
        }

        public static DiscordMessageBuilder DefaultMenuPanelMessage
        {
            get
            {
                var result = new DiscordMessageBuilder();

                result.WithContent("Настройки голосования");
                result.AddComponents(DefaultButtonMenuRows.AsEnumerable());

                return result;
            }
        }

        private static DiscordActionRowComponent[] DefaultButtonMenuRows
        {
            get
            {
                return new DiscordActionRowComponent[]
                {
                        new DiscordActionRowComponent(_firstPart()),
                        new DiscordActionRowComponent(_secondPart())
                };

                DiscordComponent[] _firstPart() => new DiscordComponent[]
                {
                    new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary,
                        "menu1",
                        "Настройка #1"
                    ),
                    new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary,
                        "menu2",
                        "Настройка #2"
                    ),
                    new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary,
                        "menu3",
                        "Настроить роли участвующие в событии"
                    ),
                    new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary,
                        "menu4",
                        "Настроить списки участников на событие"
                    ),

                    new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary,
                        "menu5",
                        "Выбрать канал для публикации"
                    ),

        };
                DiscordComponent[] _secondPart() => new DiscordComponent[]
                {
                    new DiscordButtonComponent
                    (
                        ButtonStyle.Success,
                        "menu8",
                        "Опубликовать голосование"
                    ),
                    new DiscordButtonComponent
                    (
                        ButtonStyle.Danger,
                        "menu9",
                        "Удалить"
                    )
                };
            }
        }
    }
}
