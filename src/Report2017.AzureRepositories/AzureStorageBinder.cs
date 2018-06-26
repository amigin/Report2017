using AzureStorage.Tables;
using Common.Log;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;
using Reports2017.Domains;

namespace Report2017.AzureRepositories
{
    public static class AzureStorageBinder
    {
        public static void BindAzureRepositories(this IServiceCollection serviceCollection, IReloadingManager<string> manager, ILog log)
        {
            var votesRepository = new VotesRepository(AzureTableStorage<VoteEntity>.Create(manager, "Votes", log));
            serviceCollection.AddSingleton<IVotesRepository>(votesRepository);

            var voteTokensRepository = new VoteTokensRepository(AzureTableStorage<VoteTokenEntity>.Create(manager, "VoteTokens", log));
            serviceCollection.AddSingleton<IVoteTokensRepository>(voteTokensRepository);
        }
    }
}
