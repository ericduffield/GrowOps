using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Firebase.Database;
using Firebase.Database.Offline;
using GrowOps.Interfaces;
using GrowOps.Models;

namespace GrowOps.Services
{
    public class UserRoleAuthenticationService<T> where T : class, IHasUKey
    {
        private readonly RealtimeDatabase<User> _realtimeDb;
        private ObservableCollection<User> _users;
        public User CurrentUser { get; set; }

        public UserRoleAuthenticationService(Firebase.Auth.User user, string path, string BaseUrl, string key = "")
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
                .AsRealtimeDatabase<User>(key, "", StreamingOptions.Everything, InitialPullStrategy.Everything, true);
        }
        public ObservableCollection<User> Users
        {
            get
            {
                if (_users == null)
                    Task.Run(() => LoadItems()).Wait();
                return _users;
            }
        }
        private async Task LoadItems()
        {
            _users = new ObservableCollection<User>(await GetItemsAsync());
        }
        public async Task<IEnumerable<User>> GetItemsAsync(bool forceRefresh = false)
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
        private async Task<bool> AddUser(User user)
        {
            try
            {
                string key = _realtimeDb.Post(user); //returns the unique key 
                user.Key = key; //Using the interface IHasKey to save the key to object
                _realtimeDb.Put(key, user); //Update the entry in the database to maintain the key
                Users.Add(user);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return await Task.FromResult(false);
            }
            return await Task.FromResult(true);
        }

        private async Task<User> GetUserFromEmail(string email)
        {
            var result = _realtimeDb.Once().Select(x => x.Object);
            User user = result.Where(user => user.Email == email).FirstOrDefault();
            return user;
        }

        public async Task<bool> CreateUser(string email, string password, string role, bool saveInfo)
        {
            try
            {
                string hash = HashPassword(password);

                User user = new()
                {
                    Email = email,
                    HashedPassword = hash,
                    Role = role,
                    Preferences = new()
                    {
                        SaveLogin = saveInfo,
                    }
                };

                return await AddUser(user);
            }
            catch (BCrypt.Net.SaltParseException e)
            {
                Debug.WriteLine(e);
                return false;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }

        }

        public async Task<bool> UpdateUserPreferences(User user)
        {
            try
            {
                var itemtoUpdate = Users.FirstOrDefault(i => i.Key == user.Key);

                if (itemtoUpdate == null)
                {
                    return false;
                }

                int index = Users.IndexOf(itemtoUpdate);

                Users[index] = user;
                _realtimeDb.Put(user.Key, user);
                return await Task.FromResult(true);
            }
            catch (Exception)
            {
                return await Task.FromResult(false);
            }
        }
        private string HashPassword(string input)
        {
            return BCrypt.Net.BCrypt.HashPassword(input);
        }

        public async Task<User> Authenticate(string email, string password)
        {
            User user = await GetUserFromEmail(email);

            try
            {
                if (BCrypt.Net.BCrypt.Verify(password, user.HashedPassword))
                {
                    CurrentUser = user;
                    return user;
                }
                return null;
            }
            catch (BCrypt.Net.SaltParseException spe)
            {
                Debug.WriteLine(spe);
                return null;
            }
            catch (ArgumentException e)
            {
                Debug.WriteLine(e);
                return null;
            }
        }
    }
}

