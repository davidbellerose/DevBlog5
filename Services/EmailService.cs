﻿using DevBlog5.ViewModels;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace DevBlog5.Services
{
    public class EmailService : IBlogEmailSender
    {
        private readonly MailSettings _mailSettings;

        public EmailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        //for contact form
        public async Task SendContactEmailAsync(string name, string emailFrom, string? phone, string subject, string htmlMessage)
        {

            var emailSender = _mailSettings.Email ?? Environment.GetEnvironmentVariable("Email");

            MimeMessage newEmail = new();

            newEmail.Sender = MailboxAddress.Parse(emailSender);
            newEmail.To.Add(MailboxAddress.Parse(emailSender));
            newEmail.Subject = subject;

            var builder = new BodyBuilder();

            builder.HtmlBody = $"<b>{name}</b> has sent you an email and can be reached at: <b>{emailFrom} or {phone}</b><br/><br/>{htmlMessage}";

            newEmail.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();

            var host = _mailSettings.EmailHost ?? Environment.GetEnvironmentVariable("EmailHost");
            var port = _mailSettings.EmailPort != 0 ? _mailSettings.EmailPort : int.Parse(Environment.GetEnvironmentVariable("EmailPort")!);
            var password = _mailSettings.EmailPassword ?? Environment.GetEnvironmentVariable("EmailPassword");

            smtp.Connect(host, port, SecureSocketOptions.StartTls);
            smtp.Authenticate(emailSender, password);

            await smtp.SendAsync(newEmail);

            smtp.Disconnect(true);

        }

        // for email confirmation for new user registration
        public async Task SendEmailAsync(string emailTo, string subject, string htmlMessage)
        {
            var emailSender = _mailSettings.Email ?? Environment.GetEnvironmentVariable("Email");

            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(emailSender);
            email.To.Add(MailboxAddress.Parse(emailTo)); // the email to send to; recipient email
            email.Subject = subject;

            var builder = new BodyBuilder()
            {

                HtmlBody = htmlMessage
            };

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();


            var host = _mailSettings.EmailHost ?? Environment.GetEnvironmentVariable("EmailHost");
            var port = _mailSettings.EmailPort != 0 ? _mailSettings.EmailPort : int.Parse(Environment.GetEnvironmentVariable("EmailPort")!);
            var password = _mailSettings.EmailPassword ?? Environment.GetEnvironmentVariable("EmailPassword");

            smtp.Connect(host, port, SecureSocketOptions.StartTls);
            smtp.Authenticate(emailSender, password);

            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }

        //original
        public async Task SendEmailAsync1(string emailTo, string subject, string htmlMessage)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Email);
            email.To.Add(MailboxAddress.Parse(emailTo));
            email.Subject = subject;

            var builder = new BodyBuilder()
            {

                HtmlBody = htmlMessage
            };

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.EmailHost, _mailSettings.EmailPort, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Email, _mailSettings.EmailPassword);

            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
}
