// Hubs/TaskHub.cs
using Microsoft.AspNetCore.SignalR;

public class TaskHub : Hub
{
    // Sends task update to a specific user (client or driver)
    public async Task SendTaskUpdate(int userId, int taskId, string status)
    {
        // Send status update to the specified user (client/driver)
        await Clients.User(userId.ToString()).SendAsync("ReceiveTaskUpdate", taskId, status);
    }
}
