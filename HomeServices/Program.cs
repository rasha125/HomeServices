using HomeServices.Models;
using HomeServices.Models.Repositorie;
using Microsoft.EntityFrameworkCore;

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

builder.Services.AddScoped<IRepositorie<Users>, dbUsersRepositorie>();
builder.Services.AddScoped<IRepositorie<Services>, dbServicesRepositorie>();
builder.Services.AddScoped<IRepositorie<Ratings>, dbRatingsRepositorie>();
builder.Services.AddScoped<IRepositorie<Providers>, dbProvidersRepositorie>();
builder.Services.AddScoped<IRepositorie<Messages>, dbMessagesRepositorie>();
builder.Services.AddScoped<IRepositorie<Issues>, dbIssuesRepositorie>();
builder.Services.AddScoped<IRepositorie<Orders>, dbOrdersRepositorie>();
builder.Services.AddScoped<IRepositorie<Persons>, dbPersonsRepositorie>();




//////////////////////////////////////////////////////////////




var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();
app.UseHttpsRedirection();
app.MapControllerRoute(
name: "default",
pattern: "{controller=}/{action=}/{id?}");

app.Run();
