using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Restia.Infrastructure.Identity.Models;
using Restia.Infrastructure.Persistence.Context;

namespace Restia.Infrastructure.Identity;
internal static class Startup
{
	/// <summary>
	/// Add identity
	/// </summary>
	/// <param name="services">The services</param>
	/// <returns>A <see cref="IServiceCollection"/>.</returns>
	internal static IServiceCollection AddIdentity(this IServiceCollection services) =>
		services
			.AddIdentity<ApplicationUser, ApplicationRole>(options =>
			{
				options.Password.RequiredLength = 6;
				options.Password.RequireDigit = false;
				options.Password.RequireLowercase = false;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireUppercase = false;
				options.User.RequireUniqueEmail = true;
			})
		/*
		 * By default, the table names for Identity are as follows:
			AspNetUsers: Stores user data like user ID, username, email, password hash, etc.
			AspNetRoles: Stores role data (e.g., admin, user, etc.).
			AspNetUserRoles: A many-to-many relationship table that links users to roles.
			AspNetUserClaims: Stores claims associated with users.
			AspNetRoleClaims: Stores claims associated with roles.
			AspNetUserLogins: Stores external login information (e.g., if users log in via Google or Facebook).
			AspNetUserTokens: Stores tokens for two-factor authentication, password reset tokens, etc.
		 */
		.AddEntityFrameworkStores<ApplicationDbContext>()
		.AddDefaultTokenProviders()
		.Services;
}
