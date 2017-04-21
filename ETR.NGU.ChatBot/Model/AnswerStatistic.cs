using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ETR.NGU.ChatBot.Model
{
    [Serializable]
    public class AnswerStatistic
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Index { get; set; }
        public bool IsCorrect { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}