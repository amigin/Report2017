using System.Threading.Tasks;

namespace Reports2017.Domains
{
    public interface IVotesRepository
    {
        Task<VoteResult> VoteAsync(IVote vote);
        Task<IVote> GetAsync(string userId);
    }
}
