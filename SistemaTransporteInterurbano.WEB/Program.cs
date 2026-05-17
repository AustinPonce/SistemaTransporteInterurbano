using Microsoft.EntityFrameworkCore;
using SistemaTransporteInterurbano.BL.Interfaces;
using SistemaTransporteInterurbano.BL.Services;
using SistemaTransporteInterurbano.DA.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(
    options =>
        options.UseSqlServer(
            builder.Configuration
                .GetConnectionString(
                    "DefaultConnection")));

builder.Services.AddScoped<
    IAutenticacionService,
    AutenticacionService>();

builder.Services.AddScoped<
    INotificacionCorreoService,
    NotificacionCorreoService>();

builder.Services.AddScoped<
    IUsuarioService,
    UsuarioService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern:
        "{controller=Autenticacion}/{action=IniciarSesion}/{id?}");

app.Run();

