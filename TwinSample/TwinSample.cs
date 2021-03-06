﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Azure.Devices.Shared;
using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.Devices.Client.Samples
{
    public class TwinSample
    {
        private DeviceClient _deviceClient;

        public TwinSample(DeviceClient deviceClient)
        {
            _deviceClient = deviceClient ?? throw new ArgumentNullException(nameof(deviceClient));
        }

        public async Task RunSampleAsync()
        {
            await _deviceClient.SetDesiredPropertyUpdateCallbackAsync(OnDesiredPropertyChanged, null).ConfigureAwait(false);

            Console.WriteLine("Retrieving twin...");
            Twin twin = await _deviceClient.GetTwinAsync().ConfigureAwait(false);

            Console.WriteLine("\tInitial twin value received:");
            Console.WriteLine($"\t{twin.ToJson()}");

            Console.WriteLine("Sending sample start time as reported property");
            TwinCollection reportedProperties = new TwinCollection();
            reportedProperties["DateTimeLastAppLaunch"] = DateTime.Now;

            await _deviceClient.UpdateReportedPropertiesAsync(reportedProperties).ConfigureAwait(false);

            Console.WriteLine("Waiting 30 seconds for IoT Hub Twin updates...");
            Console.WriteLine($"Use the IoT Hub Azure Portal to change the Twin desired properties within this time.");


            //now start a loop
            for (int loopIndex = 0; loopIndex < 100000; loopIndex++)
            {
                reportedProperties = new TwinCollection();
                reportedProperties["UpdatedProperties"] = DateTime.Now;

                Console.Write($"{DateTime.Now.ToString()}");
                Console.WriteLine("Sending UpdatedProperties Twin " +
                    "reported property");

                await _deviceClient.UpdateReportedPropertiesAsync(reportedProperties).ConfigureAwait(false);

                await Task.Delay(5000);
            }


            await Task.Delay(30 * 1000);
        }

        private async Task OnDesiredPropertyChanged(TwinCollection desiredProperties, object userContext)
        {
            Console.WriteLine("\tDesired property changed:");
            Console.WriteLine($"\t{desiredProperties.ToJson()}");

            Console.WriteLine("\tSending current time as reported property");
            TwinCollection reportedProperties = new TwinCollection();
            reportedProperties["DateTimeLastDesiredPropertyChangeReceived"] = DateTime.Now;

            await _deviceClient.UpdateReportedPropertiesAsync(reportedProperties).ConfigureAwait(false);
        }
    }
}
