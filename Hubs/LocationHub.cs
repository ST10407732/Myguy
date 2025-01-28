namespace MYGUYY.Hubs
{
    using Microsoft.AspNetCore.SignalR;
    using MYGUYY.Data;
    using MYGUYY.Models;
    using System;
    using System.Threading.Tasks;

    public class LocationHub : Hub
    {
        private readonly MYGUYYContext _context; // Database context

        public LocationHub(MYGUYYContext context)
        {
            _context = context; // Initialize the database context
        }

        // Update driver location and notify clients in the task group
        public async Task UpdateDriverLocation(int taskId, double latitude, double longitude)
        {
            try
            {
                if (taskId <= 0)
                {
                    await Clients.Caller.SendAsync("Error", "Invalid task ID.");
                    return;
                }

                // Validate the coordinates
                if (latitude < -90 || latitude > 90 || longitude < -180 || longitude > 180)
                {
                    await Clients.Caller.SendAsync("Error", "Invalid coordinates.");
                    return;
                }

                var taskRequest = await _context.TaskRequests.FindAsync(taskId); // Fetch the task request
                if (taskRequest != null)
                {
                    // Update driver's location in the database
                    taskRequest.DriverLatitude = latitude;
                    taskRequest.DriverLongitude = longitude;

                    _context.TaskRequests.Update(taskRequest);
                    await _context.SaveChangesAsync();

                    // Notify clients in the task group about the updated location
                    await Clients.Group(taskId.ToString()).SendAsync("ReceiveDriverLocation", new
                    {
                        Latitude = latitude,
                        Longitude = longitude,
                        DriverId = taskRequest.DriverId,
                        TaskId = taskId
                    });

                    // Optionally, you can also notify the caller that their location has been updated successfully
                    await Clients.Caller.SendAsync("LocationUpdated", "Driver location updated successfully.");
                }
                else
                {
                    await Clients.Caller.SendAsync("Error", "Task not found.");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error updating driver location: {ex.Message}");
                await Clients.Caller.SendAsync("Error", "Failed to update driver location.");
            }
        }

        // Add a client to a task group (using taskId)
        public async Task JoinTaskGroup(int taskId)
        {
            if (taskId <= 0)
            {
                await Clients.Caller.SendAsync("Error", "Invalid task ID.");
                return;
            }

            string groupName = taskId.ToString(); // Use the task ID as the group name
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("Notify", $"A client has joined task group {groupName}.");
        }

        // Remove a client from a task group (using taskId)
        public async Task LeaveTaskGroup(int taskId)
        {
            if (taskId <= 0)
            {
                await Clients.Caller.SendAsync("Error", "Invalid task ID.");
                return;
            }

            string groupName = taskId.ToString(); // Use the task ID as the group name
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("Notify", $"A client has left task group {groupName}.");
        }

        // Handle client connection
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            await Clients.Caller.SendAsync("Connected", "You are now connected to the LocationHub.");
        }

        // Handle client disconnection
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
            Console.WriteLine("Client disconnected: " + Context.ConnectionId);
        }
    }
}
