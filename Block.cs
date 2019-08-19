using System;
using System.Security.Cryptography;
using System.Text;

namespace Blockchain_UserJourney.Common.Core.Models
{
    public class Block : IBlock
    {
        public Block(int previousIndex, string name, string data, TimeSpan duration, string previousHash)
        {
            Index = previousIndex + 1;
            Name = name;
            Data = data;
            TimeStamp = DateTime.Now;
            Duration = duration;
            PreviousHash = previousHash;
            Hash = CalculateHash();
        }

        public int Index { get; set; }
        public string Name { get; set; }
        public string Data { get; set; }
        public DateTime TimeStamp { get; set; }
        public TimeSpan Duration { get; set; }
        public string PreviousHash { get; set; }
        public string Hash { get; set; }

        public string CalculateHash()
        {
            SHA256 sha256 = SHA256.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes($"{Name}-{PreviousHash ?? ""}-{Data}");
            byte[] outputBytes = sha256.ComputeHash(inputBytes);
            return Convert.ToBase64String(outputBytes);
        }
    }
}
