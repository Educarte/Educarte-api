using Bogus;
using Core;
using Core.Enums;
using Data;
using Nudes.SeedMaster.Interfaces;

namespace Api.Data.Seed
{
    public class StudentSeed : IActualSeeder<Student, ApiDbContext>
    {
        public void Seed(ApiDbContext db, ILogger logger)
        {
            var students = new Faker<Student>()
                                            .RuleFor(x => x.Name, x => x.Person.FullName)
                                            .RuleFor(x => x.LegalGuardians, x => x.PickRandom(db.Users.Local).Where(x => x.Profile == Profile.LegalGuardian).ToList())
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
                                                .RuleFor(x => x.Description, x => x.Random.Words())
                                                .GenerateBetween(1, 10).ToList())
                                            .RuleFor(x => x.AccessControls, x => new Faker<AccessControl>()
                                                .RuleFor(x => x.AccessControlType, x => x.PickRandom<AccessControlType>())
                                                .RuleFor(x => x.Time, x => x.Date.Recent())
                                                .GenerateBetween(1, 10).ToList())
                                            .RuleFor(c => c.CreatedAt, f => f.Date.Past())
                                            .RuleFor(c => c.ModifiedAt, f => f.Date.Recent())
                                            .RuleFor(c => c.DeletedAt, f => f.Random.Bool() ? f.Date.Past() : null)
                                            .Generate(25).ToList();
            db.AddRange(students);
        }

    }
}
