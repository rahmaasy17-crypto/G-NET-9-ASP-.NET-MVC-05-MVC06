using GymManagement.DAL.Data.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.PL
{
    public static  class ProgramExtension
    {
        public static async Task MigrateAndSeedDataAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<GymDbContext>();

            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            var pendingMigration = await dbContext.Database.GetPendingMigrationsAsync();

            if (pendingMigration.Any())
            {
                await dbContext.Database.MigrateAsync(); 
            }

          
            var seedFolderPath = Path.Combine(
                app.Environment.ContentRootPath,
                "wwwroot",
                "Files"
            );

            await GymDataSeeding.SeedAsync(dbContext, seedFolderPath, logger);
        }
    }
}

