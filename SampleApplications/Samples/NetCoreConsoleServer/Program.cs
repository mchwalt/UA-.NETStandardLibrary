﻿/* Copyright (c) 1996-2016, OPC Foundation. All rights reserved.
   The source code in this file is covered under a dual-license scenario:
     - RCL: for OPC Foundation members in good-standing
     - GPL V2: everybody else
   RCL license terms accompanied with this source code. See http://opcfoundation.org/License/RCL/1.00/
   GNU General Public License as published by the Free Software Foundation;
   version 2 of the License are accompanied with this source code. See http://opcfoundation.org/License/GPLv2
   This source code is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
*/

using Opc.Ua;
using Opc.Ua.Configuration;
using Opc.Ua.Sample;
using System;
using System.Threading.Tasks;

namespace NetCoreConsoleServer
{
    public class ApplicationMessageDlg : IApplicationMessageDlg
    {
        private string message = string.Empty;
        private bool ask = false;

        public override void Message(string text, bool ask)
        {
            this.message = text;
            this.ask = ask;
        }

        public override async Task<bool> ShowAsync()
        {
            if (ask)
            {
                message += " (y/n, default y): ";
                Console.Write(message);
            }
            else
            {
                Console.WriteLine(message);
            }
            if (ask)
            {
                ConsoleKeyInfo result = Console.ReadKey();
                Console.WriteLine();
                return await Task.FromResult((result.KeyChar == 'y') || (result.KeyChar == 'Y') || (result.KeyChar == '\r'));
            }
            else
            {
                return await Task.FromResult(true);
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                ApplicationInstance.MessageDlg = new ApplicationMessageDlg();
                ApplicationInstance application = new ApplicationInstance();
                application.ApplicationName = "UA Sample Server";
                application.ApplicationType = ApplicationType.Server;
                application.ConfigSectionName = "Opc.Ua.SampleServer";

                // load the application configuration.
                Task<ApplicationConfiguration> task = application.LoadApplicationConfiguration(false);
                task.Wait();

                // check the application certificate.
                Task<bool> task2 = application.CheckApplicationInstanceCertificate(false, 0);
                task2.Wait();
                bool certOK = task2.Result;
                if (!certOK)
                {
                    throw new Exception("Application instance certificate invalid!");
                }

                // start the server.
                Task task3 = application.Start(new SampleServer());
                task3.Wait();

                Console.WriteLine("Server started. Press any key to exit...");
                Console.ReadKey(true);
            }
            catch (Exception ex)
            {
                Utils.Trace("ServiceResultException:" + ex.Message);
                Console.WriteLine("Exception: {0}", ex.Message);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey(true);
            }
        }
    }
}
