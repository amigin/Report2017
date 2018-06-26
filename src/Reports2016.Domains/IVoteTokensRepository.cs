using System;
using System.Threading.Tasks;

namespace Reports2016.Domains
{

    public interface IVoteToken
    {
        string Token { get; }
        string Email { get; }
        string ClientId { get; }
        string FullName { get; }
    }

    public interface IVoteTokensRepository
    {
        Task<IVoteToken> FindTokenAsync(string token);
    }

}
