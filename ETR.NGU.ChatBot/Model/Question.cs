using System;
using System.Collections.Generic;

namespace ETR.NGU.ChatBot.Model
{
    [Serializable]
    public class Question
    {
        public int Index { get; set; }
        public string Text { get; set; }
        public string ImageUrl { get; set; }

        public List<string> Answers { get; set; }

        public string CorrectAnswer { get; set; }

        public string DescribeAnswerImageUrl { get; set; }
        public string DescribeAnswer { get; set; }
    }
}