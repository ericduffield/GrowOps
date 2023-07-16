using GrowOps.Repos;
using GrowOps.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Devices;
using GrowOps.Services;
using Azure.Storage.Blobs;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Processor;
using Newtonsoft.Json.Linq;
using GrowOps.Models;
using System.Globalization;
using LiteDB;

namespace GrowOps;

public partial class App : Application
{
    /// <summary>
    /// GrowOps
    /// Semester 6 April 28th, 2023
    /// App Dev III
    /// </summary>
    public static Settings Settings { get; private set; }
    public static DataService DataService { get; private set; }
    private event EventHandler Starting = delegate { };
    private event EventHandler Stopping = delegate { };
    private event EventHandler Resuming = delegate { };
    private static DataRepo repo;
    public static DataRepo Repo
    {
        get
        {
            return repo ??= new DataRepo();
        }
    }
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();

        //Initialize everything that has to do with appsettings.json here
        Settings = MauiProgram.Services.GetService<IConfiguration>().GetRequiredSection(nameof(Settings)).Get<Settings>();
        DataService = MauiProgram.Services.GetService<DataService>();
        DataService.ServiceClient = ServiceClient.CreateFromConnectionString(Settings.HubConnectionString);

        DataService.blobContainerClient = new BlobContainerClient(Settings.StorageConnectionString,
        Settings.BlobContainerName);
        DataService.EventProcessor = new EventProcessorClient(
              DataService.blobContainerClient,
              Settings.ConsumerGroup,
              Settings.EventHubConnectionString,
              Settings.EventHubName);
        DataService.registryManager = RegistryManager.CreateFromConnectionString(Settings.HubConnectionString);
    }
    protected override void OnResume()
    {
        base.OnResume();
        //subscribe to event
        Resuming += OnResuming;
        //raise event
        Resuming(this, EventArgs.Empty);
    }
    protected override void OnSleep()
    {
        base.OnSleep();
        Stopping += OnStopping;
        Stopping(this, EventArgs.Empty);
    }

    private async void OnResuming(object sender, EventArgs args)
    {
        Resuming -= OnResuming;
        await DataService.ContinueProcessing();
    }

    private async void OnStarting(object sender, EventArgs args)
    {
        //unsubscribe from event
        Starting -= OnStarting;
        //perform non-blocking actions
        await DataService.StartProcessing();
    }
    private async void OnStopping(object sender, EventArgs args)
    {
        Stopping -= OnStopping;
        await DataService.StopProcessing();
    }
}