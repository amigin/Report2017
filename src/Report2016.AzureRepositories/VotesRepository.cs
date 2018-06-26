using System;
using System.Threading.Tasks;
using AzureStorage;
using Microsoft.WindowsAzure.Storage.Table;
using Reports2016.Domains;
using Common;

namespace Report2016.AzureRepositories
{
    public class VoteEntity : TableEntity, IVote
    {
        public static string GeneratePartitionKey()
        {
            return "v";
        }

        public static string GenerateRowKey(string email)
        {
            return email.ToLower();
        }

        public string UserId { get; set; }

        public string Email { get; set; }


        public string Option { get; set; }

        public void SetOption(VoteOption src)
        {
            Option = src.ToString();
        }

        VoteOption IVote.Option => Option.ParseEnum<VoteOption>(VoteOption.NotSure);

        public string Comment {get;set;}

        public DateTime Created { get; set; }

        public static VoteEntity Create(IVote src)
        {
            var result = new VoteEntity
            {
                PartitionKey = GeneratePartitionKey(),
                RowKey = GenerateRowKey(src.Email),
                Email = src.Email,
                UserId = src.UserId,
                Comment = src.Comment,
                Created = src.Created
            };

            result.SetOption(src.Option);

            return result;

        }

    }


    public class VotesRepository : IVotesRepository
    {

        INoSQLTableStorage<VoteEntity> _tableStorage;

        public VotesRepository(INoSQLTableStorage<VoteEntity> tableStorage)
        {
            _tableStorage = tableStorage;
        }

        public async Task<IVote> GetAsync(string email)
        {
            var partitionKey = VoteEntity.GeneratePartitionKey();
            var rowKey = VoteEntity.GenerateRowKey(email);
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
