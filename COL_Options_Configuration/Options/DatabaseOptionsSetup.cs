using System;
using Microsoft.Extensions.Options;

namespace COL_Options_Configuration.Options;

public class DatabaseOptionsSetup : IConfigureOptions<DatabaseOptions>
{
    private readonly string _configurationSectionName = "DatabaseOptions";
    private readonly IConfiguration _configuration;

    public DatabaseOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(DatabaseOptions options)
    {
        var SqlConnString = _configuration.GetConnectionString("SqlConnString");
        options.SqlConnString = SqlConnString;
        _configuration.GetSection(_configurationSectionName).Bind(options);
    }
}
