using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using SistemaTransporteInterurbano.BL.Interfaces;

namespace SistemaTransporteInterurbano.BL.Services;

public class NotificacionCorreoService : INotificacionCorreoService
{
    private readonly string _correo;
    private readonly string _clave;

    public NotificacionCorreoService(IConfiguration configuration)
    {
        _correo = configuration["Correo:Direccion"]
            ?? throw new Exception("Correo no configurado.");
        _clave = configuration["Correo:Clave"]
            ?? throw new Exception("Clave de correo no configurada.");
    }

    public async Task EnviarCorreoAsync(string destino, string asunto, string mensaje)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_correo));
        email.To.Add(MailboxAddress.Parse(destino));
        email.Subject = asunto;
        email.Body = new TextPart("plain") { Text = mensaje };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_correo, _clave);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}