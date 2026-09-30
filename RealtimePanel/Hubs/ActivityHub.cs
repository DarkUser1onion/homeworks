using Microsoft.AspNetCore.SignalR;

namespace RealtimePanel.Hubs;

public class ActivityHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Clients.Others.SendAsync("UserConnected", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
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
        await Clients.Caller.SendAsync("RoomJoined", roomName);
    }

    public async Task LeaveRoom(string roomName)
    {
        if (string.IsNullOrWhiteSpace(roomName)) return;

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        await Clients.Caller.SendAsync("RoomLeft", roomName);
    }

    public async Task SendToRoom(string roomName, string text)
    {
        if (string.IsNullOrWhiteSpace(roomName) || string.IsNullOrWhiteSpace(text)) return;

        await Clients.Group(roomName).SendAsync("ReceiveRoomMessage", Context.ConnectionId, roomName, text);
    }
}