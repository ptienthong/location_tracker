using SQLite;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace LocationsTracker.Data
{
    public class LocationDatabase
    {
        private SQLiteAsyncConnection _database;

        public LocationDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<LocationData>();
        }

        public Task<int> SaveLocationAsync(LocationData location)
        {
            return _database.InsertAsync(location);
        }
        public Task<List<LocationData>> GetLocationsAsync() => _database.Table<LocationData>().ToListAsync();
    }
}
