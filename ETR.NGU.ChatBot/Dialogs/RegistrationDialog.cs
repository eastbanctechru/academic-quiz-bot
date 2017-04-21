using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;

namespace ETR.NGU.ChatBot.Dialogs
{
    [Serializable]
    public class RegistrationDialog : IDialog<string>
    {
        private readonly List<string> _answers = new List<string> { "Зарегистрироваться", "Нет, спасибо" };
        async Task IDialog<string>.StartAsync(IDialogContext context)
        {
            context.Wait<string>(MessageReceived);
        }

        private Task MessageReceived(IDialogContext context, IAwaitable<string> message)
        {
           
            PromptDialog.Choice(context, AnswerReceived,
                  new PromptOptions<string>(
                      prompt: "Огонь! Твои знания Академа еще улучшатся, если ты будешь иногда болтать со мной 😊 А пока, предлагаю тебе встретиться с моим создателем на мастер-классе!",
                      retry: null,
                      tooManyAttempts: "Отмена",
                      options: _answers,
                      attempts: 0,
                      promptStyler: new PromptStyler(),
                      descriptions: _answers));
            return Task.FromResult("");
        }

        private async Task AnswerReceived(IDialogContext context, IAwaitable<string> message)
        {
            try
            {
                var answer = await message;
                if (answer == _answers[0])
                {
                    await context.Forward(Chain.From(() => FormDialog.FromForm(RegistrationForm.BuildForm)), RegistrationComplete, context.MakeMessage(), CancellationToken.None);
                }
                else
                {
                    context.Done("");
                }
            }
            catch
            {
                context.Done("");
            }
        }

        private Task RegistrationComplete(IDialogContext context, IAwaitable<RegistrationForm> result)
        {
            context.Done("");
            return Task.FromResult("");
        }
    }
}