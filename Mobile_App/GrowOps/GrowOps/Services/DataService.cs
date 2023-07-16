using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using GrowOps.Enums;
using GrowOps.Models;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Diagnostics;

namespace GrowOps.Services
{
    /// <summary>
    /// GrowOps
    /// Semester 6 April 28th, 2023
    /// App Dev III
    /// </summary>
    public class DataService : INotifyPropertyChanged
    {
        private static Stopwatch stopwatch = new();
        public static int StopwatchSeconds { get; set; }
        private static bool isFirst = true;

        public event PropertyChangedEventHandler PropertyChanged;

        private bool HasStarted { get; set; } = false;
        public static IntWrapper TelemetryInterval { get; private set; }
        private Twin Twin { get; set; }
        public DataService()
        {

        }
        public ServiceClient ServiceClient { get; set; }
        public BlobContainerClient blobContainerClient { get; set; }
        public EventProcessorClient EventProcessor { get; set; }
        public CancellationTokenSource CurrentToken { get; set; }
        public static RegistryManager registryManager;

        // Invoke the direct method on the device, passing the payload.
        public static async Task InvokeMethodAsync(ServiceClient serviceClient, CommandPayload commandPayload, string methodName = "is_online")
        {
            string payload = JsonConvert.SerializeObject(commandPayload.Payload);
            var methodInvocation = new CloudToDeviceMethod(methodName)
            {
                ResponseTimeout = TimeSpan.FromSeconds(30),
            };
            methodInvocation.SetPayloadJson(payload);

            Debug.WriteLine($"Invoking direct method for device: {App.Settings.DeviceId}");

            // Invoke the direct method asynchronously and get the response from the simulated device.
            CloudToDeviceMethodResult response = await serviceClient.InvokeDeviceMethodAsync(App.Settings.DeviceId, methodInvocation);

            Debug.WriteLine($"Response status: {response.Status}, payload:\n\t{response.GetPayloadAsJson()}");

        }

        public async Task ContinueProcessing()
        {
            if (!HasStarted) return;

            stopwatch.Restart();
            CurrentToken = new CancellationTokenSource();
            CurrentToken.CancelAfter(TimeSpan.FromMinutes(10));

            await EventProcessor.StartProcessingAsync(CurrentToken.Token);
            await Task.Delay(Timeout.Infinite, CurrentToken.Token);
        }

        public async Task StartProcessing()
        {
            try
            {
                HasStarted = true;
                stopwatch.Start();
                CurrentToken = new CancellationTokenSource();

                EventProcessor.ProcessEventAsync += ProcessEventHandler;
                EventProcessor.ProcessErrorAsync += ProcessErrorHandler;

                Twin = await registryManager.GetTwinAsync(App.Settings.DeviceId);
                TelemetryInterval = new IntWrapper() { Value = (int)Twin.Properties.Desired[Constants.TelemetryInterval] };


                try
                {
                    await EventProcessor.StartProcessingAsync(CurrentToken.Token);
                    await Task.Delay(Timeout.Infinite, CurrentToken.Token);
                }
                catch (TaskCanceledException ex)
                {
                    // This is expected if the cancellation token is
                    // signaled.
                    Debug.WriteLine(ex.Message);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private Task ProcessErrorHandler(ProcessErrorEventArgs args)
        {
            try
            {
                Debug.WriteLine("Error in the EventProcessorClient");
                Debug.WriteLine($"\tOperation: {args.Operation}");
                Debug.WriteLine($"\tException: {args.Exception}");
                Debug.WriteLine("");
            }
            catch
            {
                Debug.WriteLine($"Error in Process error handler");
            }

            return Task.CompletedTask;
        }
        public static async Task ProcessEventHandler(ProcessEventArgs args)
        {
            try
            {
                StopwatchSeconds = (int)(stopwatch.ElapsedMilliseconds / 1000);
                stopwatch.Restart();
                if (args.CancellationToken.IsCancellationRequested)
                {
                    Debug.WriteLine($"Cancellation was requested");
                    return;
                }

                Debug.WriteLine($"Device sent D2C Message: {args.Data.EventBody}");
                List<Reading> readings = ParseReadings(args);
                var dbTask = Task.Run(() => AddReadingsToDatabase(readings));
                if (StopwatchSeconds >= TelemetryInterval.Value || isFirst)
                {
                    UpdateUI(readings);
                    isFirst = false;
                }
                await dbTask;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in ProcessEventHandler: {ex}");
            }
        }

        private static List<Reading> ParseReadings(ProcessEventArgs args)
        {
            JObject obj = JObject.Parse(args.Data.EventBody.ToString());
            List<Reading> readings = new();

            foreach (JProperty prop in obj.Properties())
            {
                try
                {

                    Reading reading;

                    bool boolValue = false;
                    switch (prop.Name)
                    {
                        case Enums.Type.BUZZER:
                            boolValue = true;
                            //App.Repo.GeoLocationSubsystem.BuzzerState.Value = Convert.ToBoolean(prop.Value) ? 0 : 1;
                            break;
                    }
                    if (!boolValue)
                    {
                        JObject propValue;
                        try
                        {
                            propValue = JObject.Parse(prop.Value.ToString());
                            switch (prop.Name)
                            {
                                case Enums.Type.SOIL_MOISTURE:
                                    reading = new Reading(Convert.ToDouble(propValue["value"]), Enums.Unit.UNITS, Enums.Type.SOIL_MOISTURE);
                                    readings.Add(reading);
                                    break;

                                case Enums.Type.TEMPERATURE:
                                    reading = new Reading(Convert.ToDouble(propValue["value"]), Enums.Unit.CELCIUS, Enums.Type.TEMPERATURE);
                                    readings.Add(reading);
                                    break;

                                case Enums.Type.HUMIDITY:
                                    reading = new Reading(Convert.ToDouble(propValue["value"]), Enums.Unit.HUMIDITY, Enums.Type.HUMIDITY);
                                    readings.Add(reading);
                                    break;

                                case Enums.Type.WATER_LEVEL:
                                    reading = new Reading(Convert.ToDouble(propValue["value"]), Enums.Unit.UNITS, Enums.Type.WATER_LEVEL);
                                    readings.Add(reading);
                                    break;

                                case Enums.Type.DOOR_STATE:
                                    reading = new Reading(Convert.ToDouble(propValue["value"]), Enums.Unit.BOOLEAN, Enums.Type.DOOR_STATE);
                                    readings.Add(reading);
                                    break;

                                case Enums.Type.LUMINANCE:
                                    reading = new Reading(Convert.ToDouble(propValue["value"]), Enums.Unit.LUMINOSITY, Enums.Type.LUMINANCE);
                                    readings.Add(reading);
                                    break;

                                case Enums.Type.VIBRATION:
                                    reading = new Reading(Convert.ToDouble(propValue["value"]), Enums.Unit.UNITS, Enums.Type.VIBRATION);
                                    readings.Add(reading);
                                    break;

                                case Enums.Type.PITCH:
                                    reading = new Reading(Convert.ToDouble(propValue["value"]), Enums.Unit.DEGREES, Enums.Type.PITCH);
                                    readings.Add(reading);
                                    break;

                                case Enums.Type.ROLL:
                                    reading = new Reading(Convert.ToDouble(propValue["value"]), Enums.Unit.DEGREES, Enums.Type.ROLL);
                                    readings.Add(reading);
                                    break;

                                case Enums.Type.LATITUDE:
                                    reading = new Reading(Convert.ToDouble(propValue["value"]), Enums.Unit.DEGREES, Enums.Type.LATITUDE);
                                    readings.Add(reading);
                                    break;

                                case Enums.Type.LONGITUDE:
                                    reading = new Reading(Convert.ToDouble(propValue["value"]), Enums.Unit.DEGREES, Enums.Type.LONGITUDE);
                                    readings.Add(reading);
                                    break;
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.Message);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
            return readings;
        }

        private static async Task AddReadingsToDatabase(List<Reading> readings)
        {
            foreach (Reading reading in readings)
            {
                await App.Repo.ReadingDB.AddItemAsync(reading);
            }
        }

        public async Task StopProcessing()
        {
            if (!HasStarted) return;
            Debug.WriteLine("Requested to Stop Processing");
            await EventProcessor.StopProcessingAsync(CurrentToken.Token);
        }

        public async Task SetTelemetryInterval(int telemetryInterval)
        {
            var patch = new TwinPatch()
            {
                Properties = new Properties()
                {
                    Desired = new Desired()
                    {
                        TelemetryInterval = telemetryInterval
                    }
                }
            };
            try
            {
                string jsonPatch = JsonConvert.SerializeObject(patch);
                await registryManager.UpdateTwinAsync(Twin.DeviceId, jsonPatch, Twin.ETag);
                TelemetryInterval.Value = telemetryInterval;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
        }

        private static void UpdateUI(List<Reading> readings)
        {
            foreach (Reading reading in App.Repo.GeoLocationSubsystem.Readings)
            {
                var matches = readings.Where(currentReading => currentReading.Type == reading.Type).ToList();
                if (matches.Any())
                {
                    App.Repo.GeoLocationSubsystem.Set(matches[0]);

                }

            }
            foreach (Reading reading in App.Repo.SecuritySubsystem.Readings)
            {
                var matches = readings.Where(currentReading => currentReading.Type == reading.Type).ToList();
                if (matches.Any())
                {
                    App.Repo.SecuritySubsystem.Set(matches[0]);
                }
            }
            foreach (Reading reading in App.Repo.PlantSubsystem.Readings)
            {
                var matches = readings.Where(currentReading => currentReading.Type == reading.Type).ToList();
                if (matches.Any())
                {
                    App.Repo.PlantSubsystem.Set(matches[0]);
                }
            }
        }
    }
}