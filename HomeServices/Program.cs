var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();
app.UseHttpsRedirection();
app.MapControllerRoute(
name: "default",
pattern: "{controller=}/{action=}/{id?}");

app.Run();
