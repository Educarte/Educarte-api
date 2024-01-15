//using Bogus;
//using Core;
//using Core.Enums;
//using Data;
//using Nudes.SeedMaster.Interfaces;

//namespace Api.Data.Seeds
//{
//    public class RaceSeed : IActualSeeder<Race, ApiDbContext>
//    {
//        public void Seed(ApiDbContext db, ILogger logger)
//        {

//            var races = new Faker<Race>().RuleFor(x => x.PetType, x => x.PickRandom<PetType>())
//                                         .RuleFor(x => x.Name, f => f.Name.LastName())
//                                                 .Generate(70);
//            db.AddRange(races);
//            db.SaveChanges();
//        }

//    }
//}
