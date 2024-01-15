//using Microsoft.EntityFrameworkCore;
//using Nudes.SeedMaster.Interfaces;
//using Nudes.SeedMaster.Seeder;

//namespace Api.Infrastructure.Services
//{
//    public class MigrationSeeder : EfCoreSeeder
//    {
//        private readonly IEnumerable<DbContext> contexts;
//        private readonly IHostEnvironment environment;

//        public MigrationSeeder(IEnumerable<DbContext> contexts, IEnumerable<ISeed> seeds, ILogger<EfCoreSeeder> logger, IHostEnvironment environment) : base(contexts, seeds, logger)
//        {
//            this.contexts = contexts;
//            this.environment = environment;
//        }

//        public override async Task Run()
//        {
//            if (!environment.IsStaging())
//                foreach (var context in contexts)
//                    await context.Database.MigrateAsync();

//            await base.Run();
//        }
//    }
//}
