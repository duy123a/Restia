namespace Restia.Infrastructure.Caching.Settings;

public class CacheSettings
{
	/// <summary>Get and set use distributed cache</summary>
	public bool UseDistributedCache { get; set; }
	/// <summary>Get and set prefer Redis</summary>
	public bool PreferRedis { get; set; }
	/// <summary>Get and set Redis URL</summary>
	public string? RedisURL { get; set; }
}
