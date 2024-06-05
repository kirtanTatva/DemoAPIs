using AppointmentRemainder.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Linq;

namespace AppointmentRemainder
{
    public partial class AppointmentRemainder : ServiceBase
    {
        private readonly ApplicationDBContext _context;
        Timer timer = new Timer();
        public AppointmentRemainder(ApplicationDBContext context)
        {
            _context = context;
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //timer.Interval = 24 * 60 * 60 * 1000; // 24 hours in milliseconds
            timer.Interval = 5000;
            timer.Elapsed += OnTimedEvent;
            timer.Enabled = true;
        }

        protected override void OnStop()
        {

        }
        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            var appointments = _context.Appointments
    .Where(x => (x.Appointmenttime - DateTime.Now).Value.TotalHours <= 24)
    .ToList();

            foreach (var appointment in appointments)
            {
                // Send reminder email
                SendReminderEmail(appointment.Email, appointment.Appointmenttime);
            }
        }

        private void SendReminderEmail(string toemail, DateTime? appointment)
        {
            string from = "test.dotnet@etatvasoft.com";
            string credential = "P}N^{z-]7Ilp";
            string subject = "Appointment Remainder";
            string body = "Your Appointment is of "+appointment;

            SmtpClient smtpClient = new SmtpClient("mail.etatvasoft.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(from, credential),
            };

            try
            {
                var mail = new MailMessage(from, toemail);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                
                smtpClient.Send(mail);
                WriteToFile("Remainder mail is send to " + toemail+" for "+appointment);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
    }
}
