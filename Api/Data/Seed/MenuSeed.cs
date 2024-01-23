using Bogus;
using Core;
using Core.Enums;
using Data;
using Nudes.SeedMaster.Interfaces;

namespace Api.Data.Seed
{
    public class MenuSeed : IActualSeeder<Menu, ApiDbContext>
    {
        public void Seed(ApiDbContext db, ILogger logger)
        {
            var menu = new Faker<Menu>()
                            .RuleFor(x => x.Name, x => x.Commerce.ProductName())
                            .RuleFor(x => x.Uri, x => x.Image.PicsumUrl())
                            .RuleFor(x => x.StartDate, x => x.Date.Recent())
                            .RuleFor(x => x.ValidUntil, x => x.Date.Soon())
                            .Generate(2).ToList();

            db.AddRange(menu);
        }

    }
}
