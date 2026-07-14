using CartFlow.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace CartFlow.Services.Services
{
        public class EmailService(IConfiguration configuration) : IEmailService
        {
            public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
            {
                var host = configuration["Email:SmtpHost"];
                var portString = configuration["Email:SmtpPort"];
                var username = configuration["Email:SmtpUsername"];
                var password = configuration["Email:SmtpPassword"];
                var fromEmail = configuration["Email:FromEmail"] ?? username;
                var fromName = configuration["Email:FromName"] ?? "CartFlow";

                if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    // Email isn't configured yet (e.g. local dev without SMTP secrets set).
                    // Don't throw — the caller already shows a generic "check your email"
                    // message regardless, to avoid leaking which emails are registered.
                    Console.WriteLine($"[EmailService] SMTP not configured. Password reset link for {toEmail}: {resetLink}");
                    return;
                }

                var port = int.TryParse(portString, out var parsedPort) ? parsedPort : 587;

                using var message = new MailMessage
                {
                    From = new MailAddress(fromEmail!, fromName),
                    Subject = "Reset your CartFlow password",
                    Body = $"""
                Hi,

                We received a request to reset your CartFlow password. Click the link below to choose a new one:

                {resetLink}

                This link will expire in 1 hour. If you didn't request this, you can safely ignore this email.

                — CartFlow
                """,
                    IsBodyHtml = false
                };
                message.To.Add(toEmail);

                using var client = new SmtpClient(host, port)
                {
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = true
                };

                await client.SendMailAsync(message);
            }
        }
}
