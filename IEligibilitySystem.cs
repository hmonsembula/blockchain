using System.Threading.Tasks;

namespace Blockchain_Eligibility.Service.System
{
    public interface IEligibilitySystem
    {
        void Start();
        Task Stop();
    }
}
