using System;

namespace Blockchain_Eligibility.Service.Models
{
    public class OfferModel
    {
        public OfferModel()
        {
            OfferId = Guid.NewGuid();
        }
        public Guid OfferId { get;}
        public string Description { get; set; }
        public double Percentage { get; set; }
        public bool IsClaimed { get; set; }
        public DateTime TimeStamp { get; set; }
        public DateTime ClaimDateTime { get; set; }
    }
}
