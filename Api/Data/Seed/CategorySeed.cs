using Core;
using Data;
using Nudes.SeedMaster.Interfaces;

namespace Api.Data.Seed
{
    public class CategorySeed : IActualSeeder<Student, ApiDbContext>
    {
        public void Seed(ApiDbContext db, ILogger logger)
        {
            var categories = new List<Student>() {
                new Student{
                    Name = "Gato",
                },
                new Student{
                    Name = "Cachorro",
                }
            };
            db.AddRange(categories);
        }

    }
}
