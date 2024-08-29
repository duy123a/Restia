using Microsoft.EntityFrameworkCore;

namespace Restia.Infrastructure.Persistence.Context;
public class ApplicationDbContext : DbContext
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
	{

	}
}
