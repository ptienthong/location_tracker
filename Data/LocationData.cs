using SQLite;

namespace LocationsTracker.Data
{
    public class LocationData
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
