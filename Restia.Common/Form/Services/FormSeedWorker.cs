using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Restia.Common.Shared.UnitOfWorks.Interfaces;
using FormEntity = Restia.Common.Form.Entities.Form;

namespace Restia.Common.Form.Services
{
    public sealed class FormSeedWorker : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public FormSeedWorker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var uow = scope.ServiceProvider.GetRequiredService<IAdminUnitOfWork>();

            var existing = await uow.FormRepository.FindAsync(m => m.Code == "Administrator");
            if (existing.Any())
            {
                return;
            }

            var administrator = new FormEntity
            {
                Code = "Administrator",
                Name = "Form_Administrator",
                Icon = "bi-shield-lock",
                DisplayOrder = 10,
                IsActive = true
            };

            await uow.FormRepository.AddAsync(administrator);
            await uow.SaveAsync();

            var children = new[]
            {
                new FormEntity
                {
                    Code = "User",
                    Name = "Form_User",
                    Url = "/User",
                    Icon = "bi-people",
                    ParentId = administrator.Id,
                    DisplayOrder = 1,
                    IsActive = true
                },
                new FormEntity
                {
                    Code = "Role",
                    Name = "Form_Role",
                    Url = "/Role",
                    Icon = "bi-person-badge",
                    ParentId = administrator.Id,
                    DisplayOrder = 2,
                    IsActive = true
                }
            };

            await uow.FormRepository.AddRangeAsync(children);
            await uow.SaveAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
