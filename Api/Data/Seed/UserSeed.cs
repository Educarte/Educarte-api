using Api.Infrastructure.Services;
using Bogus;
using Bogus.DataSets;
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

            var (defaultPassword, defaultSalt) = hashService.Encrypt("Asdf1234");

            var mainAdm = new User
            {
                Name = "Administrador",
                Email = "admin@email.com",
                Profile = Profile.Admin,
                PasswordSalt = defaultSalt,
                PasswordHash = defaultPassword,
            };

            var usersTeachers = new Faker<User>().RuleFor(x => x.Name, x => x.Person.FullName)
                                         .RuleFor(x => x.Email, x => x.Internet.Email())
                                         .RuleFor(x => x.Cellphone, x => x.Person.Phone)
                                         .RuleFor(x => x.Profile, x => Profile.Teacher)
                                         .RuleFor(x => x.Status, x => x.PickRandom<Status>())
                                         .RuleFor(x => x.PasswordHash, x => defaultPassword)
                                         .RuleFor(x => x.PasswordSalt, x => defaultSalt)
                                         .RuleFor(x => x.Classrooms, x => new Faker<Classroom>()
                                                                            .RuleFor(y => y.Name, y => y.Name.JobTitle())
                                                                            .RuleFor(y => y.MaxStudents, y => x.Random.Int())
                                                                            .RuleFor(y => y.Diaries, y => new Faker<Diary>()
                                                                                .RuleFor(y => y.Description, y => y.Random.Words())
                                                                                .GenerateBetween(1, 10).ToList())
                                                                            .RuleFor(c => c.CreatedAt, f => f.Date.Past())
                                                                            .RuleFor(c => c.ModifiedAt, f => f.Date.Recent())
                                                                            .GenerateBetween(1, 3).ToList())
                                         .RuleFor(c => c.CreatedAt, f => f.Date.Past())
                                         .RuleFor(c => c.ModifiedAt, f => f.Date.Recent())
                                         .RuleFor(c => c.DeletedAt, f => f.Random.Bool() ? f.Date.Past() : null)
                                         .Generate(15).ToList();

            var usersParents = new Faker<User>().RuleFor(x => x.Name, x => x.Person.FullName)
                                         .RuleFor(x => x.Email, x => x.Internet.Email())
                                         .RuleFor(x => x.Cellphone, x => x.Person.Phone)
                                         .RuleFor(x => x.Profile, x => Profile.LegalGuardian)
                                         .RuleFor(x => x.Status, x => x.PickRandom<Status>())
                                         .RuleFor(x => x.PasswordHash, x => defaultPassword)
                                         .RuleFor(x => x.PasswordSalt, x => defaultSalt)
                                         .RuleFor(x => x.Address, x => new Faker<Core.Address>().RuleFor(x => x.Name, x => x.Address.StreetName())
                                                                            .RuleFor(x => x.Cep, x => x.Address.ZipCode())
                                                                            .RuleFor(x => x.Street, x => x.Address.StreetAddress())
                                                                            .RuleFor(x => x.Number, x => $"{x.Random.Int(10, 1000)}")
                                                                            .RuleFor(x => x.District, x => x.Address.Country())
                                                                            .RuleFor(x => x.Complement, x => x.Address.BuildingNumber())
                                                                            .RuleFor(x => x.Reference, x => x.Company.CompanyName())
                                                                            .RuleFor(c => c.CreatedAt, f => f.Date.Past())
                                                                            .RuleFor(c => c.ModifiedAt, f => f.Date.Recent()))
                                         .RuleFor(c => c.CreatedAt, f => f.Date.Past())
                                         .RuleFor(c => c.ModifiedAt, f => f.Date.Recent())
                                         .RuleFor(c => c.DeletedAt, f => f.Random.Bool() ? f.Date.Past() : null)
                                         .Generate(25).ToList();

            db.Users.Add(mainAdm);
            db.AddRange(usersTeachers);
            db.AddRange(usersParents);
        }
    }
}
