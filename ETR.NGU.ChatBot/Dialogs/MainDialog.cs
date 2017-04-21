using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ETR.NGU.ChatBot.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace ETR.NGU.ChatBot.Dialogs
{
    [Serializable]
    public class MainDialog : IDialog<string>
    {
        private bool _onExit;
        public MainDialog(bool onExit = false)
        {
            _onExit = onExit;
        }
        async Task IDialog<string>.StartAsync(IDialogContext context)
        {
            context.Wait<string>(MessageReceived);
        }

        private async Task MessageReceived(IDialogContext context, IAwaitable<string> message)
        {
            var user = DataProviderService.Instance.GetUser(context.Activity.From.Id);
            if (user == null)
            {
                await context.PostAsync("Привет! Меня зовут Academic, а как тебя зовут?");
                context.Wait(Resume);
            }
            else
            {
                if (_onExit)
                {
                    _onExit = false;
                    await OnExit(context);
                }
                else
                {
                    await context.PostAsync($"Привет, {user.Name}! Давай поиграем!");
                    await StartQuiz(context);
                }
            }
        }

        private async Task Resume(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var activity = await result;
            if (!string.IsNullOrWhiteSpace(activity.Text))
            {
                var text = new StringBuilder();
                text.AppendLine($"Рад познакомиться, {activity.Text}, давай узнаем, так ли хорошо ты знаешь Академ.");
                text.AppendLine("Я проведу тебя по викторине, которую придумал Евгений Козионов. Веб-версия доступна в блоге автора: http://evgenykozionov.com/2016/02/09/moya-onlajn-viktorina-akademgorodok/");
                text.AppendLine("А мои создатели EastBanc Technologies http://www.eastbanctech.ru превратили викторину в чат-бот.");
                text.AppendLine("Ну что, 😉 Начнем!");
                await context.PostAsync(text.ToString());
                DataProviderService.Instance.AddUser(activity.From.Id, activity.Text);
                await StartQuiz(context);
            }
            else
            {
                await context.PostAsync("Очень необычное имя, мне такое даже не выговорить");
            }
        }

        private async Task StartQuiz(IDialogContext context)
        {
            await context.Forward(new QuizDialog(), QuizComplete, "", CancellationToken.None);
        }

        private async Task QuizComplete(IDialogContext context, IAwaitable<string> result)
        {
            await OnExit(context);
        }

        private async Task OnExit(IDialogContext context)
        {
            var user = DataProviderService.Instance.GetUser(context.Activity.From.Id);
           
            await context.PostAsync($"{user.Name}, отлично повеселились, приходи еще!");
            DataProviderService.Instance.ClearAnswerStatistics(context.Activity.From.Id);
           
            context.Done("");
        }
    }
}