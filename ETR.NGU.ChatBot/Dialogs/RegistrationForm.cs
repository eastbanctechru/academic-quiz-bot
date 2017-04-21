using System;
using ETR.NGU.ChatBot.Model;
using ETR.NGU.ChatBot.Services;
using Microsoft.Bot.Builder.FormFlow;


namespace ETR.NGU.ChatBot.Dialogs
{
    [Serializable]
    [Template(TemplateUsage.Bool, "{&}{||}")]
    [Template(TemplateUsage.String, "{&}")]
    
    public class RegistrationForm
    {
        [Describe("Пожалуйста, введите имя")]
        public string FirstName;
        [Describe("Пожалуйста, введите фамилию")]
        public string LastName;
        [Describe("Пожалуйста, введите номер телефона")]
        public string PhoneNumber;
        [Describe("Пожалуйста, введите email")]
        public string Email;
        [Describe("На каком факультете учитесь?")]
        public string Specialization;
        [Describe("На каком курсе?")]
        public string Year;
        [Describe("Какое направление или технологии интересны?")]
        public string Interests;
        [Describe("Согласны на обработку персональных данных? https://academicquizbot.blob.core.windows.net/content/PP.htm")]
        public bool IsAccepted;

        public static IForm<RegistrationForm> BuildForm()
        {
            OnCompletionAsyncDelegate<RegistrationForm> processOrder = async (context, state) =>
            {
                var personalData = new PersonalData
                {
                    Email = state.Email,
                    FirstName = state.FirstName,
                    Interests = state.Interests,
                    IsAccepted = state.IsAccepted,
                    LastName = state.LastName,
                    PhoneNumber = state.PhoneNumber,
                    Specialization = state.Specialization,
                    Year = state.Year
                };
                DataProviderService.Instance.SetUserData(context.Activity.From.Id, personalData);
                var reply = context.MakeMessage();
                reply.Text = "Сейчас сделаю для тебя билет, приноси его с собой на мастер класс!";
                await context.PostAsync(reply);
            };

            return new FormBuilder<RegistrationForm>()
                        .Message("Регистрация")
                        .AddRemainingFields()
                        .OnCompletion(processOrder)
                        .Build();
        }
    }
}