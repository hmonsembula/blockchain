using Blockchain_UserJourney.Common.Core.Models;
using System.Threading.Tasks;

namespace Blockchain_Eligibility.Service.Repositories
{
    public interface ICreditBlockRepository
    {
        void Save(IBlockChain transaction);
        Task<IBlockChain> Get(string name, string id);
    }
}
