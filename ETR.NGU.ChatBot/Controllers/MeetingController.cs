using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ETR.NGU.ChatBot.Controllers
{
    public class MeetPoint
    {
        public string Time { get; set; }
        public string Place { get; set; }
    }
    public class MeetingController : ApiController
    {
        public MeetPoint GetMeetingPoint() => new MeetPoint { Time = "31 марта, 16.20, аудитория 4205 ", Place = "Нового корпуса НГУ" };
    }
}
