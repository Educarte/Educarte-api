//using Bogus;
//using Core;
//using Core.Enums;
//using Data;
//using Nudes.SeedMaster.Interfaces;

//namespace Api.Data.Seeds
//{
//    public class PurchaseSeed : IActualSeeder<Purchase, ApiDbContext>
//    {
//        public void Seed(ApiDbContext db, ILogger logger)
//        {
//            var purchases = new Faker<Purchase>().RuleFor(x => x.Code, Guid.NewGuid().ToString("n").Substring(0, 6))
//                                                 .RuleFor(x => x.Status, f => f.PickRandom<PurchaseStatus>())
//                                                 .RuleFor(p => p.Total, f => f.Random.Decimal(1, 3000))
//                                                 .RuleFor(x => x.User, f => f.PickRandom(db.Users.Local, 3).First())
//                                                 .RuleFor(p => p.PurchaseItems, f => new Faker<PurchaseItem>().RuleFor(x => x.Quantity, f => f.Random.Int(1, 5))
//                                                                                                              .RuleFor(p => p.Variation, f => f.PickRandom(db.Variations.Local, 3).First())
//                                                                                                              .GenerateBetween(1, 5))
//                                                 .Generate(120);
//            db.AddRange(purchases);
//        }
//    }
//}
