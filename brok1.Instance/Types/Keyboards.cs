using Telegram.Bot.Types.ReplyMarkups;

namespace brok1.Instance.Types
{
    internal class Keyboards
    {
        public static readonly ReplyKeyboardMarkup startButtons = new ReplyKeyboardMarkup(
           new KeyboardButton[][]{
                    new KeyboardButton[] { new KeyboardButton("☀️ Профиль"),  new KeyboardButton("💫 Рулетка") },
                    new KeyboardButton[] { new KeyboardButton("💰 Магазин"), new KeyboardButton("🎟️ Лотерея") },
                    new KeyboardButton[] { new KeyboardButton("📚 Информация"), new KeyboardButton("📖 Обратная связь") },
                    //new KeyboardButton[] { new KeyboardButton("🦋 Бесплатный код на Крылья!") }
               }
       )
        { ResizeKeyboard = true };
        public static readonly ReplyKeyboardMarkup adminStartButtons = new ReplyKeyboardMarkup(
                    startButtons.Keyboard.Append(
                    new KeyboardButton[]
                    {
                        new KeyboardButton("Админ панель"),
                    })
            )
        { ResizeKeyboard = true };

        public static readonly ReplyKeyboardMarkup ShopButtons = new ReplyKeyboardMarkup
                (
                    new KeyboardButton[][]
                    {
                    new KeyboardButton[]
                    {
                        new KeyboardButton("1 крутка: 19р"),
                        new KeyboardButton("5 круток: 79р (выгоднее на 15%)"),
                        new KeyboardButton("10 круток: 149р (выгоднее на 20%)"),
                    },
                     new KeyboardButton[]
                    {
                        new KeyboardButton("1 крутка high chance: 49р"),
                        new KeyboardButton("5 круток high chance: 209р (выгоднее на 15%)"),
                        new KeyboardButton("10 круток high chance: 399р (выгоднее на 20%)"),
                    },
                    new KeyboardButton[]
                    {
                        new KeyboardButton("Назад"),
                    }
                    }
                )
        { ResizeKeyboard = true };
        public static readonly ReplyKeyboardMarkup adminPanelButtons =
        new ReplyKeyboardMarkup(
                new KeyboardButton[][]
                {
                    new KeyboardButton[]
                    {
                        new KeyboardButton("Подкрутка"),
                        new KeyboardButton("Рассылка"),
                    },
                    new KeyboardButton[]
                    {
                        new KeyboardButton("Статистика"),
                        new KeyboardButton("Спонсоры"),
                    },
                    new KeyboardButton[]
                    {
                        new KeyboardButton("Назад"),
                    }
                }
            )
        { ResizeKeyboard = true };
    }
}
