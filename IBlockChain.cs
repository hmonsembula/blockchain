using System.Collections.Generic;

namespace Blockchain_UserJourney.Common.Core.Models
{
    public interface IBlockChain
    {
        string Id { get; set; }
        string Name { get; set; }
        string TransactionId { get; set; }
        IList<IBlock> Transaction { get; set; }
        IBlock GetLastBlock();
        void Add(IBlock block);
        bool IsValid();
    }
}
