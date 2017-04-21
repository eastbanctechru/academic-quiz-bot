namespace ETR.NGU.ChatBot
{
    public static class Consts
    {
        public static readonly string[] CorrectAnswers = { "Верно!", "Угадал!", "Молодец!", "Да, это так!", "Точно!", "Правильно!" };
        public static readonly string[] BadAnswers = { "Неверно.", "Ты не угадал.", "Нееееет!", "Сожалею, но…" };
        public static readonly string[] ExitCommands = { "/exit", "хватит", "надоело", "выход", "закончить", "завершить", "все", "конец", "стоп", "stop", "end", "exit" };

        public static readonly string[] GoodStickers = { "CAADAgADNAADmtX9DYR2kzhi5vtCAg", "CAADAgADOQADmtX9DcUaRCUMG8FyAg", "CAADAgADOgADmtX9DSsFSsoo5OXHAg" };
        public static readonly string[] BadStickers = { "CAADAgADNQADmtX9DTsQHyC9ciI0Ag", "CAADAgADNgADmtX9Dam0M3XDE7yUAg", "CAADAgADNwADmtX9DVA6KI51zmERAg", "CAADAgADOAADmtX9DcmamIZIYfS7Ag" };
        public static readonly string EstBancSticker = "CAADAgADOwADmtX9DVzpgAWgyJbcAg";

        public static class Telegram
        {
            public static string Token = "yuor telegram token";
        }
    }
}