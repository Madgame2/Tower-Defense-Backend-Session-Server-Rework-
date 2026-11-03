using GameServer.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Infrastructure.Services.StaticData
{
    public class StaticDataService : IStaticDataService
    {
        private readonly string _configsDirectory;

        private readonly Dictionary<string, byte[]> _cache = new(
            StringComparer.OrdinalIgnoreCase);

        public StaticDataService(string configsDirectory)
        {
            if (!Directory.Exists(configsDirectory))
            {
                throw new DirectoryNotFoundException(
                    $"Папка с конфигами не найдена: {configsDirectory}");
            }

            _configsDirectory = configsDirectory;
        }

        public byte[] GetBytes(string fileName)
        {
            if (_cache.TryGetValue(fileName, out var cached))
                return cached;

            string path = Path.Combine(_configsDirectory, fileName);

            if (!File.Exists(path))
            {
                throw new FileNotFoundException(
                    $"Статический файл не найден: {fileName}", path);
            }

            byte[] bytes = File.ReadAllBytes(path);

            _cache.Add(fileName, bytes);

            return bytes;
        }

        public string GetText(string fileName)
        {
            return System.Text.Encoding.UTF8
                .GetString(GetBytes(fileName))
                .TrimStart('\uFEFF');
        }
    }
}
