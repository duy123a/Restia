using Microsoft.EntityFrameworkCore;
using Restia.Common.Data.Context;
using Restia.Common.Form.Repository.Interfaces;
using Restia.Common.Shared.Repository;
using FormEntity = Restia.Common.Form.Entities.Form;

namespace Restia.Common.Form.Repository
{
    public class FormRepository : BaseRepository<FormEntity>, IFormRepository
    {
        public FormRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<FormEntity>> GetActiveRootMenusWithChildrenAsync()
        {
            return await DbSet
                .AsNoTracking()
                .Where(m => m.ParentId == null && m.IsActive)
                .Include(m => m.Children.Where(c => c.IsActive).OrderBy(c => c.DisplayOrder))
                .OrderBy(m => m.DisplayOrder)
                .ToListAsync();
        }
    }
}
