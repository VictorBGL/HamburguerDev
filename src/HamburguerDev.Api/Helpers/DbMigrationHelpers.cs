using HamburguerDev.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace HamburguerDev.Api.Helpers;

public static class DbMigrationHelperExtension
{
    public static void UseDbMigrationHelper(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
    }
}
