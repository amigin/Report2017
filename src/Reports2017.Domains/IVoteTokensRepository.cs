using System.Threading.Tasks;

namespace Reports2017.Domains
{
    public interface IVoteTokensRepository
    {
        Task<IVoteToken> FindTokenAsync(string token);
    }
}
