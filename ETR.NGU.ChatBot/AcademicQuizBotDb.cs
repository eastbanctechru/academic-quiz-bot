using System.Data.Entity;
using ETR.NGU.ChatBot.Model;

namespace ETR.NGU.ChatBot
{
    public partial class AcademicQuizBotDb : DbContext
    {
        public AcademicQuizBotDb()
            : base("name=AcademicQuizBotDb")
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<PersonalData> PersonalDatas { get; set; }
        public DbSet<AnswerStatistic> AnswerStatistics { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
