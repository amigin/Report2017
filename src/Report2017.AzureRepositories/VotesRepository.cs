using System;
using System.Threading.Tasks;
using AzureStorage;
using Common;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;
using Microsoft.WindowsAzure.Storage.Table;
using Reports2017.Domains;

namespace Report2017.AzureRepositories
{
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateAlways)]
    public class VoteEntity : AzureTableEntity, IVote
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public VoteOption Option1 { get; set; }
        public VoteOption Option2 { get; set; }
        public VoteOption Option3 { get; set; }
        public string Comment {get;set;}
        public DateTime Created { get; set; }
        
        internal static string GeneratePartitionKey() => "v";
        internal static string GenerateRowKey(string userId) => userId;

        public static VoteEntity Create(IVote src)
        {
            var result = new VoteEntity
            {
                PartitionKey = GeneratePartitionKey(),
                RowKey = GenerateRowKey(src.UserId),
                Email = "---",
                UserId = src.UserId,
                Option1 = src.Option1,
                Option2 = src.Option2,
                Option3 = src.Option3,
                Comment = src.Comment,
                Created = src.Created
            };

            return result;
        }
    }

    public class VotesRepository : IVotesRepository
    {
        readonly INoSQLTableStorage<VoteEntity> _tableStorage;

        public VotesRepository(INoSQLTableStorage<VoteEntity> tableStorage)
        {
            _tableStorage = tableStorage;
        }

        public async Task<IVote> GetAsync(string userId)
        {
            var partitionKey = VoteEntity.GeneratePartitionKey();
            var rowKey = VoteEntity.GenerateRowKey(userId);
            return await _tableStorage.GetDataAsync(partitionKey, rowKey);
        }

        public async Task<VoteResult> VoteAsync(IVote vote)
        {
            try
            {
                var entity = VoteEntity.Create(vote);
                await _tableStorage.InsertAsync(entity);
                return VoteResult.Accepted;
            }
            catch (Exception)
            {
                return VoteResult.VoteIsAlreadyMade;
            }
        }
    }
}
