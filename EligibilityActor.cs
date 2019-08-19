using Akka.Actor;
using Blockchain_Eligibility.Service.Models;
using Blockchain_Eligibility.Service.Repositories;
using Blockchain_UserJourney.Common.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Blockchain_Eligibility.Service.Actors
{
    public class EligibilityActor : ReceiveActor
    {
        private readonly ILogger _logger;
        private readonly IEligibilityRepository _eligibilityRepository;
        private readonly IEligibilityBlockRepository _eligibilityBlockRepository;
        private readonly IExperienceBlockRepository _experienceBlockRepository;
        private readonly ICreditBlockRepository _creditBlockRepository;
        private readonly IActorRef _blockRef;
        private const string INSESSION_TRANSACTION_NAME = "Insession.Transaction";

        public EligibilityActor(IActorRef blockRef, ILoggerFactory loggerFactory, IEligibilityRepository eligibilityRepository, IEligibilityBlockRepository eBlockRepository, IExperienceBlockRepository experienceBlockRepository, ICreditBlockRepository creditBlockRepository)
        {
            _logger = new Logger<EligibilityActor>(loggerFactory);
            _eligibilityRepository = eligibilityRepository;
            _eligibilityBlockRepository = eBlockRepository;
            _experienceBlockRepository = experienceBlockRepository;
            _creditBlockRepository = creditBlockRepository;
            _blockRef = blockRef;
            Receive<DailyDealEligibilityModel>(_ => HandleDailyDealEligibility(_));
            Receive<ConversionModel>(_ => HandleConversion(_));
            Receive<LowBalanceEventMessage>(_ => HandleLowBalance(_));
            Receive<InsessionEligibility>(_ => HandleEligibility(_));
        }

        private void HandleDailyDealEligibility(DailyDealEligibilityModel eligibility)
        {
            _logger.LogInformation("Received new eligibility");
            var watch = Stopwatch.StartNew();
            _eligibilityRepository.Create(eligibility);
            var tId = $"{eligibility.GamingSystemId}-{eligibility.ProductId}-{eligibility.UserId}";
            IBlockChain transaction = new BlockChain(INSESSION_TRANSACTION_NAME, tId);
            IBlock lastBlock = transaction.GetLastBlock();
            IBlock currentBlock = new Block(lastBlock.Index, "Daily_Deal_Created_v1", eligibility.ToJson(), watch.Elapsed, lastBlock.Hash);
            transaction.Add(currentBlock);

            if (transaction.IsValid())
            {
                var insessTransaction = new TransactionMessage.Insession(transaction);
                PubishTransction(insessTransaction);

                var conversion = new ConversionModel()
                {
                    Amount = 250,
                    GamingSystemId = eligibility.GamingSystemId,
                    ProductId = eligibility.ProductId,
                    UserId = eligibility.UserId
                };
                Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(10), Self, conversion, Self);
            }
        }

        private void PubishInsessionEligibility(InsessionEligibility eligibility)
        {
            Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(3), Self, eligibility, Self);
        }

        private void HandleLowBalance(LowBalanceEventMessage lowBalance)
        {
            var watch = Stopwatch.StartNew();
            var match = lowBalance.Offer.Percentage * lowBalance.Conversion.Amount;
            InsessionEligibility eligibility = new InsessionEligibility
            {
                CreditAmount = match,
                OfferId = lowBalance.Offer.OfferId.ToString(),
                GamingSystemId = lowBalance.Conversion.GamingSystemId,
                ProductId = lowBalance.Conversion.ProductId,
                UserId = lowBalance.Conversion.UserId
            };

            ExperienceResult experience = new ExperienceResult
            {
                Experience = "BadMarginGoodBetSize",
                MatchOffer = $"{match}"
            };

            PlayerExperienceBlockData blockData = new PlayerExperienceBlockData(experience, eligibility);

            /////////Block
            var transaction = _experienceBlockRepository.Get(INSESSION_TRANSACTION_NAME, $"{eligibility.GamingSystemId}-{eligibility.ProductId}-{eligibility.UserId}").Result;
            IBlock lastBlock = transaction.GetLastBlock();
            IBlock currentBlock = new Block(lastBlock.Index, "Experience_Calculated_v1", blockData.ToJson(), watch.Elapsed, lastBlock.Hash);
            transaction.Add(currentBlock);

            if (transaction.IsValid())
            {
                PubishInsessionEligibility(eligibility);
                PubishTransction(new TransactionMessage.Insession(transaction));
            }
            ///////////

        }

        private void PubishTransction(TransactionMessage.Insession insession)
        {
            _logger.LogInformation($"Published transactio: {insession.Transction.Name}_{insession.Transction.TransactionId}");
            _blockRef.Tell(insession);
        }
        private void HandleConversion(ConversionModel conversion)
        {
            var watch = Stopwatch.StartNew();
            conversion.TimeStamp = DateTime.Now;

            _logger.LogInformation($"Received new conversion {conversion.ToJson()}");
            UserModel user = new UserModel { UserId = conversion.UserId, GamingSystemId = conversion.GamingSystemId, ProductId = conversion.ProductId };
            DailyDealEligibilityModel eligibility = _eligibilityRepository.Get(user).Result;

            if (eligibility != null)
            {
                if (eligibility.HasOffers())
                {
                    OfferModel offer = eligibility.OfferList.FirstOrDefault(o => o.IsClaimed == false);
                    eligibility.OfferList.Remove(offer);

                    offer.IsClaimed = true;
                    offer.ClaimDateTime = DateTime.Now;
                    eligibility.OfferList.Add(offer);

                    var lowBalance = new LowBalanceEventMessage(conversion, offer);
                    
                    /////////Block
                    var transaction = _eligibilityBlockRepository.Get(INSESSION_TRANSACTION_NAME, $"{eligibility.GamingSystemId}-{eligibility.ProductId}-{eligibility.UserId}").Result;
                    IBlock lastBlock = transaction.GetLastBlock();
                    IBlock currentBlock = new Block(lastBlock.Index, "Conversion_Received_v1", lowBalance.ToJson(), watch.Elapsed, lastBlock.Hash);
                    transaction.Add(currentBlock);

                    if (transaction.IsValid())
                    {
                        _eligibilityRepository.Update(eligibility);
                        PubishTransction(new TransactionMessage.Insession(transaction));

                        Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(60), Self, lowBalance, Self);
                    }
                    else
                    {
                        _logger.LogError($"Invalid transaction for {conversion.GamingSystemId}-{conversion.ProductId}-{conversion.UserId}. Terminating...");
                    }
                    ///////////
                }
                else
                {
                    _logger.LogWarning($"User {user.ToJson()} has no offers left.");
                    /////////Block
                    var transaction = _eligibilityBlockRepository.Get(INSESSION_TRANSACTION_NAME, $"{eligibility.GamingSystemId}-{eligibility.ProductId}-{eligibility.UserId}").Result;
                    IBlock lastBlock = transaction.GetLastBlock();
                    IBlock currentBlock = new Block(lastBlock.Index, "Offers_Used_v1", "", watch.Elapsed, lastBlock.Hash);
                    transaction.Add(currentBlock);

                    if (transaction.IsValid())
                    {
                        _eligibilityRepository.Update(eligibility);
                        PubishTransction(new TransactionMessage.Insession(transaction));
                    }
                    ///////////
                    
                }
            }
            else
            {
                _logger.LogWarning($"Plyer has no eligibility:: {user.ToJson()}");
            }
        }

        protected override void PreStart()
        {
            
            InitOffers();
            base.PreStart();
        }

        private void InitOffers()
        {
            IList<DailyDealEligibilityModel> list = new List<DailyDealEligibilityModel>();

            var eligibility = new DailyDealEligibilityModel()
            {
                GamingSystemId = 9000,
                UserId = 1,
                ProductId = 9001,
                TimeStamp = DateTime.Now.Date,
                OfferList = new List<OfferModel>()
                {
                    new OfferModel
                    {
                        Percentage = 100,
                        Description = "100% up to 1000",
                        TimeStamp = DateTime.Now.Date
                    },
                    new OfferModel
                    {
                        Percentage = 50,
                        Description = "50% up to 500",
                        TimeStamp = DateTime.Now.Date
                    },
                    new OfferModel
                    {
                        Percentage = 30,
                        Description = "30% up to 300",
                        TimeStamp = DateTime.Now.Date
                    }
                }
            };

            var eligibility2 = new DailyDealEligibilityModel
            {
                GamingSystemId = 9000,
                UserId = 2,
                ProductId = 9001,
                TimeStamp = DateTime.Now.Date,
                OfferList = new List<OfferModel>()
                {
                    new OfferModel
                    {
                        Percentage = 100,
                        Description = "100% up to 1000",
                        TimeStamp = DateTime.Now.Date
                    },
                    new OfferModel
                    {
                        Percentage = 50,
                        Description = "50% up to 500",
                        TimeStamp = DateTime.Now.Date
                    }
                }
            };

            var eligibility3 = new DailyDealEligibilityModel
            {
                GamingSystemId = 9000,
                UserId = 3,
                ProductId = 9001,
                TimeStamp = DateTime.Now.Date,
                OfferList = new List<OfferModel>()
                {
                    new OfferModel
                    {
                        Percentage = 100,
                        Description = "100% up to 1000",
                        TimeStamp = DateTime.Now.Date
                    },
                    new OfferModel
                    {
                        Percentage = 60,
                        Description = "60% up to 600",
                        TimeStamp = DateTime.Now.Date
                    },
                    new OfferModel
                    {
                        Percentage = 40,
                        Description = "40% up to 400",
                        TimeStamp = DateTime.Now.Date
                    },
                    new OfferModel
                    {
                        Percentage = 20,
                        Description = "20% up to 200",
                        TimeStamp = DateTime.Now.Date
                    }
                }
            };

            list.Add(eligibility);
            list.Add(eligibility2);
            list.Add(eligibility3);

            foreach (var e in list)
            {
                Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(0), Self, e, Self);
            }
        }

        private void HandleEligibility(InsessionEligibility eligibility)
        {
            var watch = Stopwatch.StartNew();
            _logger.LogInformation($"Eligibility Received for crediting: {eligibility.ToJson()}");
            _logger.LogInformation($"Player {eligibility.GamingSystemId}-{eligibility.ProductId}-{eligibility.UserId} has been credited with {eligibility.CreditAmount} for offer {eligibility.OfferId}");

            CreditModel blockData = new CreditModel(eligibility, true);

            /////////Block
            var transaction = _creditBlockRepository.Get(INSESSION_TRANSACTION_NAME, $"{eligibility.GamingSystemId}-{eligibility.ProductId}-{eligibility.UserId}").Result;
            IBlock lastBlock = transaction.GetLastBlock();
            IBlock currentBlock = new Block(lastBlock.Index, "Player_Credited_v1", blockData.ToJson(), watch.Elapsed, lastBlock.Hash);

            transaction.Add(currentBlock);

            if (transaction.IsValid())
            {
                var conversion = new ConversionModel()
                {
                    Amount = 500,
                    GamingSystemId = eligibility.GamingSystemId,
                    ProductId = eligibility.ProductId,
                    UserId = eligibility.UserId
                };

                PubishTransction(new TransactionMessage.Insession(transaction));
                Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(60), Self, conversion, Self);
            }
            ///////////
        }
    }
}
