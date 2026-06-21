using Microsoft.AspNetCore.SignalR;

namespace pointCounterBackend.Hubs;

public class PointMatchHub : Hub
{
    public static string GroupName(string publicId) => $"match:{publicId}";

    public async Task JoinMatch(string publicId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(publicId));
    }

    public async Task LeaveMatch(string publicId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(publicId));
    }
}
