using GrowOps.Services;
using GrowOps.ViewModels;
using GrowOps.Views.Owner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace GrowOps;

public static class MauiProgram
{
    /// <summary>
    /// GrowOps
    /// Semester 6 April 28th, 2023
    /// App Dev III
    /// Program.cs
    /// </summary>
    public static MauiApp CreateMauiApp()
  {
    var builder = MauiApp.CreateBuilder();
    builder
       .UseSkiaSharp(true)
      .UseMauiApp<App>()
      .ConfigureFonts(fonts =>
      {
        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
        fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
      })
            .UseMauiMaps();

    builder.Services.AddSingleton<IGeolocation>(Geolocation.Default);
    builder.Services.AddSingleton<IGeocoding>(Geocoding.Default);

    var a = Assembly.GetExecutingAssembly();
    using var stream = a.GetManifestResourceStream("GrowOps.appsettings.json");
    var config = new ConfigurationBuilder()
                    .AddJsonStream(stream)
                    .Build();
    builder.Configuration.AddConfiguration(config);

    builder.Services.AddSingleton<DataService>(new DataService());

    builder.Services.AddTransient<MapViewModel>();
    builder.Services.AddTransient<MapPage>();

#if DEBUG
    builder.Logging.AddDebug();
#endif



    var app = builder.Build();
    Services = app.Services;

    return app;
  }
  //Service Property need to access the services in the app 
  public static IServiceProvider Services { get; private set; }
}
