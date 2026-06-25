using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SistemaTransporteInterurbano.BL.Interfaces;
using SistemaTransporteInterurbano.BL.Services;
using SistemaTransporteInterurbano.DA.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAutenticacionService, AutenticacionService>();
builder.Services.TryAddScoped<INotificacionCorreoService, NotificacionCorreoService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IChoferService, ChoferService>();
builder.Services.AddScoped<IPasajeroService, PasajeroService>();
builder.Services.AddScoped<IRutaService, RutaService>();
builder.Services.AddScoped<IUnidadService, UnidadService>();
builder.Services.AddScoped<IViajeService, ViajeService>();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();