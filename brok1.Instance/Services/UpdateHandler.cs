using brok1.Instance.Commands;
using brok1.Instance.Commands.BotCallbacks;
using brok1.Instance.Commands.BotCommands;
using brok1.Instance.Localization;
using brok1.Instance.Types;
using brok1.Instance.Types.Utils;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace brok1.Instance.Services;

public class UpdateHandler : IUpdateHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<UpdateHandler> _logger;

    public UpdateHandler(ITelegramBotClient botClient, ILogger<UpdateHandler> logger)
    {
        _botClient = botClient;
        _logger = logger;
    }

    public async Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, HandleErrorSource handleErrorSource, CancellationToken cancellationToken)
    {
        string errorMessage = exception.ToString();
        if (exception is ApiRequestException apiRequestException)
        {
            errorMessage = $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}";
            if (apiRequestException.ErrorCode == 429)
            {
                //flood: retry after n
                await Task.Delay(3000);
            }
        }
        _logger.LogError($"{errorMessage}");
    }

    public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        try
        {
            var handler = update switch
            {
                { Message: { } message } => BotOnMessageReceivedAsync(message, cancellationToken),
                { EditedMessage: { } message } => BotOnMessageReceivedAsync(message, cancellationToken),
                { CallbackQuery: { } callbackQuery } => BotOnCallbackQueryReceived(callbackQuery, cancellationToken),
                _ => UnknownUpdateHandlerAsync(update, cancellationToken),
            };
            await handler;
        }
        catch (Exception exception)
        {
            await HandleErrorAsync(bot, exception, HandleErrorSource.HandleUpdateError, cancellationToken);
        }
    }

    private Task BotOnMessageReceivedAsync(Message message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Receive message type: {MessageType}", message.Type);
        if (message.Text is not { } messageText)
            return Task.CompletedTask;

        //some logic for locals
        //...
        ILocalization localization = new RussianLocalization();
        BotUser user = UsersManager.CheckUser(message.From!);

        ICommand<Message> action = messageText.Split(' ')[0] switch
        {
            "/start" => new StartCommand(_botClient, message, cancellationToken, localization, user),
            "/user" => new AdminUserInfoCommand(_botClient, message, cancellationToken, localization, user),
            "/test" => new TestCommand(_botClient, message, cancellationToken, localization, user),
            "/chance" => new AdminChanceCommand(_botClient, message, cancellationToken, localization, user),
            "/processed" => new AdminProcessedCommand(_botClient, message, cancellationToken, localization, user),
            "/forcesave" => new AdminForceSaveCommand(_botClient, message, cancellationToken, localization, user),
            "/listid" => new AdminGetFileWithUserIdsCommand(_botClient, message, cancellationToken, localization, user),
            _ => new OtherCommand(_botClient, message, cancellationToken, localization, user),
        };

        return action.Execute();
    }
    private Task UnknownUpdateHandlerAsync(Update update, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }
    private Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received inline keyboard callback from: {CallbackQueryId}", callbackQuery.Id);
        if (callbackQuery.Data is not { } callbackData)
            return Task.CompletedTask;

        //some logic for locals
        //...
        ILocalization localization = new RussianLocalization();
        BotUser user = UsersManager.CheckUser(callbackQuery.From!);

        ICommand<CallbackQuery> action = callbackData.Split(" ")[1] switch
        {
            "moneyAdd" => new MoneyAddCallback(_botClient, callbackQuery, cancellationToken, localization, user),
            "moneyPay" => new MoneyPayCallback(_botClient, callbackQuery, cancellationToken, localization, user),
            "checkbill" => new CheckBillCallback(_botClient, callbackQuery, cancellationToken, localization, user),
            "shop" => new ShopCallback(_botClient, callbackQuery, cancellationToken, localization, user),
            "moonout" => new MoonOutCallback(_botClient, callbackQuery, cancellationToken, localization, user),
            "referal" => new ReferalCallback(_botClient, callbackQuery, cancellationToken, localization, user),
            "referalInfo" => new ReferalInfoCallback(_botClient, callbackQuery, cancellationToken, localization, user),
            "replenish" => new ReplenishMoneyCallback(_botClient, callbackQuery, cancellationToken, localization, user),
            "withdraw" => new WithdrawMoneyCallback(_botClient, callbackQuery, cancellationToken, localization, user),
            "withdrawChoose" => new WithdrawChooseCallback(_botClient, callbackQuery, cancellationToken, localization, user),
            "notifyAllUsers" => new NotifyAllUsersCallback(_botClient, callbackQuery, cancellationToken, localization, user),
            "editrandom" => new EditRandomCallback(_botClient, callbackQuery, cancellationToken, localization, user),
            "checkFeedbacks" => new CheckFeedbacksCallback(_botClient, callbackQuery, cancellationToken, localization, user),
            _ => new DummyCallback(_botClient, callbackQuery, cancellationToken, localization, user)
        };
        _ = _botClient.AnswerCallbackQuery(callbackQuery.Id);

        return action.Execute();
    }
}