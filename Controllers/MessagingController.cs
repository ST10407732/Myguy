//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using MYGUYY.Data;
//using MYGUYY.Models;
//using System.Linq;
//using System.Threading.Tasks;

//namespace MYGUYY.Controllers
//{
//    [Route("Message")]
//    public class MessagingController : Controller
//    {
//        private readonly MYGUYYContext _context;

//        public MessagingController(MYGUYYContext context)
//        {
//            _context = context;
//        }

//        // GET: Message/ViewMessages?taskId={taskId}
//        [HttpGet("ViewMessages")]
//        public async Task<IActionResult> ViewMessages(int taskId)
//        {
//            var currentUserId = int.Parse(User.FindFirst("UserId").Value);

//            var taskRequest = await _context.TaskRequests
//                                            .Where(tr => tr.Id == taskId)
//                                            .FirstOrDefaultAsync();

//            if (taskRequest == null)
//            {
//                return NotFound("Task request not found.");
//            }

//            var messages = await _context.Messages
//                                         .Where(m => m.TaskRequestId == taskId)
//                                         .OrderBy(m => m.SentAt)
//                                         .ToListAsync();

//            return View(messages);
//        }

//        // POST: Message/SendMessage
//        [HttpPost("SendMessage")]
//        public async Task<IActionResult> SendMessage(int taskId, string content)
//        {
//            var currentUserId = int.Parse(User.FindFirst("UserId").Value);

//            var taskRequest = await _context.TaskRequests
//                                            .Where(tr => tr.Id == taskId)
//                                            .FirstOrDefaultAsync();

//            if (taskRequest == null)
//            {
//                return NotFound("Task request not found.");
//            }

//            int receiverId;
//            if (taskRequest.ClientId == currentUserId)
//            {
//                receiverId = taskRequest.DriverId.GetValueOrDefault();
//            }
//            else if (taskRequest.DriverId == currentUserId)
//            {
//                receiverId = taskRequest.ClientId;
//            }
//            else
//            {
//                return Unauthorized("You are not authorized to send a message for this task.");
//            }

//            var message = new Message
//            {
//                SenderId = currentUserId,
//                ReceiverId = receiverId,
//                TaskRequestId = taskId,
//                Content = content,
//                SentAt = DateTime.Now
//            };

//            _context.Messages.Add(message);
//            await _context.SaveChangesAsync();

//            return RedirectToAction(nameof(ViewMessages), new { taskId });
//        }
//    }
//}
