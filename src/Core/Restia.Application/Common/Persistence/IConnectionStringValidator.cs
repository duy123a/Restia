namespace Restia.Application.Common.Persistence;

public interface IConnectionStringValidator
{
	/// <summary>
	/// Try validate a connection string
	/// </summary>
	/// <param name="connectionString">A connection string</param>
	/// <param name="dbProvider">A DB provider</param>
	/// <returns>A connection string</returns>
	bool TryValidate(string connectionString, string? dbProvider = null);
}
