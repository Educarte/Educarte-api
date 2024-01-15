//using Api.Infrastructure.Services;
//using Bogus;
//using Core;
//using Core.Enums;
//using Data;
//using Nudes.SeedMaster.Interfaces;

//namespace Api.Data.Seeds
//{
//    public class ExtraSeed : IActualSeeder<Extra, ApiDbContext>
//    {

//        public void Seed(ApiDbContext db, ILogger logger)
//        {
//            var extra = new Faker<Extra>().RuleFor(x => x.Name, x => x.Commerce.ProductName())
//                                          .RuleFor(x => x.Price, f => Math.Round(f.Random.Decimal(10, 50), 2))
//                                          .RuleFor(x => x.Duration, x => TimeSpan.FromMinutes(x.Random.Int(5, 120)))
//                                          .Generate(50);
//            db.AddRange(extra);
//        }
//    }
//}
