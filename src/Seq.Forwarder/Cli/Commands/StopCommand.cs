﻿// Copyright 2016 Datalust Pty Ltd
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.IO;
using System.ServiceProcess;
using Seq.Forwarder.ServiceProcess;

namespace Seq.Forwarder.Cli.Commands
{
    [Command("stop", "Stop the Windows service")]
    class StopCommand : Command
    {
        protected override int Run(TextWriter cout)
        {
            try
            {
                var controller = new ServiceController(SeqForwarderWindowsService.WindowsServiceName);

                if (controller.Status != ServiceControllerStatus.Running)
                {
                    cout.WriteLine("Cannot stop {0}, current status is: {1}", controller.ServiceName, controller.Status);
                    return -1;
                }

                cout.WriteLine("Stopping {0}...", controller.ServiceName);
                controller.Stop();

                if (controller.Status != ServiceControllerStatus.Stopped)
                {
                    cout.WriteLine("Waiting up to 60 seconds for the service to stop (currently: " + controller.Status + ")...");                            
                    controller.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(60));
                }

                if (controller.Status == ServiceControllerStatus.Stopped)
                {
                    cout.WriteLine("Stopped.");
                    return 0;
                }

                cout.WriteLine("The service hasn't stopped successfully.");
                return -1;                
            }
            catch (Exception ex)
            {
                cout.WriteLine(ex.Message);
                if (ex.InnerException != null)
                    cout.WriteLine(ex.InnerException.Message);
                return -1;
            }
        }
    }
}
