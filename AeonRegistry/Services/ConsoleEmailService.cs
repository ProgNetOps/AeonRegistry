using Microsoft.AspNetCore.Identity.UI.Services;

namespace AeonRegistry.Services;

public class ConsoleEmailService : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        Console.WriteLine($"""
            To:{email}
            Subject:{subject}

            {htmlMessage}
            """);

        return Task.CompletedTask;
    }
}
