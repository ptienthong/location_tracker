using Microsoft.Maui.Devices.Sensors;
using LocationsTracker.Data;
using System.Threading.Tasks;

namespace LocationsTracker.Services
{
    public class LocationService
    {
        private readonly LocationDatabase _database;
        public event EventHandler<LocationDatabase>? LocationChanged;
        private LocationData? _lastKnownLocation;

        public LocationService(LocationDatabase database)
        {
            _database = database;
        }

        protected virtual void OnLocationChanged(LocationDatabase locationData)
        {
            LocationChanged?.Invoke(this, locationData);
        }


        public async Task StartTrackingLocationAsync(CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                // Define a location request
                var request = new GeolocationRequest(GeolocationAccuracy.Low, TimeSpan.FromSeconds(30));
                Console.WriteLine("Location tracking started.");
                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    var location = await Geolocation.GetLocationAsync(request, cancellationTokenSource.Token);

                    if (location != null)
                    {
                        var newlocationData = new LocationData
                        {
                            Latitude = location.Latitude,
                            Longitude = location.Longitude,
                            Timestamp = location.Timestamp.DateTime
                        };

                        // Check if this location is different from the last saved location
                        if (IsNewLocation(_lastKnownLocation, newlocationData))
                        {
                            // Save and notify listeners only if the location has changed
                            await Task.Run(async () =>
                            {
                                await _database.SaveLocationAsync(newlocationData);
                                OnLocationChanged(_database);
                            });

                            // Update the last known location
                            _lastKnownLocation = newlocationData;
                        }
                        else
                        {
                            Console.WriteLine("Location unchanged. Not saving.");
                        }

                    }
                    else
                    {
                        Console.WriteLine("Failed to get location.");
                    }

                    // Wait for the next update
                    await Task.Delay(TimeSpan.FromSeconds(2), cancellationTokenSource.Token);
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Location tracking canceled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during location tracking: {ex.Message}");
            }
        }

        public void StopTrackingLocation(CancellationTokenSource cancellationTokenSource)
        {
            cancellationTokenSource?.Cancel();
            Console.WriteLine("Location tracking stopped.");
        }

        /// Compares the new location with the last known location to determine if it should be saved.
        private bool IsNewLocation(LocationData? lastLocation, LocationData newLocation)
        {
            if (lastLocation == null)
                return true;

            // Define a threshold (e.g., 0.0001 degree difference) for significant location change
            const double threshold = 0.0001;

            return Math.Abs(lastLocation.Latitude - newLocation.Latitude) > threshold ||
                Math.Abs(lastLocation.Longitude - newLocation.Longitude) > threshold;
        }
    }
}
