using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using ETR.NGU.ChatBot.Model;
using Newtonsoft.Json;

namespace ETR.NGU.ChatBot.Services
{
    public class QuestService
    {
        private static QuestService _instance;
        public static QuestService Instance => _instance ?? (_instance = new QuestService());

        private static string _url = "https://academicquizbot.blob.core.windows.net/content/questions.txt";

        private readonly List<Question> _questions;

        public int QuestionsCount => _questions.Count;

        public QuestService()
        {
            var  root = DoRequest<QuestionRoot>(_url);
            _questions = root.Questions;

            for (int i = 0; i < _questions.Count; i++)
            {
                _questions[i].Index = i;
            }
        }

        private T DoRequest<T>(string requestStr) where T : class
        {
            var req = WebRequest.Create(new Uri(requestStr));
            req.Method = "GET";
            var response = req.GetResponse();
            var stream = response.GetResponseStream();
            if (stream == null)
            {
                return null;
            }
            
            using (var rstream = new StreamReader(stream))
            {
                var stringResult = rstream.ReadToEnd();
                var result = JsonConvert.DeserializeObject<T>(stringResult);
                return result;
            }
        }

        public Question GetQuestion(List<int> exceptIndexes)
        {
            var questions = _questions.Select(q => q).Where(q => !exceptIndexes.Contains(q.Index)).ToList();
            if (questions.Count == 0)
            {
                return null;
            }
            Random rand = new Random();
            return questions[rand.Next(questions.Count)];
        }
    }
}