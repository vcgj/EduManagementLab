using EduManagementLab.Core.Interfaces;
using EduManagementLab.Core.Services;
using EduManagementLab.EfRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<CourseService>();
builder.Services.AddTransient<CourseLineItemService>();
builder.Services.AddTransient<IMSToolService>();
builder.Services.AddTransient<ResourceLinkService>();

builder.Services.AddControllersWithViews();

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "oidc";
})
.AddCookie("Cookies")
.AddOpenIdConnect("oidc", options =>
{
    options.Authority = builder.Configuration["OpenIdConnect:Authority"];

    options.ClientId = builder.Configuration["OpenIdConnect:ClientId"];
    options.ClientSecret = builder.Configuration["OpenIdConnect:ClientSecret"];
    options.ResponseType = builder.Configuration["OpenIdConnect:ResponseType"];

    options.GetClaimsFromUserInfoEndpoint = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = builder.Configuration["OpenIdConnect:NameClaimType"],
        RoleClaimType = builder.Configuration["OpenIdConnect:RoleClaimType"]
    };

    options.SaveTokens = true;
});

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
    endpoints.MapRazorPages().RequireAuthorization();
});

app.Run();
