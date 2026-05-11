using COL_Options_Configuration.Model;
using COL_Options_Configuration.Options;
using COL_Options_Configuration.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<ApplicationOptions>(
    builder.Configuration.GetSection(nameof(ApplicationOptions))
);
builder.Services.ConfigureOptions<DatabaseOptionsSetup>();

builder.Services.AddDbContext<AppDbContext>(
    (serviceProvider, dbContextOptionsBuilder) =>
    {
        var databaseOptions = serviceProvider.GetService<IOptions<DatabaseOptions>>()!.Value;
        
        dbContextOptionsBuilder.UseSqlServer(databaseOptions.SqlConnString, sqlServerAction =>
        {
            sqlServerAction.EnableRetryOnFailure(databaseOptions.MaxRetryCount);
            sqlServerAction.CommandTimeout(databaseOptions.CommandTimeout);
        });
        dbContextOptionsBuilder.EnableDetailedErrors(databaseOptions.EnableDetailedErrors);
        dbContextOptionsBuilder.EnableSensitiveDataLogging(databaseOptions.EnableSensitiveDataLogging);
    });

var app = builder.Build();

await using var scope = app.Services.CreateAsyncScope();
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
var canConnect = await db.Database.CanConnectAsync();
app.Logger.LogInformation("Can connect to database: {CanConnect}", canConnect);

app.MapGet("/", () => "Hello World!");

app.MapGet("/company/{companyId:int}", async (int companyId, AppDbContext dbContext) =>
{
    //var company = await dbContext.company.ToListAsync();
    var company = await dbContext
        .Set<Company>()
        .AsNoTracking()
        .FirstOrDefaultAsync(c=> c.CompanyId == companyId);

    if(company == null)
    {
        return Results.NotFound($"The company with id {companyId} was not found.");
    }    
    
    return Results.Ok(company);
});

app.MapGet("/options", (IOptions<ApplicationOptions> options,
                        IOptionsSnapshot<ApplicationOptions> snapshot,
                        IOptionsMonitor<ApplicationOptions> monitor) =>
{
    //var response = $"Option1: {options.Value.Option1}, Option2: {options.Value.Option2}, Option3: {options.Value.Option3}";
    var response = new
    {
        CurrentOption1 = options.Value.Option1,
        CurrentOption2 = options.Value.Option2,
        CurrentOption3 = options.Value.Option3,
        SnapshotOption1 = snapshot.Value.Option1,
        SnapshotOption2 = snapshot.Value.Option2,
        SnapshotOption3 = snapshot.Value.Option3,
        MonitorOption1 = monitor.CurrentValue.Option1,
        MonitorOption2 = monitor.CurrentValue.Option2,
        MonitorOption3 = monitor.CurrentValue.Option3
    };
    return Results.Ok(response);
});

app.Run();
