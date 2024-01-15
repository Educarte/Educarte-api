//using Bogus;
//using Core;
//using Core.Enums;
//using Data;
//using Nudes.SeedMaster.Interfaces;

//namespace Api.Data.Seeds
//{
//    public class ServiceSeed : IActualSeeder<Service, ApiDbContext>
//    {
//        public void Seed(ApiDbContext db, ILogger logger)
//        {
//            var services = new Faker<Service>()
//                        .RuleFor(p => p.Name, f => f.Commerce.ProductName())
//                        .RuleFor(p => p.ImageUri, f => f.Image.PicsumUrl())
//                        .RuleFor(p => p.Status, f => f.PickRandom<Status>())
//                        .RuleFor(p => p.DefaultDuration, f => TimeSpan.FromMinutes(f.Random.Int(40, 100)))
//                        .RuleFor(p => p.ShortCoat, f => TimeSpan.FromMinutes(f.Random.Int(5, 20)))
//                        .RuleFor(p => p.MediumCoat, f => TimeSpan.FromMinutes(f.Random.Int(15, 30)))
//                        .RuleFor(p => p.LongCoat, f => TimeSpan.FromMinutes(f.Random.Int(25, 40)))
//                        .RuleFor(p => p.SmallSize, f => TimeSpan.FromMinutes(f.Random.Int(1, 5)))
//                        .RuleFor(p => p.MediumSize, f => TimeSpan.FromMinutes(f.Random.Int(5, 25)))
//                        .RuleFor(p => p.LargeSize, f => TimeSpan.FromMinutes(f.Random.Int(30, 40)))
//                        .RuleFor(p => p.ShortCoatPrice, f => Math.Round(f.Random.Decimal(10, 50), 2))
//                        .RuleFor(p => p.MediumCoatPrice, f => Math.Round(f.Random.Decimal(20, 100), 2))
//                        .RuleFor(p => p.LongCoatPrice, f => Math.Round(f.Random.Decimal(30, 150), 2))
//                        .RuleFor(p => p.SmallSizePrice, f => Math.Round(f.Random.Decimal(5, 30), 2))
//                        .RuleFor(p => p.MediumSizePrice, f => Math.Round(f.Random.Decimal(10, 50), 2))
//                        .RuleFor(p => p.LargeSizePrice, f => Math.Round(f.Random.Decimal(20, 100), 2))
//                        .RuleFor(p => p.ExtraServices, f => new Faker<ExtraService>().RuleFor(p => p.Name, f => f.Commerce.ProductName())
//                                                                                     .RuleFor(p => p.Extras, f => new Faker<Extra>().RuleFor(x => x.Name, x => x.Commerce.ProductName())
//                                                                                                                                    .RuleFor(x => x.Price, x => Math.Round(f.Random.Decimal(10, 50), 2))
//                                                                                                                                    .RuleFor(x => x.Duration, x => TimeSpan.FromMinutes(x.Random.Int(5, 120)))
//                                                                                                                                    //.RuleFor(x => x.Schedules, x => x.PickRandom(db.Schedules.Local, 10))
//                                                                                                                                    .GenerateBetween(1, 5))
//                                                                                     .GenerateBetween(1, 3))
//                        .Generate(3);
//            db.AddRange(services);
//        }

//    }
//}
