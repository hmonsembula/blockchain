using System;

namespace Blockchain_UserJourney.Common.Core.Models
{
    public interface IBlock
    {
        string Name { get; set; }
        int Index { get; set; }
        DateTime TimeStamp { get; set; }
        TimeSpan Duration { get; set; }
        string PreviousHash { get; set; }
        string Hash { get; set; }
        string Data { get; set; }
        string CalculateHash();
    }
}
