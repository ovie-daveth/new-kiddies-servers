using System.Collections.Concurrent;

namespace ChatApp.Backend.Services;

public class ConnectionManager : IConnectionManager
{
    private readonly ConcurrentDictionary<int, HashSet<string>> _connections = new();

    public Task AddConnection(int userId, string connectionId)
    {
        _connections.AddOrUpdate(userId, 
            new HashSet<string> { connectionId },
            (key, existing) =>
            {
                lock (existing)
                {
                    existing.Add(connectionId);
                    return existing;
                }
            });
        
        return Task.CompletedTask;
    }

    public Task RemoveConnection(int userId, string connectionId)
    {
        if (_connections.TryGetValue(userId, out var connections))
        {
            lock (connections)
            {
                connections.Remove(connectionId);
                if (connections.Count == 0)
                {
                    _connections.TryRemove(userId, out _);
                }
            }
        }
        
        return Task.CompletedTask;
    }

    public Task<List<string>> GetConnections(int userId)
    {
        if (_connections.TryGetValue(userId, out var connections))
        {
            lock (connections)
            {
                return Task.FromResult(connections.ToList());
            }
        }
        
        return Task.FromResult(new List<string>());
    }

    public Task ClearConnections(int userId)
    {
        _connections.TryRemove(userId, out _);
        return Task.CompletedTask;
    }
}

