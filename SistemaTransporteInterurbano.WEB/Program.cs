using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
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

builder.Services.TryAddScoped<
    INotificacionCorreoService,
    NotificacionCorreoService>();

builder.Services.AddScoped<
    IUsuarioService,
    UsuarioService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        SistemaTransporteInterurbano.DA.Context.DbInitializer.InitializeAsync(context).Wait();
    }
    catch
    {
    }
}

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
    name: "autenticacion",
    pattern: "Autenticacion/{action=IniciarSesion}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern:
        "{controller=Autenticacion}/{action=IniciarSesion}/{id?}");

app.Run();

