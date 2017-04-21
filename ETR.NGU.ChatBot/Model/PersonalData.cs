using System.ComponentModel.DataAnnotations.Schema;

namespace ETR.NGU.ChatBot.Model
{
    public class PersonalData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Specialization { get; set; }
        public string Year { get; set; }
        public string Interests { get; set; }
        public bool IsAccepted { get; set; }
        public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}