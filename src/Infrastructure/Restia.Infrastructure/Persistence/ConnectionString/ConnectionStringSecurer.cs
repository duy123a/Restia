using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Npgsql;
using Restia.Application.Common.Persistence;
using Restia.Infrastructure.Common.Constants;
using Restia.Infrastructure.Persistence.Settings;

namespace Restia.Infrastructure.Persistence.ConnectionString;

public class ConnectionStringSecurer : IConnectionStringSecurer
{
	/// <summary>Hidden value default</summary>
	private const string HiddenValueDefault = "*******";
	/// <summary>The db settings</summary>
	private readonly DatabaseSettings _dbSettings;

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="dbSettings">The db settings</param>
	public ConnectionStringSecurer(IOptions<DatabaseSettings> dbSettings) =>
		_dbSettings = dbSettings.Value;

	/// <summary>
	/// Make secure
	/// </summary>
	/// <param name="connectionString">The connection string</param>
	/// <param name="dbProvider">The db provider</param>
	/// <returns></returns>
	public string? MakeSecure(string? connectionString, string? dbProvider)
	{
		if ((connectionString == null)
			|| string.IsNullOrEmpty(connectionString))
		{
			return connectionString;
		}

		if (string.IsNullOrWhiteSpace(dbProvider))
		{
			dbProvider = _dbSettings.DBProvider;
		}

		return dbProvider?.ToLower() switch
		{
			DbProviderKeys.Npgsql => MakeSecureNpgsqlConnectionString(connectionString),
			DbProviderKeys.SqlServer => MakeSecureSqlConnectionString(connectionString),
			_ => connectionString
		};
	}

	/// <summary>
	/// Make secure Sql connection string
	/// </summary>
	/// <param name="connectionString">The connection string</param>
	/// <returns>Sql connection string</returns>
	private static string MakeSecureSqlConnectionString(string connectionString)
	{
		var builder = new SqlConnectionStringBuilder(connectionString);

		if (!string.IsNullOrEmpty(builder.Password) && !builder.IntegratedSecurity)
		{
			builder.Password = HiddenValueDefault;
		}

		if (!string.IsNullOrEmpty(builder.UserID) && !builder.IntegratedSecurity)
		{
			builder.UserID = HiddenValueDefault;
		}

		return builder.ToString();
	}

	/// <summary>
	/// Make secure Npgsql connection string
	/// </summary>
	/// <param name="connectionString">The connection string</param>
	/// <returns>Npgsql connection string</returns>
	private static string MakeSecureNpgsqlConnectionString(string connectionString)
	{
		var builder = new NpgsqlConnectionStringBuilder(connectionString);

		if (!string.IsNullOrEmpty(builder.Password))
		{
			builder.Password = HiddenValueDefault;
		}

		if (!string.IsNullOrEmpty(builder.Username))
		{
			builder.Username = HiddenValueDefault;
		}

		return builder.ToString();
	}
}
