using GameServer.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Infrastructure.Services.StaticData
{
    public class StaticDataService : IStaticDataService
    {
        private readonly Dictionary<string, string> _textCache = new();
        private readonly Dictionary<string, byte[]> _byteCache = new();

        public StaticDataService(string configsDirectory)
        {
            if (!Directory.Exists(configsDirectory))
            {
                throw new DirectoryNotFoundException($"Папка с конфигами не найдена: {configsDirectory}");
            }

            var files = Directory.GetFiles(configsDirectory, "*.*", SearchOption.AllDirectories)
                                 .Where(f => f.EndsWith(".xml") || f.EndsWith(".json"));

            foreach (var filePath in files)
            {
                string key = Path.GetFileName(filePath).ToLower();

                byte[] fileBytes = File.ReadAllBytes(filePath);
                string fileText = System.Text.Encoding.UTF8.GetString(fileBytes);

                _byteCache[key] = fileBytes;
                _textCache[key] = fileText;

                Console.WriteLine($"[StaticData] Loaded: {key}");
            }
        }

        public byte[] GetBytes(string fileName)
        {
            var key = fileName.ToLower();
            if (_byteCache.TryGetValue(key, out var bytes))
            {
                return bytes;
            }
            throw new KeyNotFoundException($"Файл {fileName} не найден в кэше.");
        }

        public string GetText(string fileName)
        {
            var key = fileName.ToLower();
            if (_textCache.TryGetValue(key, out var text))
            {
                return text;
            }
            throw new KeyNotFoundException($"Файл {fileName} не найден в кэше.");
        }
    }
}
