using System.Collections.Generic;
using System.Web.Http;
using ETR.NGU.ChatBot.Model;
using ETR.NGU.ChatBot.Services;

namespace ETR.NGU.ChatBot.Controllers
{
   
    public class RegistrationController : ApiController
    {
        public List<PrinterUser> GetNewUsers() => DataProviderService.Instance.GetNewUsers();
    }
}
