using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asp.Core.Domains;
using Asp.Data;
using Microsoft.Extensions.Caching.Memory;

namespace Asp.Repositories.Settings
{
    public class SettingRepository : ISettingRepository
    {
        private const string SettingsAllKey = "setting.all";
        private readonly IDbContext _context;
        private readonly IMemoryCache _cache;

        public SettingRepository(IDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public Setting GetSettingById(int settingId)
        {
            if (settingId == 0)
                return null;

            var query = _context.Settings
                .Where(b => b.Id == settingId);

            return query.FirstOrDefault();
        }

        public string GetSettingByKey(string key, string defaultValue)
        {
            if (string.IsNullOrWhiteSpace(key))
                return defaultValue;

            var settings = GetAllSettings();
            key = key.Trim().ToLowerInvariant();

            var setting = settings.FirstOrDefault(x => x.Name == key);

            if (setting != null)
                return setting.Value;

            return defaultValue;
        }

        public T GetSettingByKey<T>(string key, T defaultValue)
        {
            if (string.IsNullOrWhiteSpace(key))
                return defaultValue;

            var settings = GetAllSettings();
            key = key.Trim().ToLowerInvariant();

            var setting = settings.FirstOrDefault(x => x.Name == key);

            if (setting != null)
                return (T) Convert.ChangeType(setting.Value, typeof (T));

            return defaultValue;
        }

        public IList<Setting> GetAllSettings()
        {
            string key = string.Format(SettingsAllKey);
            IList<Setting> setting = null;
            if (!_cache.TryGetValue(key, out setting))
            {
                setting = _context.Settings
                    .OrderBy(s => s.Name)
                    .ToList();

                var options = new MemoryCacheEntryOptions().SetSlidingExpiration(Core.Constants.CacheTimes.DefaultTimeSpan);
                _cache.Set(key, setting, options);
            }
            return setting;
        }

        public void ClearCache()
        {
            _cache.Remove(SettingsAllKey);
        }

        public async Task UpdateSettingAsync(Setting setting)
        {
            _context.Settings.Update(setting);
            await _context.SaveChangesAsync();

            _cache.Remove(SettingsAllKey);
        }
    }
}