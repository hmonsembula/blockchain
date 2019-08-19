using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blockchain_Eligibility.Service.Models
{
    public class DailyDealEligibilityModel : UserModel
    {
        public DailyDealEligibilityModel()
        {
        }

        [BsonId]
        public string Id => $"{GamingSystemId}-{ProductId}-{UserId}";
        public DateTime TimeStamp { get; set; }
        public IList<OfferModel> OfferList { get; set; }

        
    }

    public static class Extensions
    {
        public static bool HasOffers(this DailyDealEligibilityModel eligibilityModel)
        {
            bool offers = eligibilityModel.OfferList.Any(o => o.IsClaimed == false);
            return offers;
        }

        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
