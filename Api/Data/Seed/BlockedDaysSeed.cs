//using Bogus;
//using Core;
//using Data;
//using Nudes.SeedMaster.Interfaces;

//namespace Api.Data.Seeds
//{
//    public class BlockedDaysSeed : IActualSeeder<BlockedDay, ApiDbContext>
//    {
//        public void Seed(ApiDbContext db, ILogger logger)
//        {

//            var blockedDays = new Faker<BlockedDay>().RuleFor(x => x.Date, f => f.Date.Between(DateTime.Now.AddDays(-30), DateTime.Now.AddDays(60)))
//                                                 .Generate(100);
//            db.AddRange(blockedDays);
//            db.SaveChanges();
//        }
//    }
//}
