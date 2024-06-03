using System.Net.Mail;
using System.Net;

namespace DemoAPIs.Services
{
    public class EmailService
    {
        #region Send Mail
        public void SendMail(string from, string to, string credential, string subject, string body, List<string>? attachments)
        {
            SmtpClient smtpClient = new SmtpClient("mail.etatvasoft.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(from, credential),
            };

            try
            {
                var mail = new MailMessage(from, to);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                if (attachments != null)
                {
                    foreach (var attachment in attachments)
                    {
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", attachment);
                        if (System.IO.File.Exists(filePath))
                        {
                            mail.Attachments.Add(new Attachment(filePath));
                        }
                    }
                }
                smtpClient.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        #endregion
    }
}
