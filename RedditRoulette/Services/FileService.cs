using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedditRoulette.Model;
using System.Text.Json;



namespace RedditRoulette.Services
{
    public class FileService
    {
        private readonly string _profilePath;
        private readonly string _apiKeyPath;

        public FileService()
        {
            _profilePath = Path.Combine(FileSystem.AppDataDirectory, "profile.json");
            _apiKeyPath = Path.Combine(FileSystem.AppDataDirectory, "apikey.txt");
        }

        public async Task SaveProfileAsync(Profile profile)
        {
            var json = JsonSerializer.Serialize(profile);
            await File.WriteAllTextAsync(_profilePath, json);
        }

        public async Task<Profile> LoadProfileAsync()
        {
            if (!File.Exists(_profilePath))
            {
                return new Profile();
            }

            var json = await File.ReadAllTextAsync(_profilePath);
            return JsonSerializer.Deserialize<Profile>(json) ?? new Profile();
        }

        public async Task SaveApiKeyAsync(string apiKey)
        {
            await File.WriteAllTextAsync(_apiKeyPath, apiKey);
        }

        public async Task<string> LoadApiKeyAsync()
        {
            if (!File.Exists(_apiKeyPath))
            {
                return string.Empty;
            }

            return await File.ReadAllTextAsync(_apiKeyPath);
        }
    }
}
