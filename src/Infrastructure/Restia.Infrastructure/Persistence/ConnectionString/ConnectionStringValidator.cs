using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using Restia.Application.Common.Persistence;
using Restia.Infrastructure.Common.Constants;
using Restia.Infrastructure.Persistence.Settings;

namespace Restia.Infrastructure.Persistence.ConnectionString;

internal class ConnectionStringValidator : IConnectionStringValidator
{
	/// <summary>The db settings</summary>
	private readonly DatabaseSettings _dbSettings;
	/// <summary>The logger</summary>
	private readonly ILogger<ConnectionStringValidator> _logger;

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="dbSettings">The db settings</param>
	/// <param name="logger">The logger</param>
	public ConnectionStringValidator(
		IOptions<DatabaseSettings> dbSettings,
		ILogger<ConnectionStringValidator> logger)
	{
		_dbSettings = dbSettings.Value;
		_logger = logger;
	}

	/// <summary>
	/// Try validate for connection string
	/// </summary>
	/// <param name="connectionString">The connection string</param>
	/// <param name="dbProvider">The db provider</param>
	/// <returns>True: the connection is valid</returns>
	public bool TryValidate(string connectionString, string? dbProvider = null)
	{
		if (string.IsNullOrWhiteSpace(dbProvider))
		{
			dbProvider = _dbSettings.DBProvider;
		}

		try
		{
			switch (dbProvider?.ToLowerInvariant())
			{
				case DbProviderKeys.Npgsql:
					var postgresqlcs = new NpgsqlConnectionStringBuilder(connectionString);
					break;

				case DbProviderKeys.SqlServer:
					var mssqlcs = new SqlConnectionStringBuilder(connectionString);
					break;
			}

			return true;
		}
		catch (Exception ex)
		{
			_logger.LogError($"Connection String Validation Exception : {ex.Message}");
			return false;
		}
	}
}
