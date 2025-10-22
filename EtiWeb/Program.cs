using EtiWeb.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EtiWeb.Services;

var builder = WebApplication.CreateBuilder(args);

// ======= Configuración de servicios =======
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

//  ApplicationDbContext para Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

//  EtiContext para tus entidades de negocio
builder.Services.AddDbContext<EtiContext>(options =>
    options.UseSqlServer(connectionString));

//  Identity con roles
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

//  Página de desarrollo para errores de DB
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//  MVC + Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddScoped<PrinterService>();

//  HttpClient para PrintController
var apiBaseUrl = builder.Configuration["ApiWebBaseUrl"] ?? "http://localhost:5235/";
builder.Services.AddHttpClient<ApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});


// ======= Construir la app =======
var app = builder.Build();

// ======= Middleware =======
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// ======= Rutas =======
// Rutas de áreas
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Ruta default
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Razor Pages (Identity)
app.MapRazorPages();

// ======= Ejecutar =======
app.Run();
