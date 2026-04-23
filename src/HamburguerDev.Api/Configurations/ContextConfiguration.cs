using HamburguerDev.Data.Contexts;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace HamburguerDev.Api.Configurations;

public static class ContextConfiguration
{
    public static WebApplicationBuilder AddContextConfiguration(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        var resolvedConnectionString = ResolveSqliteConnectionString(connectionString, builder.Environment.ContentRootPath);

        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(resolvedConnectionString));

        return builder;
    }

    private static string ResolveSqliteConnectionString(string connectionString, string contentRootPath)
    {
        var connectionBuilder = new SqliteConnectionStringBuilder(connectionString);
        var dataSource = connectionBuilder.DataSource;

        if (string.IsNullOrWhiteSpace(dataSource) || dataSource == ":memory:")
        {
            return connectionString;
        }

        var absoluteDataSource = Path.IsPathRooted(dataSource)
            ? dataSource
            : Path.GetFullPath(Path.Combine(contentRootPath, dataSource));

        var directory = Path.GetDirectoryName(absoluteDataSource);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        connectionBuilder.DataSource = absoluteDataSource;
        return connectionBuilder.ToString();
    }
}
