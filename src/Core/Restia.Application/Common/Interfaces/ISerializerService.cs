namespace Restia.Application.Common.Interfaces;

public interface ISerializerService : ITransientService
{
	/// <summary>
	/// Deserialize a string into an object
	/// </summary>
	/// <typeparam name="T">Type of object</typeparam>
	/// <param name="text">A string (text)</param>
	/// <returns>A type of object</returns>
	T Deserialize<T>(string text);

	/// <summary>
	/// Serialize an object to a string
	/// </summary>
	/// <typeparam name="T">Type of object</typeparam>
	/// <param name="obj">A object</param>
	/// <returns>A string</returns>
	string Serialize<T>(T obj);

	/// <summary>
	/// Serialize an object to a string
	/// </summary>
	/// <typeparam name="T">Type of object</typeparam>
	/// <param name="obj">A object</param>
	/// <param name="type">The type</param>
	/// <returns>A string</returns>
	string Serialize<T>(T obj, Type type);
}
