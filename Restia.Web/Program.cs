using Restia.Common.Identity.Extensions;
using Restia.Common.Localization.Extensions;
using Restia.Common.Notification.Service;
using Restia.Common.Shared.Extensions;
using Restia.Common.Tenant.Constants;
using Restia.Common.Tenant.Middlewares;

var builder = WebApplication.CreateBuilder(args);
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

// Add services to the container.
builder.Services.AddControllersWithViews(o => o.AddStringTrimModelBinderProvider());
builder.Services.AddLocalizationModule();
builder.Services.AddInfrastructureModules(builder.Configuration);
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.UseHttpsRedirection();
app.UseRequestLocalization();
app.UseCors();

app.MapStaticAssets();

app.UseRouting();

app.UseMiddleware<TenantResolutionMiddleware>();
app.UseAuthentication();
app.UseMiddleware<TenantIsolationMiddleware>();
app.UseAuthorization();

app.MapGet("/", (HttpContext context) =>
{
    if (context.User.Identity?.IsAuthenticated == true)
    {
        var userSchema = context.User.GetSchemaId();
        var targetTenant = TenantConstants.IsValidSchemaName(userSchema)
            ? userSchema.ToLowerInvariant()
            : TenantConstants.DefaultTenantId;
        return Results.Redirect($"/{targetTenant}");
    }

    return Results.Redirect($"/{TenantConstants.DefaultTenantId}/Account/Login");
});

app.MapControllerRoute(
    name: "default",
    pattern: "{tenantId}/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapHub<NotificationHub>("/notificationHub");

app.Run();
