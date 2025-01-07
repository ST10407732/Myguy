using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace MYGUYY.Hubs
{
    public class MessageHub : Hub
    {
        /// <summary>
        /// Sends a message to a specific task group.
        /// </summary>
        /// <param name="taskId">The task ID for the conversation.</param>
        /// <param name="userId">The sender's user ID.</param>
        /// <param name="username">The sender's username.</param>
        /// <param name="content">The message content.</param>
        public async Task SendMessage(int taskId, int userId, string username, string content)
        {
            // Broadcast the message to all members of the task group
            await Clients.Group(taskId.ToString()).SendAsync("ReceiveMessage", new
            {
                TaskId = taskId,
                UserId = userId,
                Username = username,
                Content = content,
                SentAt = System.DateTime.Now.ToString("g")
            });
        }

        /// <summary>
        /// Adds a connection to a task-specific group.
        /// </summary>
        /// <param name="taskId">The task ID.</param>
        public Task JoinTaskGroup(int taskId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, taskId.ToString());
        }

        /// <summary>
        /// Removes a connection from a task-specific group.
        /// </summary>
        /// <param name="taskId">The task ID.</param>
        public Task LeaveTaskGroup(int taskId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, taskId.ToString());
        }
    }
}
