using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using GrowOps.Models;

using CommunityToolkit.Mvvm.Input;

namespace GrowOps.ViewModels
{
    /// <summary>
    /// GrowOps
    /// Semester 6 April 28th, 2023
    /// App Dev III
    /// Code taken from: https://github.com/icebeam7/MapDemoNetMaui/tree/bindable-behavior
    /// </summary>
    public partial class MapViewModel : BaseViewModel
    {
        public ObservableCollection<Place> Places { get; } = new();

        private CancellationTokenSource cts;
        private IGeolocation geolocation;
        private IGeocoding geocoding;

        [ObservableProperty]
        bool isReady;

        [ObservableProperty]
        ObservableCollection<Place> bindablePlaces;


        public MapViewModel(IGeolocation geolocation, IGeocoding geocoding)
        {
            this.geolocation = geolocation;
            this.geocoding = geocoding;
        }

        [RelayCommand]
        private async Task GetCurrentLocationAsync()
        {
            try
            {
                Location location = new Location(App.Repo.GeoLocationSubsystem.Latitude.Value, App.Repo.GeoLocationSubsystem.Longitude.Value);
                var placemarks = await Geocoding.GetPlacemarksAsync(location);
                var address = placemarks?.FirstOrDefault()?.AdminArea;

                Places.Clear();
                var place = new Place()
                {
                    Location = location,
                    Address = address,
                    Description = "Container Location"
                };

                Places.Add(place);

                var placeList = new List<Place>() { place };
                BindablePlaces = new ObservableCollection<Place>(placeList);
                IsReady = true;
            }
            catch (Exception ex)
            {
                // Unable to get location
            }
        }


        [RelayCommand]
        private void DisposeCancellationToken()
        {
            if (cts != null && !cts.IsCancellationRequested)
                cts.Cancel();
        }
    }
}