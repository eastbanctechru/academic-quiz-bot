using System;
using System.Collections.Generic;
using System.Linq;
using ETR.NGU.ChatBot.Model;

namespace ETR.NGU.ChatBot.Services
{
    public class DataProviderService
    {
        private static DataProviderService _instance;
        public static DataProviderService Instance => _instance ?? (_instance = new DataProviderService());

        public User GetUser(string userId)
        {
            using (var db = new AcademicQuizBotDb())
            {
                return db.Users.FirstOrDefault(u => u.UserId == userId);
            }
        }

        public ICollection<AnswerStatistic> GetAnswerStatistic(string userId)
        {
            using (var db = new AcademicQuizBotDb())
            {
                var user = db.Users.FirstOrDefault(u => u.UserId == userId);
                return  user?.AnswerStatistic;
            }
        }

        public User AddUser(string userId, string name)
        {
            var user = GetUser(userId);
            return user ?? AddNewUser(userId, name);
        }

        public void SetUserData(string userId, PersonalData userData)
        {
            using (var db = new AcademicQuizBotDb())
            {
                var user = db.Users.FirstOrDefault(u => u.UserId == userId);
                if (user == null) return;

                user.PersonalData.Add(userData);
                db.SaveChanges();
            }
        }

        public void AddAnswerStatistics(string userId, AnswerStatistic answerStatistics)
        {
            using (var db = new AcademicQuizBotDb())
            {
                var user = db.Users.FirstOrDefault(u => u.UserId == userId);
                if (user == null) return;

                user.AnswerStatistic.Add(answerStatistics);
                db.SaveChanges();
            }
        }

        public void ClearAnswerStatistics(string userId)
        {
            using (var db = new AcademicQuizBotDb())
            {
                var user = db.Users.FirstOrDefault(u => u.UserId == userId);
                if (user == null) return;
                db.AnswerStatistics.RemoveRange(user.AnswerStatistic);
                db.SaveChanges();
            }
        }

        public void ClearPersonalData(string userId)
        {
            using (var db = new AcademicQuizBotDb())
            {
                var user = db.Users.FirstOrDefault(u => u.UserId == userId);
                if (user == null) return;
                db.PersonalDatas.RemoveRange(user.PersonalData);
                db.SaveChanges();
            }
        }

        public void SetLiteQuizComlete(string userId)
        {
            using (var db = new AcademicQuizBotDb())
            {
                var user = db.Users.FirstOrDefault(u => u.UserId == userId);
                if (user == null) return;

                user.IsFullQuiz = true;
                db.SaveChanges();
            }
        }

        public void Delete(string userId)
        {
            ClearAnswerStatistics(userId);
            ClearPersonalData(userId);
            using (var db = new AcademicQuizBotDb())
            {
                var entity = db.Users.FirstOrDefault(e => e.UserId == userId);
                if (entity == null) return;
                db.Users.Remove(entity);
                db.SaveChanges();
            }
        }

        public List<PrinterUser> GetNewUsers()
        {
            using (var db = new AcademicQuizBotDb())
            {
                var newUsers = db.Users.Where(u => !u.IsPrintedTicked).ToList();
                var printUsers = new List<PrinterUser>();
                foreach (var user in newUsers)
                {
                    var personalData = user.PersonalData.FirstOrDefault();
                    if (personalData == null) continue;
                    printUsers.Add(new PrinterUser
                    {
                        Id = user.UserId,
                        FirstName = personalData.FirstName,
                        LastName = personalData.LastName,
                        TicketNumber = user.Id
                    });
                    user.IsPrintedTicked = true;
                }
                db.SaveChanges();
                return printUsers;
            }
        } 

        private User AddNewUser(string userId, string name)
        {
            using (var db = new AcademicQuizBotDb())
            {
                var user = new User
                {
                    UserId = userId,
                    Name = name
                };

                db.Users.Add(user);
                db.SaveChanges();
                return user;
            }
        }

        public string GetWinner()
        {
            using (var db = new AcademicQuizBotDb())
            {
                var userlist = db.Users.Where(u => u.PersonalData.Count > 0 && u.PersonalData.FirstOrDefault().IsAccepted).ToList();
                Random r = new Random();
                int count = userlist.Count;
                if (count == 0)
                {
                    return "";
                }
                int index = r.Next(count);
                var winner = userlist[index];
                var pd = winner.PersonalData.First();
                string winnerName = $"{pd.FirstName} {pd.LastName} {winner.Id}";
                return winnerName;
            }
        }
    }
}