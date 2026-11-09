using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Restia.Common.Shared.UnitOfWorks.Interfaces;
using FormCodes = Restia.Common.Permission.Constants.PermissionConstants.FormCodes;
using FormEntity = Restia.Common.Form.Entities.Form;

namespace Restia.Common.Form.Services
{
    public sealed class FormSeedWorker : IHostedService
    {
        private sealed record FormSeed(
            string Code,
            string? ParentCode,
            string? Url,
            string Icon,
            int DisplayOrder);

        // Parents must be listed before their children.
        private static readonly FormSeed[] Seeds =
        [
            new(FormCodes.Administrator, null, null, "bi-shield-lock", 10),
            new(FormCodes.User, FormCodes.Administrator, "/User", "bi-people", 1),
            new(FormCodes.Role, FormCodes.Administrator, "/Role", "bi-person-badge", 2),
            new(FormCodes.Permission, FormCodes.Administrator, "/Permission", "bi-shield-check", 3),
            new(FormCodes.Tenant, FormCodes.Administrator, "/Tenant", "bi-building", 4)
        ];

        private readonly IServiceProvider _serviceProvider;

        public FormSeedWorker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var uow = scope.ServiceProvider.GetRequiredService<IAdminUnitOfWork>();
            await SeedFormsAsync(uow, cancellationToken);
        }

        public static async Task SeedFormsAsync(IAdminUnitOfWork uow, CancellationToken cancellationToken = default)
        {
            var existing = (await uow.FormRepository.GetAllAsync(cancellationToken))
                .ToDictionary(f => f.Code, StringComparer.OrdinalIgnoreCase);

            foreach (var seed in Seeds.Where(s => !existing.ContainsKey(s.Code)))
            {
                var form = new FormEntity
                {
                    Code = seed.Code,
                    Name = $"Form_{seed.Code}",
                    Url = seed.Url,
                    Icon = seed.Icon,
                    ParentId = seed.ParentCode is null ? null : existing[seed.ParentCode].Id,
                    DisplayOrder = seed.DisplayOrder,
                    IsActive = true
                };

                await uow.FormRepository.AddAsync(form, cancellationToken);
                await uow.SaveChangesAsync(cancellationToken);

                existing[form.Code] = form;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
