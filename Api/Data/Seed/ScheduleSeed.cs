//using Bogus;
//using Core;
//using Core.Enums;
//using Data;
//using Nudes.SeedMaster.Interfaces;

//namespace Api.Data.Seeds
//{
//    public class ScheduleSeed : IActualSeeder<Schedule, ApiDbContext>
//    {
//        public void Seed(ApiDbContext db, ILogger logger)
//        {
//            var f = new Faker();
//            var teste = f.PickRandom(f.PickRandom(db.Services.Local).First());
//            var teste2 = f.PickRandom(teste.ExtraServices.First().Extras);

//            var schedules = new Faker<Schedule>().RuleFor(x => x.Status, f => f.PickRandom<ScheduleStatus>())
//                                                 .RuleFor(x => x.StartDate, f => f.Date.Soon())
//                                                 .RuleFor(p => p.TotalDuration, f => TimeSpan.FromMinutes(f.Random.Int(40, 100)))
//                                                 .RuleFor(x => x.Adress, f => f.PickRandom(db.Adresses.Local).First())
//                                                 .RuleFor(x => x.Pet, f => f.PickRandom(db.Pets.Local).First())
//                                                 .RuleFor(x => x.Service, f => f.PickRandom(db.Services.Local).First())
//                                                 .RuleFor(x => x.User, f => f.PickRandom(db.Users.Local).First())
//                                                 //.RuleFor(x => x.Extras, f => f.PickRandom(db.Extras.Local, 2))
//                                                 .Generate(120);
//            db.AddRange(schedules);
//            db.SaveChanges();
//        }

//    }
//}
