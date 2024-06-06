//using AppointmentRemainder.Data;
//using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentRemainder
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            //var services = new ServiceCollection();
            //services.AddDbContext<ApplicationDBContext>();
            //services.AddSingleton<ServiceBase, AppointmentRemainder>();
            //var serviceProvider = services.BuildServiceProvider();
            //ServiceBase.Run(serviceProvider.GetRequiredService<ServiceBase>());

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new AppointmentRemainder()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
