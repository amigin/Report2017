using System;
using System.Threading.Tasks;

namespace Reports2016.Domains
{

    public enum VoteOption
    {
        Yes, No, NotSure
    }

    public enum VoteResult
    {
        Accepted, VoteIsAlreadyMade
    }


    public interface IVote{
        string UserId { get; }
        string Email { get; }
        VoteOption Option { get; }
        string Comment { get; }
        DateTime Created { get; }
    }

    public interface IVotesRepository
    {
        Task<VoteResult> VoteAsync(IVote vote);
        Task<IVote> GetAsync(string email);

    }

}
