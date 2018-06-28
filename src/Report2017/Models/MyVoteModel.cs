using System;
using System.ComponentModel.DataAnnotations;
using Reports2017.Domains;

namespace Report2017.Models
{
    public class MyVoteContract : IVote
    {
        public string Token { get; set; }

        public VoteOption Option1 { get; set; }
        public VoteOption Option2 { get; set; }
        public VoteOption Option3 { get; set; }
        public string Comment { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public DateTime Created => DateTime.UtcNow;
        public string Name { get; set; }

        public bool NotVoted(){
            return Option1 == VoteOption.NotSure && Option2 == VoteOption.NotSure && Option3 == VoteOption.NotSure;
        }
    }
}
