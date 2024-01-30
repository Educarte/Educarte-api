using Api.Infrastructure.Services;
using Bogus;
using Bogus.DataSets;
using Core;
using Core.Enums;
using Data;
using Nudes.SeedMaster.Interfaces;

namespace Api.Data.Seed
{
    public class ClassroomSeed : IActualSeeder<Classroom, ApiDbContext>
    {

        public void Seed(ApiDbContext db, ILogger logger)
        {
            var classrooms = new Faker<Classroom>()
                                        .RuleFor(y => y.Teachers, y => y.PickRandom(db.Users.Local).Where(x => x.Profile == Profile.Teacher).Take(2).ToList())
                                        .RuleFor(y => y.Name, y => y.Name.JobTitle())
                                        .RuleFor(y => y.MaxStudents, y => y.Random.Int(1,15))
                                        .RuleFor(y => y.Diaries, y => new Faker<Diary>()
                                            .RuleFor(y => y.Description, y => y.Random.Words())
                                            .RuleFor(y => y.FileUri, y => y.Image.PicsumUrl())
                                            .RuleFor(y => y.Time, y => y.Date.Past())
                                            .GenerateBetween(1, 10).ToList())
                                        .RuleFor(c => c.CreatedAt, f => f.Date.Past())
                                        .RuleFor(c => c.ModifiedAt, f => f.Date.Recent())
                                        .Generate(10).ToList();

            db.AddRange(classrooms);
        }
    }
}
