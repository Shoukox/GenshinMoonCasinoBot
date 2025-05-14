using brok1.Instance.Localization;
using brok1.Instance.Services;
using brok1.Instance.Types;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;
using System.Text;
using Telegram.Bot;

namespace brok1.Instance
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var botConfigurationSection = builder.Configuration.GetSection(BotConfiguration.Configuration);
            var botConfiguration = botConfigurationSection.Get<BotConfiguration>();
            builder.Services.AddSingleton<BotConfiguration>(botConfiguration);
            builder.Services.AddSingleton<RussianLocalization>();

            builder.Services.AddHttpClient("telegram_bot_client")
                            .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
                            {
                                BotConfiguration botConfig = sp.GetService<BotConfiguration>();
                                TelegramBotClientOptions options = new(botConfig!.BotToken);
                                return new TelegramBotClient(options, httpClient);
                            });
            // Dummy business-logic service
            builder.Services.AddScoped<UpdateHandler>();

            builder.Services.AddHostedService<ConfigureWebhook>();
            builder.Services.AddHostedService<NotifyAboutSpinService>();

            builder.Services
                .AddControllers()
                .AddNewtonsoftJson();
            
            //some opt
            ThreadPool.SetMaxThreads(Int16.MaxValue, Int16.MaxValue);
            ServicePointManager.DefaultConnectionLimit = Int16.MaxValue;

            var app = builder.Build();
            app.MapControllers();
            await app.RunAsync();
        }
    }
}