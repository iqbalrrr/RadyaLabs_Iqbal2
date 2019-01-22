using System;
using System.Net.Configuration;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace RadyaLabs.Components.Mail
{
    public class SmtpMailClient : IMailClient
    {
        public async Task SendAsync(String email, String subject, String body)
        {
            using (SmtpClient client = new SmtpClient())
            {
                String sender = ((SmtpSection)WebConfigurationManager.GetSection("system.net/mailSettings/smtp")).From;
                MailMessage mail = new MailMessage(sender, email, subject, body);
                mail.SubjectEncoding = Encoding.UTF8;
                mail.BodyEncoding = Encoding.UTF8;
                mail.IsBodyHtml = true;

                await client.SendMailAsync(mail);
            }
        }
    }
}
