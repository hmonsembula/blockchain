using Blockchain_Eligibility.Service.Models;
using System.Threading.Tasks;

namespace Blockchain_Eligibility.Service.Repositories
{
    public interface IEligibilityRepository
    {
        void Create(DailyDealEligibilityModel eligibility);
        Task<DailyDealEligibilityModel> Get(UserModel user);
        Task Update(DailyDealEligibilityModel eligibility);
    }
}
