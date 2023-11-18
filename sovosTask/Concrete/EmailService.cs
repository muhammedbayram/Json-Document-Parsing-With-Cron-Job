using sovosTask.Interfaces;
using System.Net.Mail;
using System.Net;

namespace sovosTask.Concrete
{
    public class EmailService : IEmailService
    {
        public void SendEmail(string recipient, string subject, string body)
        {

            try
            {
                string senderEmail = " "; // sender email
                string senderPassword = " ";  // sender email password
                string smtpServer = "smtp-mail.outlook.com";
                int smtpPort = 587;

                using (SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort))
                {
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);
                    smtpClient.EnableSsl = true;

                    MailMessage mailMessage = new MailMessage(senderEmail, recipient, subject, body);
                    smtpClient.Send(mailMessage);
                }

            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}
