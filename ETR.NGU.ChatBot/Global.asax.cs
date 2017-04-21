using System.Data.Entity;
using System.Web.Http;

namespace ETR.NGU.ChatBot
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<AcademicQuizBotDb>());
        }
    }
}
