using FluentEmail.Core;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Worktime.Global;
using Worktime.Models;
using Worktime.Settings;

namespace Worktime.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        private async Task SendEmailAsync(MimeMessage email)
        {
            using var smtp = new SmtpClient();
#if DEBUG
            DisableCertificateValidation();
#endif
            smtp.Connect(_mailSettings.Smtp, _mailSettings.PortSmtp, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);

            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }

        static void DisableCertificateValidation()
        {
            // Disabling certificate validation can expose you to a man-in-the-middle attack
            // which may allow your encrypted message to be read by an attacker
            // https://stackoverflow.com/a/14907718/740639
            ServicePointManager.ServerCertificateValidationCallback =
                delegate (
                    object s,
                    X509Certificate certificate,
                    X509Chain chain,
                    SslPolicyErrors sslPolicyErrors
                ) {
                    return true;
                };
        }

        private void emailMultiDestinataires(ref MimeMessage email, string toEmail)
        {
            string[] adresses = toEmail.Split(';').Select(sValue => sValue.Trim()).ToArray();
            foreach(String s in adresses) 
                email.To.Add(MailboxAddress.Parse(s));            
        }


        public async Task SendMajReport(MajReportRequest request)
        {
            string FilePath = Directory.GetCurrentDirectory() + @"\wwwroot\mail\MessageEmail.html";
            StreamReader str = new StreamReader(FilePath);
            string MailText = str.ReadToEnd();
            str.Close();

            var email = new MimeMessage();
            email.Headers.Add(HeaderId.From, _mailSettings.Mail);
            email.From[0].Name = _mailSettings.DisplayName;

            // Split the request.ToEmail string into individual email addresses
            string[] toEmails = request.ToEmail.Split(';');

            foreach (string toEmail in toEmails)
            {
                // Trim any leading or trailing spaces
                string trimmedEmail = toEmail.Trim();

                // Add each recipient to the Bcc list
                email.Bcc.Add(MailboxAddress.Parse(trimmedEmail));
            }

            email.Subject = $"Rapport de présence du {DateTime.Now.ToShortDateString()} à {DateTime.Now.ToLongTimeString()}";

            MailText = MailText.Replace("[nowTime]", DateTime.Now.ToString());

            string strAbsent = "";
            foreach (var absentItem in request.Absents)
            {
                strAbsent += $"<div class='absent col-2'>{absentItem}</div>";
            }

            string tableData = "";
            foreach(var passageItem in request.Passages)
            {
                string styleCss = passageItem.Type == 1 ? "out" : "in";

                //tableData += $"<tr class='{styleCss}'><td>{passageItem.LogTime}</td><td>" + $"{passageItem.PointerName}</td>" +
                //    $"<td>{passageItem.FirstName}" +
                //    $"</td><td>{passageItem.LastName}" +
                //    $"</td><td>{passageItem.SSN}</td></tr>";
                string formattedDate = passageItem.LogTime.ToString("dd/MM - HH:mm");
                
                tableData += $"<tr class='{styleCss}'><td>{formattedDate}</td><td>{passageItem.PointerName}</td>" +
                    $"<td>{passageItem.FirstName}</td><td>{passageItem.LastName}</td>" +
                    $"<td><a href='https://jcp.worktime.nc/MajPassage/Details?SSN={passageItem.SSN}'>{passageItem.SSN}</a></td></tr>";

            }

            MailText = MailText.Replace("[absent]", strAbsent);
            MailText = MailText.Replace("[tableData]", tableData);

            var plain = new TextPart(MimeKit.Text.TextFormat.Plain) { Text = $"Worktime MajReport - Message - https://jcp.worktime.nc" };
            var html = new TextPart(MimeKit.Text.TextFormat.Html) { Text = MailText };

            var alternative = new MultipartAlternative();
            alternative.Add(plain);
            alternative.Add(html);

            var multipart = new Multipart("mixed");
            multipart.Add(alternative);

            email.Body = multipart;

            await SendEmailAsync(email);
        }

    }
}
