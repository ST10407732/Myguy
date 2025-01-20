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
                // Validate the coordinates
                if (latitude < -90 || latitude > 90 || longitude < -180 || longitude > 180)
                {
                    throw new ArgumentOutOfRangeException("Invalid coordinates.");
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
                }
                else
                {
                    // Handle case where the task does not exist
                    await Clients.Caller.SendAsync("Error", "Task not found.");
                }
            }
            catch (Exception ex)
            {
                // Handle any errors that may occur
                Console.Error.WriteLine(ex.Message);
                await Clients.Caller.SendAsync("Error", "Failed to update driver location.");
            }
        }

        // Add a client to a task group (using taskId)
        public async Task JoinTaskGroup(int taskId)
        {
            string groupName = taskId.ToString(); // Use the task ID as the group name
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("Notify", $"A client has joined task group {groupName}.");
        }

        // Remove a client from a task group (using taskId)
        public async Task LeaveTaskGroup(int taskId)
        {
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
            // Optionally log or handle disconnection events
        }
    }
}
