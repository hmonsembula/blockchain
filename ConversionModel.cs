using System;

namespace Blockchain_Eligibility.Service.Models
{
    public class ConversionModel : UserModel
    {
        public double Amount { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
