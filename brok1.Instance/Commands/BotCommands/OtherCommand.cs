using brok1.Instance.Commands.BotCommands.Handlers;
using brok1.Instance.Localization;
using brok1.Instance.Types;
using brok1.Instance.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace brok1.Instance.Commands.BotCommands
{
    public class OtherCommand : ICommand<Message>
    {
        private readonly ITelegramBotClient bot;
        private readonly Message message;
        private readonly CancellationToken ct;
        private readonly ILocalization localization;
        private readonly BotUser user;

        public OtherCommand(ITelegramBotClient bot, Message message, CancellationToken ct, ILocalization localization, BotUser user)
        {
            this.bot = bot;
            this.message = message;
            this.ct = ct;
            this.localization = localization;
            this.user = user;
        }
        public Task Execute()
        {
            if (BotFuncs.buttons.FirstOrDefault(m => m == message.Text) is string msgText)
            {
                Task func = msgText switch
                {
                    "☀️ Профиль" => ReplyCommandsHandler.Профиль(bot, message, user, localization),
                    "💫 Рулетка" => ReplyCommandsHandler.Рулетка(bot, message, user, localization),
                    "📖 Обратная связь" => ReplyCommandsHandler.Обратная_связь(bot, message, user, localization),
                    "Ваши пожелания" => ReplyCommandsHandler.Ваши_пожелания(bot, message, user, localization),
                    //"🦋 Бесплатный код на Крылья!" => ReplyCommandsHandler.Бесплатный_код_на_Крылья(bot, message, user, localization),
                    "Получить код" => ReplyCommandsHandler.Получить_код(bot, message, user, localization),
                    "Общая информация" => ReplyCommandsHandler.Общая_информация(bot, message, user, localization),
                    "Отзывы" => ReplyCommandsHandler.Отзывы(bot, message, user, localization),
                    "Отзывы участников" => ReplyCommandsHandler.Отзывы_участников(bot, message, user, localization),
                    "Написать отзыв" => ReplyCommandsHandler.Написать_отзыв(bot, message, user, localization),
                    "📚 Информация" => ReplyCommandsHandler.Информация(bot, message, user, localization),
                    "Назад" => ReplyCommandsHandler.Назад(bot, message, user, localization),
                    "🌙 Крутить Луну" => ReplyCommandsHandler.Крутить_луну(bot, message, user, localization),
                    "💰 Магазин" => ReplyCommandsHandler.Магазин(bot, message, user, localization),
                    "🎟️ Лотерея" => ReplyCommandsHandler.Лотерея(bot, message, user, localization),
                    "Рассылка" => ReplyCommandsHandler.Рассылка(bot, message, user, localization),
                    "Спонсоры" => ReplyCommandsHandler.Спонсоры(bot, message, user, localization),
                    "Уведомления" => ReplyCommandsHandler.Уведомления(bot, message, user, localization),
                    "Включить" => ReplyCommandsHandler.Включить_уведомления(bot, message, user, localization),
                    "Выключить" => ReplyCommandsHandler.Выключить_уведомления(bot, message, user, localization),
                    "🎟 Получить билетик" => ReplyCommandsHandler.Получить_билетик(bot, message, user, localization),
                    "Информация о розыгрыше" => ReplyCommandsHandler.Информация_о_розыгрыше(bot, message, user, localization),
                    "Админ панель" => ReplyCommandsHandler.Админ_панель(bot, message, user, localization),
                    "Статистика" => ReplyCommandsHandler.Статистика(bot, message, user, localization),
                    "Подкрутка" => ReplyCommandsHandler.Подкрутка(bot, message, user, localization),
                    "Шанс след. крутки" => ReplyCommandsHandler.Шанс_следующей_крутки(bot, message, user, localization),
                    "Изменить баланс" => ReplyCommandsHandler.Изменить_баланс(bot, message, user, localization),
                    "Изменить кол-во лун" => ReplyCommandsHandler.Изменить_количество_лун(bot, message, user, localization),
                    "Изменить кол-во круток" => ReplyCommandsHandler.Изменить_количество_круток(bot, message, user, localization),
                    "1 крутка: 19р" => ReplyCommandsHandler.крутка1_19(bot, message, user, localization),
                    "5 круток: 79р (выгоднее на 15%)" => ReplyCommandsHandler.крутка5_79(bot, message, user, localization),
                    "10 круток: 149р (выгоднее на 20%)" => ReplyCommandsHandler.крутка10_149(bot, message, user, localization),
                    "1 крутка high chance: 49р" => ReplyCommandsHandler.крутка1_49(bot, message, user, localization),
                    "5 круток high chance: 209р (выгоднее на 15%)" => ReplyCommandsHandler.крутка5_209(bot, message, user, localization),
                    "10 круток high chance: 399р (выгоднее на 20%)" => ReplyCommandsHandler.крутка10_399(bot, message, user, localization),
                    "Добавить кнопки" => ReplyCommandsHandler.Добавить_кнопки(bot, message, user, localization),
                    "Без кнопок" => ReplyCommandsHandler.Без_кнопок(bot, message, user, localization),
                    _ => Task.CompletedTask
                };
                return func;
            }
            else
            {
                if (user.isWishing)
                    return ChangedStatusHandler.isWishing(bot, message, user, localization);
                else if (user.isFeedbacking) 
                    return ChangedStatusHandler.isFeedbacking(bot, message, user, localization);
                else if (user.adminPanel.stage == EAdminPanelStage.WaitingForStringToEditSponsors) 
                    return ChangedStatusHandler.WaitingForStringToEditSponsors(bot, message, user, localization);
                else if (user.adminPanel.stage == EAdminPanelStage.WaitingForButtonText) 
                    return ChangedStatusHandler.WaitingForButtonText(bot, message, user, localization);
                else if (user.adminPanel.stage == EAdminPanelStage.WaitingForStringToNotifyAllUsers)
                    return ChangedStatusHandler.WaitingForStringToNotifyAllUsers(bot, message, user, localization);
                else if (user.adminPanel.stage == EAdminPanelStage.WaitingForNum)
                    return ChangedStatusHandler.WaitingForNum(bot, message, user, localization);
                else if (user.adminPanel.stage == EAdminPanelStage.WaitingForUserNameOrUserId)
                    return ChangedStatusHandler.WaitingForUserNameOrUserId(bot, message, user, localization);
                else if (user.paydata.payStatus == EPayStatus.WaitingForAmount)
                    return ChangedStatusHandler.WaitingForAmount(bot, message, user, localization);
                else if (user.stage == EStage.waitingForQIWINumber)
                    return ChangedStatusHandler.waitingForQIWINumber(bot, message, user, localization);
                else
                {
                    string sendText = localization.error_commandNotFound();
                    return bot.SendMessage(message.Chat.Id, sendText, parseMode: ParseMode.Html, replyParameters: message.MessageId);
                }
            }
        }
    }
}
