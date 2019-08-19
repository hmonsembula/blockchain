using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Blockchain_UserJourney.Common.Core.Models
{
    public class BlockChain : IBlockChain
    {
        public BlockChain(string name, string id)
        {       
            CreateGenesis();
            Name = name;
            TransactionId = id;
            Id = $"{name}-{TransactionId}";
        }

        [BsonId]
        public string Id { get; set; }
        public string TransactionId { get; set; }
        public string Name { get; set; }

        public IList<IBlock> Transaction { get; set; }

        private void CreateGenesis()
        {
            Transaction = new List<IBlock>();
            IBlock genesis = new Block(0, "Genesis", "", TimeSpan.FromMilliseconds(0), "");
            Transaction.Add(genesis);
        }

        public void Add(IBlock block)
        {
            Transaction.Add(block);
        }

        public IBlock GetLastBlock()
        {
            return Transaction[Transaction.Count - 1];
        }

        public bool IsValid()
        {
            for (int i = 1; i < Transaction.Count; i++)
            {
                IBlock currentBlock = Transaction[i];
                IBlock previousBlock = Transaction[i - 1];

                if (currentBlock.Hash != currentBlock.CalculateHash())
                {
                    return false;
                }

                if (previousBlock.Hash != previousBlock.CalculateHash())
                {
                    return false;
                }

                if (currentBlock.PreviousHash != previousBlock.CalculateHash())
                {
                    return false;
                }
            }
            return true;
        }
    }
}
