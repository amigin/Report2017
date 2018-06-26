using System;

namespace Reports2017.Domains
{
    public interface IVote
    {
        string UserId { get; }
        string Email { get; }
        VoteOption Option1 { get; }
        VoteOption Option2 { get; }
        VoteOption Option3 { get; }
        string Comment { get; }
        DateTime Created { get; }
    }
}
