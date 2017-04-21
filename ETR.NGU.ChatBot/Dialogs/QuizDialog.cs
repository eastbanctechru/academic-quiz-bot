using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;
using ETR.NGU.ChatBot.Model;
using ETR.NGU.ChatBot.Services;
using Microsoft.Bot.Connector;
using Telegram.Bot;

namespace ETR.NGU.ChatBot.Dialogs
{
    [Serializable]
    public class QuizDialog : IDialog<string>
    {
        private const int LongTextLength = 30;
        private Question _question;
        async Task IDialog<string>.StartAsync(IDialogContext context)
        {
            context.Wait<string>(MessageReceived);
        }

        private async Task AnswerReceived(IDialogContext context, IAwaitable<string> message)
        {
            bool isCorrect = false;
            try
            {
                var answer = await message;
                
                isCorrect = answer == _question.CorrectAnswer;
                
            }
            catch
            {
                // ignored
            }
            await ShowResult(context, isCorrect);

        }

        private async Task LongAnswerReceived(IDialogContext context, IAwaitable<string> message)
        {
            bool isCorrect = false;
            try
            {
                var answer = await message;
                var answerNumber = int.Parse(answer) - 1;
                isCorrect = _question.Answers[answerNumber] == _question.CorrectAnswer;
            }
            catch
            {
                // ignored
            }
            await ShowResult(context, isCorrect);

        }

        private async Task ShowResult(IDialogContext context, bool isCorrect)
        {
            string isCorrectStr;
            Random rand = new Random();
            string sticker = "";
            if (isCorrect)
            {
                var index = rand.Next(Consts.CorrectAnswers.Length);
                isCorrectStr = Consts.CorrectAnswers[index];
                var stickerIndex = rand.Next(Consts.GoodStickers.Length);
                sticker = Consts.GoodStickers[stickerIndex];
            }
            else
            {
                var index = rand.Next(Consts.BadAnswers.Length);
                isCorrectStr = Consts.BadAnswers[index];
                var stickerIndex = rand.Next(Consts.BadStickers.Length);
                sticker = Consts.BadStickers[stickerIndex];
            }
            bool isTelegram = context.Activity.ChannelId == "telegram";
            if (isTelegram && rand.Next(100) > 30)
            {
                var bot = new TelegramBotClient(Consts.Telegram.Token);
                await bot.SendStickerAsync(context.Activity.From.Id, sticker);
            }

            var reply = context.MakeMessage();
            reply.Text = BoldString($"{isCorrectStr}");
            reply.TextFormat = "xml";
            await context.PostAsync(reply);

            var answer =  new AnswerStatistic { Index = _question.Index, IsCorrect = isCorrect };
            DataProviderService.Instance.AddAnswerStatistics(context.Activity.From.Id, answer);
            await ShowAnswerDescribe(context);
            await ShowStatistic(context);
            await ShowQuestion(context);
        }

        private async Task ShowStatistic(IDialogContext context)
        {
            var user = DataProviderService.Instance.GetUser(context.Activity.From.Id);
            var history = DataProviderService.Instance.GetAnswerStatistic(context.Activity.From.Id);

            var reply = context.MakeMessage();
            int correctCount = history.Count(h => h.IsCorrect);
            int questionsCount = QuestService.Instance.QuestionsCount;
            string text = $"{history.Count}/{questionsCount} Из них верно: {correctCount}";
            reply.Text = text;
            await context.PostAsync(reply);
        }


        private async Task MessageReceived(IDialogContext context, IAwaitable<string> message)
        {
            await ShowQuestion(context);
        }

        private async Task ShowAnswerDescribe(IDialogContext context)
        {
            var reply = context.MakeMessage();
            reply.Text = _question.DescribeAnswer + _question.DescribeAnswerImageUrl;
            await context.PostAsync(reply);
        }
       
        private async Task ShowQuestion(IDialogContext context)
        {
            var history = DataProviderService.Instance.GetAnswerStatistic(context.Activity.From.Id);

            var exceptIndexes = history.Select(h => h.Index).ToList();
            _question = QuestService.Instance.GetQuestion(exceptIndexes);

            if (_question == null)
            {
                context.Done("");
                return;
            }

            StringBuilder questionText = new StringBuilder();
            questionText.AppendLine(BoldString($"Вопрос №{history.Count + 1}:"));
            questionText.AppendLine(_question.Text);
            questionText.AppendLine(_question.ImageUrl);

            var reply = context.MakeMessage();
            reply.Text = questionText.ToString();
            reply.TextFormat = "xml";

            await context.PostAsync(reply);

            bool isLongAnswer = _question.Answers.Any(a => a.Length > LongTextLength);

            var answers = _question.Answers;
            if (isLongAnswer)
            {
                var answersReply = context.MakeMessage();
                StringBuilder text = new StringBuilder();
                answers = new List<string>();

                for (int i = 0; i < _question.Answers.Count; i++)
                {
                    text.AppendLine($"{i+1}) {_question.Answers[i]}");
                    answers.Add($"{i + 1}");
                }
                answersReply.Text = text.ToString();
                answersReply.TextFormat = "xml";
                await context.PostAsync(answersReply);
            }

            ResumeAfter<string> answerRecived = AnswerReceived;
            if (isLongAnswer)
            {
                answerRecived = LongAnswerReceived;
            }
            
            PromptDialog.Choice(context, answerRecived,
                  new PromptOptions<string>(
                      prompt: "Выберите один из вариантов. \n /exit - для отмены",
                      retry: null,
                      tooManyAttempts: "Неверно",
                      options: answers,
                      attempts: 0,
                      promptStyler: new PromptStyler(),
                      descriptions: answers));
        }

        private string BoldString(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }
            return $"<b>{text}</b>";
        }
    }
}