using Microsoft.AspNetCore.SignalR;
using RealtimePanel.Services;

namespace RealtimePanel.Hubs;

public class ActivityHub : Hub
{
    private readonly ConnectionTracker _tracker;

    public ActivityHub(ConnectionTracker tracker) => _tracker = tracker;

    public override async Task OnConnectedAsync()
    {
        await _tracker.RegisterAsync(Context.ConnectionId);
        await Clients.Others.SendAsync("UserConnected", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await _tracker.RemoveAsync(Context.ConnectionId);
        await Clients.All.SendAsync("UserDisconnected", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        await Clients.All.SendAsync("ReceiveMessage", Context.ConnectionId, text);
    }

    public async Task JoinRoom(string roomName)
    {
        if (string.IsNullOrWhiteSpace(roomName)) return;
        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        await _tracker.UpdateRoomAsync(Context.ConnectionId, roomName);
        await Clients.Caller.SendAsync("RoomJoined", roomName);
    }

    public async Task LeaveRoom(string roomName)
    {
        if (string.IsNullOrWhiteSpace(roomName)) return;
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        await _tracker.UpdateRoomAsync(Context.ConnectionId, "");
        await Clients.Caller.SendAsync("RoomLeft", roomName);
    }

    public async Task SendToRoom(string roomName, string text)
    {
        if (string.IsNullOrWhiteSpace(roomName) || string.IsNullOrWhiteSpace(text)) return;
        await Clients.Group(roomName).SendAsync("ReceiveRoomMessage", Context.ConnectionId, roomName, text);
    }

    public async Task SendPrivate(string targetConnectionId, string text)
    {
        if (string.IsNullOrWhiteSpace(targetConnectionId) || string.IsNullOrWhiteSpace(text)) return;

        await Clients.Client(targetConnectionId).SendAsync("PrivateMessage", Context.ConnectionId, text);
        await Clients.Caller.SendAsync("PrivateMessageSent", targetConnectionId, text);
    }

    public async Task Typing(string roomName)
    {
        if (string.IsNullOrWhiteSpace(roomName)) return;
        
        await Clients.OthersInGroup(roomName).SendAsync("UserTyping", Context.ConnectionId, roomName);
    }
}