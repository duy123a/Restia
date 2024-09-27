using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Restia.Application.Common.Exceptions;
using Restia.Infrastructure.Auth.Jwt.Settings;

namespace Restia.Infrastructure.Auth.Jwt.Configure;
public class ConfigureJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions>
{
	/// <summary>Jwt settings</summary>
	private readonly JwtSettings _jwtSettings;

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="jwtSettings">Jwt settings</param>
	public ConfigureJwtBearerOptions(IOptions<JwtSettings> jwtSettings) =>
		_jwtSettings = jwtSettings.Value;

	/// <summary>
	/// Configure
	/// </summary>
	/// <param name="options">The options</param>
	public void Configure(JwtBearerOptions options) => Configure(string.Empty, options);

	/// <summary>
	/// Configure
	/// </summary>
	/// <param name="name">The name</param>
	/// <param name="options">The options</param>
	/// <exception cref="UnauthorizedException">A UnauthorizedException</exception>
	/// <exception cref="ForbiddenException">A ForbiddenException</exception>
	public void Configure(string? name, JwtBearerOptions options)
	{
		if (name != JwtBearerDefaults.AuthenticationScheme)
		{
			return;
		}

		// The key should have 32 characters
		var keys = Encoding.UTF8.GetBytes(_jwtSettings.Key);

		// Allow http request (not https)
		options.RequireHttpsMetadata = false;
		options.SaveToken = true;
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(keys),
			ValidateIssuer = false,
			ValidateLifetime = true,
			ValidateAudience = false,
			// Specifies the claim type that will be used to get the user's roles from the JWT.
			// In this case, it’s set to ClaimTypes.Role, which means the system will look for claims of type "role" in the JWT to determine the user's roles.
			RoleClaimType = ClaimTypes.Role,
			// Delay time from server time to end user time
			ClockSkew = TimeSpan.Zero,
		};
		options.Events = new JwtBearerEvents
		{
			OnChallenge = context =>
			{
				context.HandleResponse();
				if (!context.Response.HasStarted)
				{
					throw new UnauthorizedException("Authentication Failed.");
				}

				return Task.CompletedTask;
			},
			OnForbidden = _ => throw new ForbiddenException("You are not authorized to access this resource."),
			OnMessageReceived = context =>
			{
				var accessToken = context.Request.Query["access_token"];

				if (!string.IsNullOrEmpty(accessToken)
					&& context.HttpContext.Request.Path.StartsWithSegments("/notifications"))
				{
					// Read the token out of the query string
					context.Token = accessToken;
				}

				return Task.CompletedTask;
			}
		};
	}
}
