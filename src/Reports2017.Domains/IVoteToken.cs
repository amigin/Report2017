namespace Reports2017.Domains
{
    public interface IVoteToken
    {
        string Token { get; }
        string Email { get; }
        string ClientId { get; }
        string FullName { get; }
    }
}
