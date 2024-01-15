//using Bogus;
//using Core;
//using Core.Enums;
//using Data;
//using Nudes.SeedMaster.Interfaces;

//namespace Api.Data.Seeds
//{
//    public class ProductSeed : IActualSeeder<Product, ApiDbContext>
//    {
//        public void Seed(ApiDbContext db, ILogger logger)
//        {
//            var products = new Faker<Product>()
//                                    .RuleFor(p => p.Name, f => f.Commerce.ProductName())
//                                    .RuleFor(p => p.Description, f => f.Lorem.Sentence())
//                                    .RuleFor(p => p.ImageUri, f => f.Image.PicsumUrl())
//                                    .RuleFor(p => p.Type, f => f.PickRandom<ProductType>())
//                                    .RuleFor(p => p.Status, f => f.PickRandom<Status>())
//                                    .RuleFor(p => p.Categories, f => f.PickRandom(db.Categories.Local).ToList())
//                                    .RuleFor(p => p.Variations, f => new Faker<Variation>().RuleFor(x => x.Size, x => $"{x.Random.Int(100, 250)}")
//                                                                                           .RuleFor(p => p.Price, f => Math.Round(f.Random.Decimal(10, 50), 2))
//                                                                                           .GenerateBetween(1, 5).ToList())
//                                    .RuleFor(c => c.CreatedAt, f => f.Date.Past())
//                                    .RuleFor(c => c.ModifiedAt, f => f.Date.Recent())
//                                    .RuleFor(c => c.DeletedAt, f => f.Random.Bool() ? f.Date.Past() : null)
//                                    .Generate(50);
//            db.AddRange(products);
//            db.SaveChanges();
//        }
//    }
//}
