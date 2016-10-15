﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace DisplayPi.DocumentWriter
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            var jobConfiguration = new JobHostConfiguration();
            jobConfiguration.UseServiceBus();
            var host = new JobHost(jobConfiguration);
            // The following code ensures that the WebJob will be running continuously
            Functions.CreateDatabase().Wait();
            Functions.CreateCollection().Wait();
            host.RunAndBlock();
        }
    }
}
