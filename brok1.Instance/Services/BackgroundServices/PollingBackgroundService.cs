using brok1.Instance.Services.Data;
using brok1.Instance.Types;
using brok1.Instance.Types.Utils;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace brok1.Instance.Services;

public class PollingBackgroundService(
    ILogger<PollingBackgroundService> logger,
    ITelegramBotClient botClient,
    UpdateQueueService updateQueueService,
    IOptions<BotConfiguration> botConfig) : BackgroundService
{
    private int? _offset = null;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await botClient.DeleteWebhook();
        Settings.LoadAllSettings(botConfig.Value);
        BotInfo.bot = await botClient.GetMe();
        BotInfo.botClient = botClient;
        await InitializeAllGIFsAndPhotos(botClient);

        logger.LogInformation("Starting polling background service");
        try
        {
            // Skip pending updates
            var pendingUpdates = await botClient.GetUpdates(timeout: 1);
            if (pendingUpdates.Length != 0) _offset = pendingUpdates.Last().Id + 1;
        }
        catch (OperationCanceledException e)
        {
            logger.LogError(e, "Operation cancelled");
            return;
        }
        await EnqueueAllUpdates(stoppingToken);
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
                "5star.mp4" => (await botClient.SendAnimation(BotUser.ADMINS[0], gif_5StarFile)).Animation!.FileId,
                "4star.mp4" => (await botClient.SendAnimation(BotUser.ADMINS[0], gif_4StarFile)).Animation!.FileId,
                "3star.mp4" => (await botClient.SendAnimation(BotUser.ADMINS[0], gif_3StarFile)).Animation!.FileId,
                _ => throw new NotSupportedException()
            };
        }

        foreach (var photo in BotFile.AllPhotos)
        {
            photo.file_id = photo.file_name switch
            {
                "1.png" => (await botClient.SendPhoto(BotUser.ADMINS[0], photoFile)).Photo![0].FileId,
                _ => throw new NotSupportedException()
            };
        }

    }

    private async Task EnqueueAllUpdates(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var updates = await botClient.GetUpdates(_offset, timeout: 30, cancellationToken: stoppingToken);
                foreach (var update in updates)
                {
                    _offset = update.Id + 1;
                    await updateQueueService.EnqueueUpdateAsync(update, CancellationToken.None);
                }
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning("Operation cancelled");
                return;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception");
            }
        }
    }
}