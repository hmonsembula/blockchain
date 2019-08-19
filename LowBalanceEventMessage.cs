using System;

namespace Blockchain_Eligibility.Service.Models
{
    public class LowBalanceEventMessage
    {
        public LowBalanceEventMessage(ConversionModel conversion, OfferModel offer)
        {
            EventId = Guid.NewGuid();
            Conversion = conversion;
            TimeStamp = DateTime.Now;
            Offer = offer;
        }
        public Guid EventId { get;}
        public ConversionModel Conversion { get;}
        public OfferModel Offer { get; }
        public DateTime TimeStamp { get;}
    }
}
