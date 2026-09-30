using Microsoft.AspNetCore.SignalR;
using RealtimePanel.Hubs;

namespace RealtimePanel.Services;

public class ConnectionTracker
{
    private readonly IHubContext<ActivityHub> _hub;

    private readonly Dictionary<string, string> _connections = new();
    private readonly object _lock = new();

    public ConnectionTracker(IHubContext<ActivityHub> hub)
    {
        _hub = hub;
    }

    public int Count
    {
        get { lock (_lock) return _connections.Count; }
    }

    public async Task RegisterAsync(string connId)
    {
        lock (_lock) _connections[connId] = "";
        await BroadcastCountAsync();
    }

    public async Task RemoveAsync(string connId)
    {
        lock (_lock) _connections.Remove(connId);
        await BroadcastCountAsync();
    }

    public async Task UpdateRoomAsync(string connId, string? room)
    {
        lock (_lock)
        {
            if (_connections.ContainsKey(connId))
                _connections[connId] = room ?? "";
        }
    }

    public async Task BroadcastCountAsync()
    {
        var count = Count;
        await _hub.Clients.All.SendAsync("OnlineCount", count);
    }

    public async Task SendSystemMessageAsync(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        await _hub.Clients.All.SendAsync("SystemMessage", text);
    }
}