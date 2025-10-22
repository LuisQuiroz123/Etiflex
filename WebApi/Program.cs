using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WebApi.Data;
using WebApi.Services;


var builder = WebApplication.CreateBuilder(args);

// Configuración Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// DbContext con SQL Server y logging EF Core
builder.Services.AddDbContext<EtiDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information));

// Servicios de MVC
builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
            options.JsonSerializerOptions.WriteIndented = true; 
        });

builder.Services.AddHttpClient<AccessManagerService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5235/");
});



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<SalesOrderService>();



builder.Services.AddHttpClient<CermOrderService>(client =>
{
    client.BaseAddress = new Uri("https://secure.cerm.be/hd/"); // URL de CERM
});


var app = builder.Build();

// Manejo global de errores
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (exceptionHandlerPathFeature != null)
        {
            Log.Error(exceptionHandlerPathFeature.Error, "Error inesperado en la aplicación");
        }

        await context.Response.WriteAsJsonAsync(new
        {
            error = "Ocurrió un error interno en el servidor"
        });
    });
});



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();


