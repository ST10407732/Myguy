using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MYGUYY.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class NotificationHub : Hub
{
    private static readonly Dictionary<int, List<string>> _userConnections = new();
    private readonly MYGUYYContext _context;

    public NotificationHub(MYGUYYContext context)
    {
        _context = context;
    }

    public override async Task OnConnectedAsync()
    {
        // Check if the connected user is a driver or client
        if (Context.User?.IsInRole("Driver") == true || Context.User?.IsInRole("Client") == true)
        {
            // Get user ID from user claims
            var userId = int.Parse(Context.User.FindFirst("UserId")?.Value);

            // Ensure there is a list for this user in the connection dictionary
            if (!_userConnections.ContainsKey(userId))
            {
                _userConnections[userId] = new List<string>();
            }

            // Store the connection ID for the user
            _userConnections[userId].Add(Context.ConnectionId);

            // Retrieve any unread notifications from the database for this user
            var pendingNotifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            // Send unread notifications to the user
            foreach (var notification in pendingNotifications)
            {
                await Clients.Client(Context.ConnectionId).SendAsync("ReceiveNotification", notification.Message);

                // Mark the notification as read after sending it
                notification.IsRead = true;
            }

            // Save changes to the database (marking notifications as read)
            await _context.SaveChangesAsync();
        }

        await base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        // Find and remove the connection ID for the disconnected user
        foreach (var userId in _userConnections.Keys)
        {
            _userConnections[userId].Remove(Context.ConnectionId);
            if (!_userConnections[userId].Any()) // Remove user from dictionary if no connections left
            {
                _userConnections.Remove(userId);
            }
        }

        return base.OnDisconnectedAsync(exception);
    }

    // Send notifications to all connected users (drivers and clients)
    public async Task SendTaskNotificationToAllUsers(string message)
    {
        foreach (var userId in _userConnections.Keys)
        {
            foreach (var connectionId in _userConnections[userId])
            {
                await Clients.Client(connectionId).SendAsync("ReceiveNotification", message);
            }
        }
    }

    // Notify users (drivers and clients) of a new task and store it in the database
    public async Task NotifyUsersOfNewTask(int taskId, string message)
    {
        // Retrieve all users from the database (drivers and clients)
        var users = await _context.Users
            .Where(u => u.Role == "Driver" || u.Role == "Client")
            .ToListAsync();

        // Save notifications for all users
        foreach (var user in users)
        {
            var notification = new MYGUYY.Models.Notification
            {
                UserId = user.Id,  // Using UserId instead of DriverId
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                TaskId = taskId
            };
            _context.Notifications.Add(notification);
        }

        await _context.SaveChangesAsync();

        // Optionally, send real-time notifications to online users (drivers and clients)
        await SendTaskNotificationToAllUsers(message);
    }
}
