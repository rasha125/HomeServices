using HomeServices.Models;
using HomeServices.Data;
using HomeServices.Models.Repositorie;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
///////////////////////////////////Db////////////////////////
///
IConfiguration configuration = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json").Build();
builder.Services.AddDbContext<AppDBContext>(options =>
{
    options.UseSqlServer(configuration.GetConnectionString("sqlCon"));
});

builder.Services.AddScoped<IRepositorie<Users, string>, dbUsersRepositorie>();
builder.Services.AddScoped<IRepositorie<Services, int>, dbServicesRepositorie>();
builder.Services.AddScoped<IRepositorie<Ratings, int>, dbRatingsRepositorie>();
builder.Services.AddScoped<IRepositorie<Providers, int>, dbProvidersRepositorie>();
builder.Services.AddScoped<IRepositorie<Issues, int>, dbIssuesRepositorie>();
builder.Services.AddScoped<IRepositorie<Orders, int>, dbOrdersRepositorie>();
builder.Services.AddScoped<IRepositorie<Persons, int>, dbPersonsRepositorie>();



builder.Services.AddIdentity<Users, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
    .AddEntityFrameworkStores<AppDBContext>()
    .AddDefaultTokenProviders();




//////////////////////////////////////////////////////////////




var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await AppDbSeeder.SeedAdminAsync(services);
}

app.UseStaticFiles();
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Users}/{action=Login}/{id?}");

app.Run();
