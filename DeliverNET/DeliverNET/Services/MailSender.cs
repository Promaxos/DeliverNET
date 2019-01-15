// using SendGrid's C# Library
// https://github.com/sendgrid/sendgrid-csharp
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace DeliverNET.Services
{
    public class MailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // TODO Implement email API ("SendGrid")
            await Execute(email, htmlMessage);
        }

        [AllowAnonymous]
        public async Task Execute(string fromEmail, string htmlMessage)
        {
            // TODO EMail sender is not working
            var apiKey = Environment.GetEnvironmentVariable("SendGrid_ApiKey");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(fromEmail, "Example User");
            var subject = "DeliverNET - Contact Us Message";
            var to = new EmailAddress("stathispantazis7@hotmail.com", "Stathis Pantazis");
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = htmlMessage;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}