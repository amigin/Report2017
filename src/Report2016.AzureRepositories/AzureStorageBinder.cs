using Reports2016.Domains;
using Microsoft.Extensions.DependencyInjection;
using Common.Log;
using AzureStorage.Tables;
using Lykke.SettingsReader;

namespace Report2016.AzureRepositories
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
