using LocationsTracker.Data;
using Microsoft.Maui.Controls.Maps;

namespace LocationsTracker.CustomControls
{
    /// <summary>
    /// The HeatMap class is a custom map control that adds heat map points to the map.
        /// <summary>
    public class HeatMap : Microsoft.Maui.Controls.Maps.Map
    {
        public void AddHeatMapPoints(IEnumerable<LocationData> locations)
        {
            foreach (var location in locations)
            {
                var pin = new Pin
                {
                    Label = "Heat Point",
                    Location = new Location(location.Latitude, location.Longitude),
                    Type = PinType.Place
                };
                Pins.Add(pin);
            }
        }
    }
}
