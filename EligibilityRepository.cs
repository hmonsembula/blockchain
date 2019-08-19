using Blockchain_Eligibility.Service.Models;
using Blockchain_Eligibility.Service.Settings;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace Blockchain_Eligibility.Service.Repositories
{
    public class EligibilityRepository : IEligibilityRepository
    {
        private readonly IMongoCollection<DailyDealEligibilityModel> _collection;
        public EligibilityRepository(IEligibilityRepositorySettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var db = client.GetDatabase(settings.DataBaseName);
            _collection = db.GetCollection<DailyDealEligibilityModel>(settings.CollectionName);
        }
        public void Create(DailyDealEligibilityModel eligibility)
        {
            var updateOptions = new UpdateOptions
            {
                IsUpsert = true
            };
            FilterDefinition<DailyDealEligibilityModel> query = Builders<DailyDealEligibilityModel>.Filter.Eq("_id", eligibility.Id);
            _collection.ReplaceOne(
                   query,
                      eligibility,
                      updateOptions
                );
        }

        public async Task<DailyDealEligibilityModel> Get(UserModel user)
        {
            FilterDefinition<DailyDealEligibilityModel> query = Builders<DailyDealEligibilityModel>.Filter.Eq("_id", $"{user.GamingSystemId}-{user.ProductId}-{user.UserId}");
            var result = await _collection.FindAsync(query);
            return result.FirstOrDefault();
        }

        public async Task Update(DailyDealEligibilityModel eligibility)
        {
            await _collection.ReplaceOneAsync(doc => doc.Id == eligibility.Id, eligibility, new UpdateOptions { IsUpsert = true });
        }
    }
}
