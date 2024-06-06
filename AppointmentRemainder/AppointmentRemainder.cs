//using AppointmentRemainder.Data;
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
using AppointmentRemainder.Models;
using System.Reflection;
using Npgsql;

namespace AppointmentRemainder
{
    public partial class AppointmentRemainder : ServiceBase
    {
        //private readonly ApplicationDBContext _context;
        Timer timer = new Timer();
        public AppointmentRemainder(/*ApplicationDBContext context*/)
        {
            //_context = context;
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //timer.Interval = 24 * 60 * 60 * 1000; // 24 hours in milliseconds
            timer.Interval = 5000;
            timer.Elapsed += OnTimedEvent;
            timer.Enabled = true;
            WriteToFile("Appointment Remainder Service started ");
        }

        protected override void OnStop()
        {
            WriteToFile("Appointment Remainder Service stoped ");

        }
        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            #region LINQ
            //        var appointments = _context.Appointments
            //.Where(x => (x.Appointmenttime - DateTime.Now).Value.TotalHours <= 24)
            //.ToList();
            #endregion

            #region Call using ADO.net
            //WriteToFile("Connection String");
            string connectionString = "Host=localhost;Database=AppointmentRemainder;Username=postgres;Password=Postgres123";

            List<Appointments> appointments = new List<Appointments>();
            WriteToFile("Fetching Data from the DB");

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand("SELECT * FROM AppointmentDetails() ", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var appointmentdata = new Appointments
                                {
                                    Id = reader.GetInt32(0),
                                    Email = reader.IsDBNull(1) ? null : reader.GetString(1),
                                    Appointmenttime = reader.GetDateTime(2)
                                };
                                appointments.Add(appointmentdata);
                            }
                        }

                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            WriteToFile("Fetched Data from the DB");
            #endregion

            #region offline appointment data
            //List<Appointments> appointments = new List<Appointments>
            //{
            //    new Appointments
            //    {
            //        Id = 1,
            //        Email = "kirtan.patel@etatvasoft.com",
            //        Appointmenttime = DateTime.Parse("2024-06-06 12:07:16.889248")
            //    },
            //    new Appointments
            //    {
            //        Id = 2,
            //        Email = "kirtan.patel@etatvasoft.com",
            //        Appointmenttime = DateTime.Parse("2024-06-07 12:07:16.889248")
            //    },
            //    new Appointments
            //    {
            //        Id = 3,
            //        Email = "kirtan.patel@etatvasoft.com",
            //        Appointmenttime = DateTime.Parse("2024-06-08 12:07:16.889248")
            //    }
            //};
            #endregion

            foreach (var appointment in appointments)
            {
                // Send reminder email
                //WriteToFile("Remainder mail is send to " + appointment.Email + " for " + appointment.Appointmenttime);
                SendReminderEmail(appointment.Email, appointment.Appointmenttime);
            }
        }

        private void SendReminderEmail(string toemail, DateTime? appointment)
        {
            string from = "test.dotnet@etatvasoft.com";
            string credential = "P}N^{z-]7Ilp";
            string subject = "Appointment Remainder";
            string body = "Your Appointment is of " + appointment;

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
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\EmailRemainderLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
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
