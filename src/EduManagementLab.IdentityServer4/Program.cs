using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EduManagementLab.IdentityServer4.Data;
using EduManagementLab.IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;

var builder = WebApplication.CreateBuilder(args);

var assembly = typeof(Program).Assembly.GetName().Name;

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AspNetIdentityServerDbcontext>(options =>
    options.UseSqlServer(connectionString, opt => opt.MigrationsAssembly(assembly)));

builder.Services.AddControllersWithViews();

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AspNetIdentityServerDbcontext>();

builder.Services.AddIdentityServer()
    .AddAspNetIdentity<IdentityUser>()
    .AddConfigurationStore(options =>
    {
        options.ConfigureDbContext = b => b.UseSqlServer(connectionString, opt => opt.MigrationsAssembly(assembly));
    })
    .AddOperationalStore(options =>
    {
        options.ConfigureDbContext = b => b.UseSqlServer(connectionString, opt => opt.MigrationsAssembly(assembly));
    })
    .AddDeveloperSigningCredential();

builder.Services.AddAuthentication();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using (var scope = builder.Services.BuildServiceProvider().CreateScope())
    {
        var serviceProvider = scope.ServiceProvider;
        var configcontext = serviceProvider.GetRequiredService<ConfigurationDbContext>();
        var usermanager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var aspNetdbContext = serviceProvider.GetRequiredService<AspNetIdentityServerDbcontext>();

        DevTestData.EnsureSeedData(aspNetdbContext, configcontext, usermanager);
    }
}

app.UseStaticFiles();

app.UseRouting();

app.UseIdentityServer();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});

app.Run();