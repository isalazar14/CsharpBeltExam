using BeltExam.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();

/* Configure Session (https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-7.0#configure-session-state) */
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromSeconds(10);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;

});

// services.Configure<IdentityOptions>(options =>
// {
//   // Default Password settings.
//   options.Password.RequireDigit = true;
//   options.Password.RequireLowercase = true;
//   options.Password.RequireNonAlphanumeric = true;
//   options.Password.RequireUppercase = true;
//   options.Password.RequiredLength = 8;
//   options.Password.RequiredUniqueChars = 1;
//   // Default User settings.
//   options.User.AllowedUserNameCharacters =
//     "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
//   options.User.RequireUniqueEmail = true;
// });


var configuration = new ConfigurationBuilder()
	.AddJsonFile("appsettings.json")
	.AddJsonFile("appsettings.development.json");


#region DATABASE
// source: https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/working-with-sql?view=aspnetcore-7.0&tabs=visual-studio-code

// GetRequiredSection instead of GetConnectionString to raise an exception if connection string not present.
string? dbConnectionString = builder.Configuration.GetRequiredSection("DBInfo:ConnectionString").Value;


if (dbConnectionString != null)
{
	var serverVersion = new MySqlServerVersion(new Version(8, 0));
	builder.Services.AddDbContext<BeltExamContext>(dbContextOptions => dbContextOptions
	// source: https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql
	.UseMySql(dbConnectionString, serverVersion)
		// The following three options help with debugging, but should
		// be changed or removed for production.
		.LogTo(Console.WriteLine, LogLevel.Information)
		.EnableSensitiveDataLogging()
		.EnableDetailedErrors()
	);
}
#endregion DATABASE

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();

// app.UseCookiePolicy(); // from .net core 2.2 Startup.cs

app.UseRouting();

app.UseAuthorization();

// app.UseIdentity(); // from .net core 2.2 Startup.cs

app.UseSession();


/* Conventional Routing (https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-7.0#conventional-routing) */
// app.MapControllerRoute(
// 	name: "default",
// 	pattern: "{controller=Home}/{action=Index}/{id?}");

/* Attribute routing (https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-7.0#attribute-routing-for-rest-apis) */
app.MapControllers();
	

app.Run();