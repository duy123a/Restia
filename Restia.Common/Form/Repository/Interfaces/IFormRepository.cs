using Restia.Common.Shared.Repository.Interfaces;
using FormEntity = Restia.Common.Form.Entities.Form;

namespace Restia.Common.Form.Repository.Interfaces
{
    public interface IFormRepository : IRepository<FormEntity>
    {
        Task<List<FormEntity>> GetActiveRootMenusWithChildrenAsync();
    }
}
