using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using ETR.NGU.ChatBot.Dialogs;
using ETR.NGU.ChatBot.Services;

namespace ETR.NGU.ChatBot.Controllers
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            try
            {
                if (activity.Type == ActivityTypes.Message)
                {
                    if (!string.IsNullOrEmpty(activity.Text))
                    {
                        await ProcessMessage(activity);
                    }
                }
                else
                {
                    await HandleSystemMessage(activity);
                }
            }
            catch (Exception ex)
            {
                await Reply(activity, $":-) {ex.Message} {ex.InnerException?.Message}");
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private async Task ProcessMessage(Activity activity)
        {
            string command = activity.Text.ToLower();
            if (command == "/getnextwinner")
            {
                var winner = DataProviderService.Instance.GetWinner();
                if (string.IsNullOrEmpty(winner))
                {
                    winner = "Нет таких";
                }
                await Reply(activity, winner);
            }
            else if (command == "/deleteuser")
            {
                await ClearStack(activity);
                DataProviderService.Instance.Delete(activity.From.Id);
                await Reply(activity, "User deleted");
            }
            else if (Consts.ExitCommands.Contains(command))
            {
                await ClearStack(activity);
                await Conversation.SendAsync(activity, () => new MainDialog(true));
            }
            else
            {
                await Conversation.SendAsync(activity, () => new MainDialog());
            }
        }

        private async Task ClearStack(Activity activity)
        {
            using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, activity))
            {
                var botData = scope.Resolve<IBotData>();
                await botData.LoadAsync(default(CancellationToken));
                var stack = scope.Resolve<IDialogStack>();
                stack.Reset();
                botData.UserData.Clear();
                await botData.FlushAsync(default(CancellationToken));
            }
        }

        private async Task Reply(Activity activity, string message)
        {
            var reply = activity.CreateReply();
            reply.Text = message;
            using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, activity))
            {
                var client = scope.Resolve<IConnectorClient>();
                await client.Conversations.ReplyToActivityAsync(reply);
            }
        }

        private async Task HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                await ClearStack(message);
                DataProviderService.Instance.Delete(message.From.Id);
                await Reply(message, "User deleted");
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }
        }
    }
}
