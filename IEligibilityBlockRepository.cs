using Blockchain_UserJourney.Common.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blockchain_Eligibility.Service.Repositories
{
    public interface IEligibilityBlockRepository
    {
        void Save(IBlockChain transaction);
        Task<IBlockChain> Get(string name, string id);
    }
}
