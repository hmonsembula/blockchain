using System.Threading.Tasks;

namespace Blockchain_UserJourney.ActorSystem
{
    public interface IInsessionBlockchain
    {
        void Start();
        Task Stop();
    }
}
