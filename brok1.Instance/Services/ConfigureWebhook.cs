using brok1.Instance.Types;
using brok1.Instance.Types.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace brok1.Instance.Services;

public class ConfigureWebhook : IHostedService
{
    private readonly ILogger<ConfigureWebhook> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly BotConfiguration _botConfig;

    public ConfigureWebhook(
        ILogger<ConfigureWebhook> logger,
        IServiceProvider serviceProvider,
        BotConfiguration botOptions)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _botConfig = botOptions;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
        Settings.LoadAllSettings(_botConfig);

        BotInfo.bot = await botClient.GetMeAsync();
        BotInfo.botClient = botClient;

        await InitializeAllGIFsAndPhotos(botClient);

        var webhookAddress = $"{_botConfig.HostAddress}/{_botConfig.Route}";
        _logger.LogInformation("Setting webhook: {WebhookAddress}", webhookAddress);

        await botClient.SetWebhookAsync(webhookAddress, cancellationToken: cancellationToken, dropPendingUpdates: true);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

        // Remove webhook on app shutdown
        _logger.LogInformation("Removing webhook");
        await botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);
    }

    private async Task InitializeAllGIFsAndPhotos(ITelegramBotClient botClient)
    {
        using var photoStream = File.OpenRead("1.png");
        using var gif_3Star = File.OpenRead("3star.mp4");
        using var gif_4Star = File.OpenRead("4star.mp4");
        using var gif_5Star = File.OpenRead("5star.mp4");

        var photoFile = new InputFileStream(photoStream, "1.png");
        var gif_3StarFile = new InputFileStream(gif_3Star, "3star.mp4");
        var gif_4StarFile = new InputFileStream(gif_4Star, "4star.mp4");
        var gif_5StarFile = new InputFileStream(gif_5Star, "5star.mp4");

        foreach (var gif in BotFile.AllGIFs)
        {
            gif.file_id = gif.file_name switch
            {
                "5star.mp4" => (await botClient.SendAnimationAsync(BotUser.ADMINS[0], gif_5StarFile)).Animation!.FileId,
                "4star.mp4" => (await botClient.SendAnimationAsync(BotUser.ADMINS[0], gif_4StarFile)).Animation!.FileId,
                "3star.mp4" => (await botClient.SendAnimationAsync(BotUser.ADMINS[0], gif_3StarFile)).Animation!.FileId
            };
        }

        foreach (var photo in BotFile.AllPhotos)
        {
            photo.file_id = photo.file_name switch
            {
                "1.png" => (await botClient.SendPhotoAsync(BotUser.ADMINS[0], photoFile)).Photo![0].FileId,
            };
        }

    }
}