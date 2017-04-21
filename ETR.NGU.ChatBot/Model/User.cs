using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ETR.NGU.ChatBot.Model
{
    public class User
    {
        public User()
        {
            PersonalData = new HashSet<PersonalData>();
            AnswerStatistic = new HashSet<AnswerStatistic>();
        }

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public bool IsFullQuiz { get; set; }
        public bool IsPrintedTicked { get; set; }

        public virtual ICollection<PersonalData> PersonalData { get; set; }
        public virtual ICollection<AnswerStatistic> AnswerStatistic { get; set; }
    }
}