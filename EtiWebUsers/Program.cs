using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EtiWebUsers.Data; // Tu DbContext personalizado

var builder = WebApplication.CreateBuilder(args);

//  Configurar la cadena de conexión
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

//  Configurar DbContext con SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

//  Identity para autenticación de usuarios
//builder.Services.AddDefaultIdentity<IdentityUser>(options =>
//{
//    options.SignIn.RequireConfirmedAccount = true; // Confirmación de correo opcional
//})
//    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
    .AddRoles<IdentityRole>() //  esto agrega soporte a roles
    .AddEntityFrameworkStores<ApplicationDbContext>();

//  MVC con vistas
builder.Services.AddControllersWithViews();

//  Para errores detallados en desarrollo
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

//  Middleware
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

//  Sirve archivos estáticos (CSS, JS, imágenes)
app.UseStaticFiles();

app.UseRouting();

//  Autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

//  Rutas MVC y Razor Pages (Identity)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
