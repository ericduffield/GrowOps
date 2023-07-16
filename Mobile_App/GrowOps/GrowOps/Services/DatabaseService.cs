using Firebase.Database.Offline;
using Firebase.Database;
using GrowOps.Interfaces;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace GrowOps.Services
{
    /// <summary>
    /// GrowOps
    /// Semester 6 April 28th, 2023
    /// App Dev III
    /// </summary>

    public class DatabaseService<T> : IDataStore<T> where T : class, IHasUKey
    {
        private readonly RealtimeDatabase<T> _realtimeDb;
        private ObservableCollection<T> _items;
        public ObservableCollection<T> Items
        {
            get
            {
                if (_items == null)
                    Task.Run(() => LoadItems()).Wait();
                return _items;
            }
        }
        private async Task LoadItems()
        {
            _items = new ObservableCollection<T>(await GetItemsAsync());
        }

        public DatabaseService(Firebase.Auth.User user, string path, string BaseUrl, string key = "")
        {
            FirebaseOptions options = new FirebaseOptions()
            {
                OfflineDatabaseFactory = (t, s) => new OfflineDatabase(t, s),
                AuthTokenAsyncFactory = async () => await user.GetIdTokenAsync()
            };
            // The offline database filename is named after type T.
            // So, if you have more than one list of type T objects, you need to differentiate it
            // by adding a filename modifier; which is what we're using the "key" parameter for.
            var client = new FirebaseClient(BaseUrl, options);
            _realtimeDb =
                client.Child(path)
                .AsRealtimeDatabase<T>(key, "", StreamingOptions.LatestOnly, InitialPullStrategy.MissingOnly, true);
        }

        public async Task<bool> AddItemAsync(T item)
        {
            try
            {
                string key = _realtimeDb.Post(item); //returns the unique key 
                item.Key = key; //Using the interface IHasKey to save the key to object
                _realtimeDb.Put(key, item); //Update the entry in the database to maintain the key
                Items.Add(item); //place new item in the observable collection for UI display
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return await Task.FromResult(false);
            }
            return await Task.FromResult(true);
        }

        public Task<bool> DeleteItemAsync(T item)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false)
        {
            if (_realtimeDb.Database?.Count == 0)
            {
                try
                {
                    await _realtimeDb.PullAsync();
                }
                catch (Exception)
                {
                    return null;
                }
            }
            var result = _realtimeDb.Once().Select(x => x.Object);
            return await Task.FromResult(result);
        }

        public Task<bool> UpdateItemAsync(T item)
        {
            throw new NotImplementedException();
        }
    }
}
