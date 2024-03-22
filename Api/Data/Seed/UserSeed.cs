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
            var childs = new Faker<Student>()
                            .RuleFor(x => x.Name, x => x.Person.FullName)
                            .RuleFor(x => x.RegistrationNumber, x => x.Random.Int(10000000, 99999999).ToString())
                            .RuleFor(x => x.Classroom, x => x.PickRandom(db.Classrooms.Local).First())
                            .RuleFor(x => x.ContractedHours, x => new Faker<ContractedHour>()
                                .RuleFor(x => x.Hours, x => x.Random.Int(1, 10))
                                .RuleFor(x => x.StartDate, x => x.Date.Recent())
                                .RuleFor(x => x.StartDate, x => x.Date.Soon())
                                .GenerateBetween(1, 2).ToList())
                            .RuleFor(x => x.EmergencyContacts, x => new Faker<EmergencyContact>()
                                .RuleFor(x => x.Name, x => x.Person.FullName)
                                .RuleFor(x => x.Telephone, x => x.Phone.PhoneNumber())
                                .GenerateBetween(1, 2).ToList())
                            .RuleFor(x => x.Diaries, x => new Faker<Diary>()
                                .RuleFor(y => y.Name, y => y.Random.Word())
                                .RuleFor(x => x.Description, x => x.Random.Words())
                                .RuleFor(y => y.FileUri, y => y.Image.PicsumUrl())
                                .RuleFor(y => y.Time, y => y.Date.Past())
                                .GenerateBetween(1, 10).ToList())
                            .RuleFor(x => x.AccessControls, x => new Faker<AccessControl>()
                                .RuleFor(x => x.AccessControlType, x => x.PickRandom<AccessControlType>())
                                .RuleFor(x => x.Time, x => x.Date.Recent())
                                .GenerateBetween(1, 10).ToList())
                            .RuleFor(c => c.BirthDate, f => f.Date.Past(10))
                            .RuleFor(c => c.CreatedAt, f => f.Date.Past())
                            .RuleFor(c => c.ModifiedAt, f => f.Date.Recent())
                            .RuleFor(c => c.DeletedAt, f => f.Random.Bool() ? f.Date.Past() : null)
                            .Generate(2);
            
            var father = new User
            {
                Name = "Pai",
                Email = "pai@email.com",
                Profile = Profile.LegalGuardian,
                PasswordSalt = defaultSalt,
                PasswordHash = defaultPassword,
                Childs = childs,
                LegalGuardianType = "Responsável Legal",
                Cellphone = "11940028922"
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
                                                                            .RuleFor(y => y.MaxStudents, y => x.Random.Int(1, 15))
                                                                            .RuleFor(y => y.Diaries, y => new Faker<Diary>()
                                                                                .RuleFor(y => y.Name, y => y.Random.Word())
                                                                                .RuleFor(y => y.Description, y => y.Random.Words())
                                                                                .RuleFor(y => y.FileUri, y => y.Image.PicsumUrl())
                                                                                .RuleFor(y => y.Time, y => y.Date.Past())
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
                                         .RuleFor(x => x.Profession, x => x.Company.CompanyName())
                                         .RuleFor(x => x.Workplace, x => x.Address.City())
                                         .RuleFor(x => x.Profile, x => Profile.LegalGuardian)
                                         .RuleFor(x => x.Status, x => x.PickRandom<Status>())
                                         .RuleFor(x => x.LegalGuardianType, x => "Responsável Legal")
                                         .RuleFor(x => x.Childs, x => x.PickRandom(db.Students.Local, 2).ToList())
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
            db.Users.Add(father);
            db.AddRange(usersTeachers);
            db.AddRange(usersParents);
        }
    }
}
