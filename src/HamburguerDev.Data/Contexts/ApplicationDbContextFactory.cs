using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HamburguerDev.Data.Contexts;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var currentDirectory = Directory.GetCurrentDirectory();

        var candidatePaths = new[]
        {
            Path.Combine(currentDirectory, "src", "HamburguerDev.Shared", "CommonConfigurations", "sharedsettings.json"),
            Path.Combine(currentDirectory, "..", "HamburguerDev.Shared", "CommonConfigurations", "sharedsettings.json"),
            Path.Combine(currentDirectory, "..", "src", "HamburguerDev.Shared", "CommonConfigurations", "sharedsettings.json")
        };

        var settingsFile = candidatePaths.FirstOrDefault(File.Exists)
            ?? throw new FileNotFoundException("sharedsettings.json não foi encontrado para criação da migração.");

        var configuration = new ConfigurationBuilder()
            .AddJsonFile(settingsFile, optional: false)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        optionsBuilder.UseSqlite(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
