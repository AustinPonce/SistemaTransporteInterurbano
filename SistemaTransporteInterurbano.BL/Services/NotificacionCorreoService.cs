using MailKit.Net.Smtp;
using MimeKit;
using SistemaTransporteInterurbano.BL.Interfaces;

namespace SistemaTransporteInterurbano.BL.Services;

public class NotificacionCorreoService
    : INotificacionCorreoService
{
    private readonly string _correo =
        "transporteinterurbano93@gmail.com";

    private readonly string _clave =
        "vape ioje cago ketp";

    public async Task EnviarCorreoAsync(
        string destino,
        string asunto,
        string mensaje)
    {
        var email = new MimeMessage();

        email.From.Add(
            MailboxAddress.Parse(_correo));

        email.To.Add(
            MailboxAddress.Parse(destino));

        email.Subject = asunto;

        email.Body = new TextPart("plain")
        {
            Text = mensaje
        };

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(
            "smtp.gmail.com",
            587,
            MailKit.Security.SecureSocketOptions.StartTls);

        await smtp.AuthenticateAsync(
            _correo,
            _clave);

        await smtp.SendAsync(email);

        await smtp.DisconnectAsync(true);
    }
}