using LocationsTracker.CustomControls;
using LocationsTracker.Data;
using LocationsTracker.Services;
using System.Timers;
using System;
using System.IO;

namespace LocationsTracker.Pages
{
    public partial class MainPage : ContentPage
    {
        private readonly LocationService _locationService;
        private LocationDatabase _locationDatabase;
        private CancellationTokenSource? _trackingCancellationTokenSource;
        public MainPage()
        {
            InitializeComponent();

            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "tracked_locations.db");
            _locationDatabase = new LocationDatabase(dbPath);
            _locationService = new LocationService(_locationDatabase);

            // Subscribe to the LocationChanged event
            _locationService.LocationChanged += OnLocationChanged;
        }

        private void OnLocationChanged(object? sender, LocationDatabase locationData)
        {
            // Update the heatmap with the new location
            MainThread.BeginInvokeOnMainThread(() =>
            {
                HeatMapView.AddHeatMapPoints(locationData.GetLocationsAsync().Result);
            });
        }

        private async void StartTracking_Clicked(object sender, EventArgs e)
        {
            
            if (_trackingCancellationTokenSource != null)
            {
                Console.WriteLine("Location tracking is already in progress.");
                return;
            }

            _trackingCancellationTokenSource = new CancellationTokenSource();

            try
            {
                await _locationService.StartTrackingLocationAsync(_trackingCancellationTokenSource);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting location tracking: {ex.Message}");
            }            
        }

        private void StopTracking_Clicked(object sender, EventArgs e)
        {
            if (_trackingCancellationTokenSource != null)
            {
                _locationService.StopTrackingLocation(_trackingCancellationTokenSource);
                _trackingCancellationTokenSource = null;
            }
        }

    }
}
