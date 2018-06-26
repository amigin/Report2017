using System;
using System.Threading.Tasks;
using AzureStorage;
using Microsoft.WindowsAzure.Storage.Table;
using Reports2016.Domains;

namespace Report2016.AzureRepositories
{
    public class VoteTokenEntity : TableEntity, IVoteToken
    {
        public static string GeneratePartitionKey(){
            return "t";
        }

        public static string GenerateRowKey(string token){
            return token;
        }

        public string Token => RowKey;
        public string Email { get; set; }

        public string ClientId { get; set; }

        public string FullName { get; set; }
    }


    public class VoteTokensRepository : IVoteTokensRepository
    {

        INoSQLTableStorage<VoteTokenEntity> _tableStorage;

        public VoteTokensRepository(INoSQLTableStorage<VoteTokenEntity> tableStorage)
        {
            _tableStorage = tableStorage;
        }

        public async Task<IVoteToken> FindTokenAsync(string token)
        {
            var partitionKey = VoteTokenEntity.GeneratePartitionKey();
            var rowKey = VoteTokenEntity.GenerateRowKey(token);

            return await _tableStorage.GetDataAsync(partitionKey, rowKey);
        }
    }
}
