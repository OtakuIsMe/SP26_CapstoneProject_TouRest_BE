using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TouRest.Infrastructure.Persistence
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // Load .env from solution root (one level above Infrastructure)
            var envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", ".env");
            LoadEnvFile(envPath);

            var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException(
                    "DATABASE_CONNECTION is not set in .env file. " +
                    $"Expected .env at: {Path.GetFullPath(envPath)}");

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }

        private static void LoadEnvFile(string path)
        {
            if (!File.Exists(path)) return;

            foreach (var line in File.ReadAllLines(path))
            {
                var trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith('#')) continue;

                var idx = trimmed.IndexOf('=');
                if (idx < 0) continue;

                var key = trimmed[..idx].Trim();
                var value = trimmed[(idx + 1)..].Trim().Trim('"').Trim('\'');

                Environment.SetEnvironmentVariable(key, value);
            }
        }
    }
}