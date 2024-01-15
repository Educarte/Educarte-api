//using Api.Infrastructure.Services;
//using Bogus;
//using Core;
//using Core.Enums;
//using Data;
//using Nudes.SeedMaster.Interfaces;

//namespace Api.Data.Seeds
//{
//    public class ExtraServiceSeed : IActualSeeder<ExtraService, ApiDbContext>
//    {

//        public void Seed(ApiDbContext db, ILogger logger)
//        {
//            var extra = new Faker<ExtraService>().RuleFor(p => p.Name, f => f.Commerce.ProductName())
//                                                 .RuleFor(x => x.Extras, f => f.PickRandom(db.Extras.Local, f.Random.Int(1, 5)))
//                                                 .Generate(20);
//            db.AddRange(extra);
//        }
//    }
//}
