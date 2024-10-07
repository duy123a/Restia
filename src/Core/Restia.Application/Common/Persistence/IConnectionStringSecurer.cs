namespace Restia.Application.Common.Persistence;

/// <summary>
/// Connection String Securer interface
/// </summary>
public interface IConnectionStringSecurer
{
	/// <summary>
	/// Make secure
	/// </summary>
	/// <param name="connectionString">A connection string</param>
	/// <param name="dbProvider">A DB provider</param>
	/// <returns>A connection string</returns>
	string? MakeSecure(string? connectionString, string? dbProvider = null);
}
