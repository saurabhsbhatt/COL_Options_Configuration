using COL_Options_Configuration.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(
    dbContextOptionsBuilder =>
    {
        var SqlConnString = builder.Configuration.GetConnectionString("SqlConnString");
        dbContextOptionsBuilder.UseSqlServer(SqlConnString);
    });

var app = builder.Build();

await using var scope = app.Services.CreateAsyncScope();
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
var canConnect = await db.Database.CanConnectAsync();
app.Logger.LogInformation("Can connect to database: {CanConnect}", canConnect);

app.MapGet("/", () => "Hello World!");

// app.MapGet("/companies", async (AppDbContext dbContext) =>
// {
//     var companies = await dbContext.Companies.ToListAsync();
//     return Results.Ok(companies);
// });

app.Run();
