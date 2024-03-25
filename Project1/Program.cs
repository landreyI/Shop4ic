using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Project1.Data;
using Project1.Models.DBModels;
using Project1.Service;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddHttpClient();
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(1);
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.SlidingExpiration = true;
    });
builder.Services.AddScoped<DBService>();

var connectionStringBD = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDBContext>(options => {
    options.LogTo(Console.WriteLine);
    options.UseSqlServer(connectionStringBD);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseAuthentication();
app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    // ѕолучаем сервис IDBService из контейнера зависимостей
    var dbService = context.RequestServices.GetRequiredService<DBService>();

    if (context.User.Identity.IsAuthenticated)
    {
        var roleClaim = context.User.FindFirstValue(ClaimTypes.Role);
        int? idAuth = UserVerification.checAuthorization(context);
        UserModel userAuth = dbService.GetUser(idAuth);
        if (userAuth != null && userAuth.role != roleClaim)
        {
            await context.SignOutAsync("Cookies");
            await context.SignInAsync("Cookies", UserVerification.AuthorizationUser(userAuth), UserVerification.AuthenProperties());
        }
    }

    await next();
});

app.UseStaticFiles();

app.UseSession();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
