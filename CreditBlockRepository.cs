using Blockchain_Eligibility.Service.Settings;
using Blockchain_UserJourney.Common.Core.Models;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace Blockchain_Eligibility.Service.Repositories
{
    public class CreditBlockRepository : ICreditBlockRepository
    {
        private readonly IMongoCollection<IBlockChain> _collection;
        public CreditBlockRepository(ICreditBlockRepositorySettings repositorySettings)
        {
            var client = new MongoClient(repositorySettings.ConnectionString);
            var db = client.GetDatabase(repositorySettings.DataBaseName);
            _collection = db.GetCollection<IBlockChain>(repositorySettings.CollectionName);
        }

        public void Save(IBlockChain transaction)
        {
            if (transaction.IsValid())
            {
                var updateOptions = new UpdateOptions
                {
                    IsUpsert = true
                };
                FilterDefinition<IBlockChain> query = Builders<IBlockChain>.Filter.Eq("_id", transaction.Id);
                _collection.ReplaceOne(
                       query,
                          transaction,
                          updateOptions
                    );
            }
        }

        public async Task<IBlockChain> Get(string name, string id)
        {
            FilterDefinition<IBlockChain> query = Builders<IBlockChain>.Filter.Eq("_id", $"{name}-{id}");
            var result = await _collection.FindAsync(query);
            return result.FirstOrDefault();
        }
    }
}
