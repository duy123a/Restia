using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Restia.Application.Common.Interfaces;

namespace Restia.Infrastructure.Common.Services;

/// <summary>
/// Newton Soft Service
/// </summary>
public class NewtonSoftService : ISerializerService
{
	/// <summary>
	/// Deserialize text to object T
	/// </summary>
	/// <typeparam name="T">Type of object</typeparam>
	/// <param name="text">The text</param>
	/// <returns>Object as T</returns>
	public T Deserialize<T>(string text)
	{
		return JsonConvert.DeserializeObject<T>(text)!;
	}

	/// <summary>
	/// Serialize object T to json string
	/// </summary>
	/// <typeparam name="T">Type of object</typeparam>
	/// <param name="obj">The object T</param>
	/// <returns>A json string</returns>
	public string Serialize<T>(T obj)
	{
		return JsonConvert.SerializeObject(
			obj,
			new JsonSerializerSettings
			{
				// Will ensure that property names in the serialized JSON are in camelCase rather than the default PascalCase (the usual convention in C#).
				ContractResolver = new CamelCasePropertyNamesContractResolver(),
				// Null value in object is not be included in the serialized JSON
				NullValueHandling = NullValueHandling.Ignore,
				Converters =
				[
					// Will ensure that convert enum to enum name instead of index
					new StringEnumConverter(new CamelCaseNamingStrategy())
				]
			});
	}

	/// <summary>
	/// Serialize object T to json string
	/// </summary>
	/// <typeparam name="T">Type of object</typeparam>
	/// <param name="obj">The object T</param>
	/// <param name="type">The type to serialize</param>
	/// <returns>A json string</returns>
	public string Serialize<T>(T obj, Type type)
	{
		return JsonConvert.SerializeObject(obj, type, new());
	}
}
