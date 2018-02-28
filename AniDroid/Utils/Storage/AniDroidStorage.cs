using System;
using System.Threading.Tasks;
using Android.Content;
using AniDroid.Utils.Interfaces;
using Newtonsoft.Json;
using Trace = System.Diagnostics.Trace;

namespace AniDroid.Utils.Storage
{
    internal abstract class AniDroidStorage
    {
        protected abstract string Group { get; }

        private readonly ISharedPreferences _prefs;

        protected AniDroidStorage(Context c)
        {
            _prefs = c.GetSharedPreferences(Group, FileCreationMode.Private);
        }

        /// <summary>
        /// Persists a value with given key.
        /// </summary>
        /// <param name="key">The key used for the key-value pair.</param>
        /// <param name="value">The value of the key-value pair. If value is null, the key will be deleted.</param>
        public void Put(string key, string value)
        {
            _prefs.Edit().PutString(key, value).Apply();
        }

        /// <summary>
        /// Retrives a value with given key. If key can not be found, defaultValue is returned.
        /// </summary>
        /// <param name="key">The key to retrieve data for.</param>
        /// <param name="defaultValue">The default value of the key-value pair, used if the supplied key isn't found.</param>
        public async Task<string> Get(string key, string defaultValue = null)
        {
            return await Task.Run(() => _prefs.GetString(key, defaultValue)).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete the specified key.
        /// </summary>
        /// <param name="key">The key to delete.</param>
        public void Delete(string key)
        {
            _prefs.Edit().Remove(key).Apply();
        }

        /// <summary>
        /// Persists a complex value with given key via binary serialization.
        /// </summary>
        /// <param name="key">The key used for the key-value pair.</param>
        /// <param name="value">The value of the key-value pair. If value is null, the key will be deleted. Value must be serializable.</param>
        public void Put<T>(string key, T value)
        {
            var data = JsonConvert.SerializeObject(value);
            Put(key, data);
        }

        /// <summary>
        /// Retrives a value with given key. If key can not be found, defaultValue is returned.
        /// </summary>
        /// <param name="key">The key to retrieve data for.</param>
        /// <param name="defaultValue">The default value of the key-value pair, used if the supplied key isn't found.</param>
        /// <returns>Deserialzed complex value.</returns>
        public async Task<T> Get<T>(string key, T defaultValue = default(T))
        {
            var data = await Get(key);
            if (data == null)
            {
                return defaultValue;
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(data);
            }
            catch (Exception)
            {
                Trace.TraceError($"Error occurred while deserializing object with key {key} from SharedPreferences.");
                return defaultValue;
            }
        }

        /// <summary>
        /// Clears the group.
        /// </summary>
        /// <returns><c>true</c>, if group had items that were cleared, <c>false</c> if it was empty to begin with.</returns>
        public bool ClearGroup()
        {
            if (_prefs.All.Count == 0)
                return false;
            _prefs.Edit().Clear().Apply();
            return true;
        }
    }
}