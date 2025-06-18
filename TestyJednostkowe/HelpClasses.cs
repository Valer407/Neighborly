using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Session;
using Microsoft.EntityFrameworkCore;
using Moq;
using Neighborly.Data;

namespace TestyJednostkowe
{
    public static class DbContextHelper
    {
        public static NeighborlyContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<NeighborlyContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new NeighborlyContext(options);
        }
    }
    public class DummySession : ISession
    {
        private Dictionary<string, byte[]> _sessionStorage = new();

        public IEnumerable<string> Keys => _sessionStorage.Keys;

        public string Id => Guid.NewGuid().ToString();

        public bool IsAvailable => true;

        public void Clear() => _sessionStorage.Clear();

        public void Remove(string key) => _sessionStorage.Remove(key);

        public void Set(string key, byte[] value) => _sessionStorage[key] = value;

        public bool TryGetValue(string key, out byte[] value) => _sessionStorage.TryGetValue(key, out value);

        public void SetInt32(string key, int value) => Set(key, BitConverter.GetBytes(value));

        public int? GetInt32(string key)
        {
            if (_sessionStorage.TryGetValue(key, out var data))
            {
                return BitConverter.ToInt32(data, 0);
            }
            return null;
        }

        public void SetString(string key, string value) => Set(key, Encoding.UTF8.GetBytes(value));

        public string GetString(string key)
        {
            if (_sessionStorage.TryGetValue(key, out var data))
            {
                return Encoding.UTF8.GetString(data);
            }
            return null;
        }

        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
