using System;
using Reports2016.Domains;

namespace Report2016.Models
{
    public class MyVoteContract : IVote
    {

        public string Token { get; set; }

        public string Yes { get; set; }
        public string No { get; set; }
        public string NotSure { get; set; }

        public string Comment { get; set; }

        public string UserId { get; set; }

        public string Email { get; set; }

        public DateTime Created => DateTime.UtcNow;

        VoteOption IVote.Option {
            get{
                if (!string.IsNullOrEmpty(Yes))
                    return VoteOption.Yes;


				if (!string.IsNullOrEmpty(No))
					return VoteOption.No;

                if (!string.IsNullOrEmpty(NotSure))
					return VoteOption.NotSure;

                throw new Exception("Yes, No or NotSure required");

            }
        }


        public bool NotVoted(){
            return Yes == null && No == null && NotSure == null;
        }
    }
}
