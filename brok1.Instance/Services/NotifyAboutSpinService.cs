﻿using brok1.Instance.Localization;
using brok1.Instance.Types;
using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace brok1.Instance.Services
{
    internal class NotifyAboutSpinService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NotifyAboutSpinService> _logger;
        private readonly ITelegramBotClient _botClient;
        private readonly RussianLocalization localization;

        public NotifyAboutSpinService(IServiceProvider _serviceProvider, ILogger<NotifyAboutSpinService> _logger, ITelegramBotClient _botClient, RussianLocalization localization)
        {
            this._serviceProvider = _serviceProvider;
            this._logger = _logger;
            this._botClient = _botClient;
            this.localization = localization;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Notify Timer started its work");
            string sendText = localization.notify_Text();
            while (!stoppingToken.IsCancellationRequested)
            {
                for (int i = 0; i <= BotUser.AllUsers.Count - 1; i++)
                {
                    try
                    {
                        if ((BotUser.AllUsers[i].nextFreeSpin - DateTime.Now).TotalSeconds <= 60 && !BotUser.AllUsers[i].wasNotified && BotUser.AllUsers[i].notifyEnabled)
                        {
                            BotUser.AllUsers[i].wasNotified = true;

                            await _botClient.SendTextMessageAsync(BotUser.AllUsers[i].userid, sendText);
                            //await _botClient.SendTextMessageAsync(BotUser.ADMINS[0], sendText);
                            await Task.Delay(1000);

                            _logger.LogInformation($"Sent notifyMessage to user.id: {BotUser.AllUsers[i].userid}");
                        }
                    }
                    catch (ApiRequestException e)
                    {
                        //_logger.LogError(e.Message);
                        if (e.ErrorCode == 429)
                        {
                            //flood: retry after n
                            await Task.Delay(e.Parameters!.RetryAfter!.Value * 1000);
                        }
                        else if (e.ErrorCode == 400)
                        {
                            //chat not found
                            //todo
                        }
                        else if (e.ErrorCode == 403)
                        {
                            //blocked by user
                            BotUser.AllUsers[i].stoppedBot = true;
                        }
                        await Task.Delay(1000);
                    }
                }
                await Task.Delay(TimeSpan.FromSeconds(30));
            }
        }
    }
}