using HamburguerDev.Api.Configurations;
using HamburguerDev.Api.Helpers;
using HamburguerDev.Shared.CommonConfigurations;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddSharedConfiguration()
    .AddIdentityConfiguration()
    .AddApiConfiguration()
    .AddSwaggerConfiguration()
    .AddContextConfiguration()
    .ResolveDependencies();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("Total");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDbMigrationHelper();
app.Run();
