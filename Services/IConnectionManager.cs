namespace ChatApp.Backend.Services;

public interface IConnectionManager
{
    Task AddConnection(int userId, string connectionId);
    Task RemoveConnection(int userId, string connectionId);
    Task<List<string>> GetConnections(int userId);
    Task ClearConnections(int userId);
}

