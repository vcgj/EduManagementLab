using Microsoft.EntityFrameworkCore;
using EduManagementLab.Core.Interfaces;
using EduManagementLab.Core.Services;
using EduManagementLab.EfRepository;
using EduManagementLab.IdentityServer;
using EduManagementLab.Core.Validation;

var builder = WebApplication.CreateBuilder(args);

var assembly = typeof(Program).Assembly.GetName().Name;

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(connectionString, opt => opt.MigrationsAssembly(assembly)));

builder.Services.AddControllersWithViews(); 

builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<CourseService>();
builder.Services.AddTransient<CourseLineItemService>();
builder.Services.AddTransient<IMSToolService>();
builder.Services.AddTransient<ResourceLinkService>();

builder.Services.AddIdentityServer()
    .AddInMemoryIdentityResources(Config.IdentityResources)
    .AddInMemoryApiResources(Config.ApiResources)
    .AddInMemoryClients(Config.Clients)
    .AddInMemoryApiScopes(Config.ApiScopes)
    .AddProfileService<CustomProfileService>()
    .AddResourceOwnerValidator<CustomResourceOwnerPasswordValidator>()
    .AddDeveloperSigningCredential()
    // Adds support for client authentication using JWT bearer assertions
    // where token is signed by a private key stored in PEM format. 
    .AddLtiJwtBearerClientAuthentication();

builder.Services.AddAuthentication();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.UseIdentityServer();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});

app.Run();