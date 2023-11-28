using CoursePilotWebApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using CoursePilotWebApp.Models.Domain;
//Pathan, F. (27 Aug 2019). How To Get Current User Claims In ASP.NET Identity. [online] www.c-sharpcorner.com.
//Available at: https://www.c-sharpcorner.com/blogs/how-to-get-current-user-claims-in-asp-net-identity [Accessed 12 Oct. 2023].
//mjrousos (n.d.). Securing .NET Microservices and Web Applications. [online] learn.microsoft.com.
//Available at: https://learn.microsoft.com/en-us/dotnet/architecture/microservices/secure-net-microservices-web-applications/. [Accessed 12 Oct. 2023].

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<CoursePilotDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("ConnString")));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<CoursePilotDbContext>();
builder.Services.AddSession();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
