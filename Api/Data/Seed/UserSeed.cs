using Api.Infrastructure.Services;
using Bogus;
using Core;
using Core.Enums;
using Data;
using Nudes.SeedMaster.Interfaces;

namespace Api.Data.Seed
{
    public class UserSeed : IActualSeeder<User, ApiDbContext>
    {

        public void Seed(ApiDbContext db, ILogger logger)
        {
            HashService hashService = new HashService();

            var (defaultPassword, defaultSalt) = hashService.Encrypt("asdf1234");

            var (applicationPassword, applicationSalt) = hashService.Encrypt("(qYV;L3U7XW?oizhWtGy&>$<g^7FA&u)");
            var mainAdm = new User
            {
                Name = "Administrador",
                Email = "admin@email.com.br",
                Profile = Profile.Admin,
                //Pets = new Faker<Pet>().RuleFor(x => x.Name, x => x.Name.FirstName())
                //                                                                           .RuleFor(x => x.ImageUri, x => x.Image.PicsumUrl())
                //                                                                           .RuleFor(x => x.Observation, x => x.Lorem.Paragraph())
                //                                                                           .RuleFor(x => x.Agressive, x => x.Random.Bool())
                //                                                                           .RuleFor(x => x.PetType, x => x.PickRandom<PetType>())
                //                                                                           .RuleFor(x => x.CoatType, x => x.PickRandom<CoatType>())
                //                                                                           .RuleFor(x => x.Size, x => x.PickRandom<Size>())
                //                                                                           .RuleFor(x => x.Race, x => x.PickRandom(db.Races.Local).First())
                //                                                                           .RuleFor(c => c.CreatedAt, f => f.Date.Past())
                //                                                                           .RuleFor(c => c.ModifiedAt, f => f.Date.Recent())
                //                                                                           .RuleFor(c => c.DeletedAt, f => f.Random.Bool() ? f.Date.Past() : null)
                //                                                                           .GenerateBetween(1, 2),
                PasswordSalt = defaultSalt,
                PasswordHash = defaultPassword,
            };
            var application = new User
            {
                Name = "non_logged_user",
                Email = "non_logged@email.com",
                Profile = Profile.Admin,
                PasswordSalt = applicationSalt,
                PasswordHash = applicationPassword,
            };

            //var users = new Faker<User>().RuleFor(x => x.Name, x => x.Person.FullName)
            //                             .RuleFor(x => x.Email, x => x.Internet.Email())
            //                             .RuleFor(x => x.Cellphone, x => x.Person.Phone)
            //                             .RuleFor(x => x.Profile, x => x.PickRandom<Profile>())
            //                             .RuleFor(x => x.Status, x => x.PickRandom<Status>())
            //                             .RuleFor(x => x.Adresses, x => new Faker<Adress>().RuleFor(x => x.Name, x => x.Address.StreetName())
            //                                                                               .RuleFor(x => x.Cep, x => x.Address.ZipCode())
            //                                                                               .RuleFor(x => x.Street, x => x.Address.StreetAddress())
            //                                                                               .RuleFor(x => x.Number, x => $"{x.Random.Int(10, 1000)}")
            //                                                                               .RuleFor(x => x.District, x => x.Address.Country())
            //                                                                               .RuleFor(x => x.Complement, x => x.Address.BuildingNumber())
            //                                                                               .RuleFor(x => x.Reference, x => x.Company.CompanyName())
            //                                                                               .RuleFor(c => c.CreatedAt, f => f.Date.Past())
            //                                                                               .RuleFor(c => c.ModifiedAt, f => f.Date.Recent())
            //                                                                               .RuleFor(c => c.DeletedAt, f => f.Random.Bool() ? f.Date.Past() : null)
            //                                                                               .GenerateBetween(1, 2))
            //                             .RuleFor(x => x.Pets, x => new Faker<Pet>().RuleFor(x => x.Name, x => x.Name.FirstName())
            //                                                                               .RuleFor(x => x.ImageUri, x => x.Image.PicsumUrl())
            //                                                                               .RuleFor(x => x.Observation, x => x.Lorem.Paragraph())
            //                                                                               .RuleFor(x => x.Agressive, x => x.Random.Bool())
            //                                                                               .RuleFor(x => x.PetType, x => x.PickRandom<PetType>())
            //                                                                               .RuleFor(x => x.CoatType, x => x.PickRandom<CoatType>())
            //                                                                               .RuleFor(x => x.Size, x => x.PickRandom<Size>())
            //                                                                               .RuleFor(x => x.Race, x => x.PickRandom(db.Races.Local).First())
            //                                                                               .RuleFor(c => c.CreatedAt, f => f.Date.Past())
            //                                                                               .RuleFor(c => c.ModifiedAt, f => f.Date.Recent())
            //                                                                               .RuleFor(c => c.DeletedAt, f => f.Random.Bool() ? f.Date.Past() : null)
            //                                                                               .GenerateBetween(1, 2))
            //                             .RuleFor(c => c.CreatedAt, f => f.Date.Past())
            //                             .RuleFor(c => c.ModifiedAt, f => f.Date.Recent())
            //                             .RuleFor(c => c.DeletedAt, f => f.Random.Bool() ? f.Date.Past() : null)
            //                             .Generate(100);
            db.Users.Add(mainAdm);
            db.Users.Add(application);
            //db.AddRange(users);
        }
    }
}
