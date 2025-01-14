using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using MYGUYY.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using MYGUYY.Models;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Hosting;
using YourNamespace.ViewModels;
using MYGUYY.Models.ViewModels;


namespace MYGUYY.Controllers
{
    using Microsoft.AspNetCore.SignalR;

    using Microsoft.AspNetCore.SignalR;
    using MYGUYY.Models;
    using System.Threading.Tasks;

    namespace MyGuyy.Hubs
    {
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
            }

            // Add a client to a task group
            public async Task JoinTaskGroup(int taskId)
            {
                string groupName = taskId.ToString(); // Use the task ID as the group name
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                await Clients.Group(groupName).SendAsync("Notify", $"A client has joined task group {groupName}.");
            }

            // Remove a client from a task group
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


    public class AccountController : Controller
    {
        private readonly MYGUYYContext _context;
        private readonly object _hostEnvironment;


        public AccountController(MYGUYYContext context)
        {
            _context = context;
        }

        // GET: Render the registration page
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        // GET: Render the registration page
        [HttpGet]
        public IActionResult DriverTasks()
        {
            return View();
        }

        // GET: Render the registration page
        public IActionResult TaskGroup()
        {
            // Set ViewData for title
            ViewData["Title"] = "Task Groups";
            return View();  // Return the view
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string username, string password, string role)
        {
            if (_context.Users.Any(u => u.Username == username))
            {
                ModelState.AddModelError("", "Username already exists.");
                return View();
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                Username = username,
                PasswordHash = hashedPassword,
                Role = role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Registration successful! You can now log in.";
            return RedirectToAction("Login");
        }

        // GET: Render the Login page
        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Handle user login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, string returnUrl = "/")
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("UserId", user.Id.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return user.Role switch
                {
                    "Admin" => RedirectToAction("ManageTasks"),
                    "Driver" => RedirectToAction("TaskRequestsForDriver"),
                    "Client" => RedirectToAction("ClientTasks"),
                    _ => LocalRedirect(returnUrl)
                };
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // GET: Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // Client Features
        [HttpGet]
        [Authorize(Roles = "Client")]
        public IActionResult CreateTask()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Client")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTask(string Description, double pickupLatitude, double pickupLongitude, double dropoffLatitude, double dropoffLongitude)
        {
            var task = new TaskRequest
            {
                Description = Description,
                Status = "Pending",
                ClientId = int.Parse(User.FindFirst("UserId").Value),
                CreatedAt = DateTime.Now,
                PickupLatitude = pickupLatitude,
                PickupLongitude = pickupLongitude,
                DropoffLatitude = dropoffLatitude,
                DropoffLongitude = dropoffLongitude
            };

            _context.TaskRequests.Add(task);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Task created successfully.";
            return RedirectToAction("ClientTasks");
        }
        [HttpGet]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> ClientTasks()
        {
            // Get the logged-in client's ID
            var clientId = int.Parse(User.FindFirst("UserId").Value);

            // Fetch the tasks for the logged-in client, ordered by CreatedAt
            var tasks = await _context.TaskRequests
                .Where(t => t.ClientId == clientId) // Filter tasks by the logged-in client's ID
                .OrderBy(t => t.CreatedAt) // Order by creation date to show recent tasks first
                .ToListAsync();

            // Prepare task summaries using LINQ for the task counts based on status
            var taskSummaries = new
            {
                Total = tasks.Count,
                Pending = tasks.Count(t => t.Status == "Pending"),
                Accepted = tasks.Count(t => t.Status == "Accepted"),
                Completed = tasks.Count(t => t.Status == "Completed"),
                Declined = tasks.Count(t => t.Status == "Declined")  // Added declined status count
            };

            // Pass the task summaries and tasks to the view
            ViewBag.TaskSummaries = taskSummaries;

            // Return the view with the list of tasks
            return View(tasks);
        }

        [HttpPost]
        [Authorize(Roles = "Client")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessageForClient(int taskId, string content, IFormFile file)
        {
            var task = await _context.TaskRequests.FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null || task.ClientId != int.Parse(User.FindFirst("UserId").Value))
            {
                TempData["Message"] = task == null ? "Task not found." : "You are not authorized to send messages for this task.";
                return RedirectToAction("ClientTasks");
            }

            if (string.IsNullOrWhiteSpace(content) && file == null)
            {
                TempData["Message"] = "Message content or file must be provided.";
                return RedirectToAction("ViewMessages", new { taskId });
            }

            string filePath = null;
            string fileType = null;

            // Handle file upload if a file is provided
            if (file != null)
            {
                // Validate file type
                var allowedFileTypes = new[] { "image/jpeg", "image/png", "video/mp4", "video/avi" };
                if (!allowedFileTypes.Contains(file.ContentType))
                {
                    TempData["Message"] = "Invalid file type. Only images (JPEG, PNG) and videos (MP4, AVI) are allowed.";
                    return RedirectToAction("ViewMessages", new { taskId });
                }

                // Save the file
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/messages");
                Directory.CreateDirectory(uploadsFolder); // Ensure the folder exists

                string uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                filePath = $"/uploads/messages/{uniqueFileName}"; // Save relative path for accessibility
                fileType = file.ContentType;
            }

            var message = new Message
            {
                TaskRequestId = taskId,
                Content = content,
                SentAt = DateTime.Now,
                FilePath = filePath, // Save file path
                FileType = fileType  // Save file type
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Message sent successfully.";
            return RedirectToAction("ViewMessages", new { taskId });
        }


        [HttpGet]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> ViewMessages(int taskId)
        {
            var task = await _context.TaskRequests.FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null || task.ClientId != int.Parse(User.FindFirst("UserId").Value))
            {
                TempData["Message"] = task == null ? "Task not found." : "You are not authorized to view messages for this task.";
                return RedirectToAction("ClientTasks");
            }

            var messages = await _context.Messages
                .Where(m => m.TaskRequestId == taskId)
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            // Pass the taskId to the view
            ViewData["TaskId"] = taskId;
            return View(messages);
        }



        [HttpGet]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> TaskRequestsForDriver()
        {
            // Fetch both pending and accepted tasks for the driver
            var tasks = await _context.TaskRequests
                .Where(task => (task.DriverId == null && task.Status == "Pending") || task.Status == "Accepted")
                .Include(task => task.Client)
                .Include(task => task.Driver)  // Optionally, include the Driver if you need that info
                .ToListAsync();

            return View(tasks);
        }

        [HttpPost]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> AcceptTask(int id)
        {
            var task = await _context.TaskRequests.FirstOrDefaultAsync(t => t.Id == id);

            if (task == null || task.DriverId != null || task.Status != "Pending")
            {
                TempData["Message"] = task == null ? "Task not found." : "Task already accepted or invalid status.";
                return RedirectToAction("TaskRequestsForDriver");
            }

            task.DriverId = int.Parse(User.FindFirst("UserId").Value);
            task.Status = "Accepted";
            await _context.SaveChangesAsync();

            TempData["Message"] = "Task accepted.";
            return RedirectToAction("ViewMessagesForDriver", new { taskId = task.Id });
        }

        [HttpPost]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> DeclineTask(int id)
        {
            var task = await _context.TaskRequests.FirstOrDefaultAsync(t => t.Id == id);

            if (task != null && task.Status == "Pending" && task.DriverId == null)
            {
                task.Status = "Declined";
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("TaskRequestsForDriver");
        }

        [HttpGet]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> ViewMessagesForDriver(int taskId)
        {
            var task = await _context.TaskRequests
                .Include(t => t.Client)
                .Include(t => t.Driver)
                .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null || task.DriverId != int.Parse(User.FindFirst("UserId").Value))
            {
                TempData["Message"] = task == null ? "Task not found." : "You are not authorized to view messages for this task.";
                return RedirectToAction("TaskRequestsForDriver");
            }

            var messages = await _context.Messages
                .Where(m => m.TaskRequestId == taskId)
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            ViewData["TaskId"] = taskId;
            return View(messages); // Pass the list of messages to the view
        }

       
        [HttpPost]
        [Authorize(Roles = "Driver")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessageForDriver(int taskId, string content, IFormFile file)
        {
            var task = await _context.TaskRequests.FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null || task.DriverId != int.Parse(User.FindFirst("UserId").Value))
            {
                TempData["Message"] = task == null ? "Task not found." : "You are not authorized to send messages for this task.";
                return RedirectToAction("TaskRequestsForDriver");
            }

            if (string.IsNullOrWhiteSpace(content) && file == null)
            {
                TempData["Message"] = "Message content or file must be provided.";
                return RedirectToAction("ViewMessagesForDriver", new { taskId });
            }

            string filePath = null;
            string fileType = null;

            // Handle file upload if a file is provided
            if (file != null)
            {
                // Validate file type
                var allowedFileTypes = new[] { "image/jpeg", "image/png", "video/mp4", "video/avi" };
                if (!allowedFileTypes.Contains(file.ContentType))
                {
                    TempData["Message"] = "Invalid file type. Only images (JPEG, PNG) and videos (MP4, AVI) are allowed.";
                    return RedirectToAction("ViewMessagesForDriver", new { taskId });
                }

                // Save the file
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/messages");
                Directory.CreateDirectory(uploadsFolder); // Ensure the folder exists

                string uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                filePath = $"/uploads/messages/{uniqueFileName}"; // Save relative path for accessibility
                fileType = file.ContentType;
            }

            var message = new Message
            {
                TaskRequestId = taskId,
                Content = content,
                SentAt = DateTime.Now,
                FilePath = filePath, // Save file path
                FileType = fileType  // Save file type
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Message sent successfully.";
            return RedirectToAction("ViewMessagesForDriver", new { taskId });
        }

        // Admin Features
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult ManageTasks(string search = "", string statusFilter = "All", string sortBy = "CreatedAt")
        {
            var tasksQuery = _context.TaskRequests
                .Include(task => task.Driver)
                .Include(task => task.Client);

            if (!string.IsNullOrEmpty(search))
            {
                tasksQuery = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<TaskRequest, User>)tasksQuery.Where(task => task.Description.Contains(search));
            }

            if (statusFilter != "All")
            {
                tasksQuery = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<TaskRequest, User>)tasksQuery.Where(task => task.Status.Equals(statusFilter, StringComparison.OrdinalIgnoreCase));
            }

            //tasksQuery = sortBy switch
            //{
            //    "CreatedAt" => tasksQuery.OrderBy(task => task.CreatedAt),
            //    "Status" => tasksQuery.OrderBy(task => task.Status),
            //    _ => tasksQuery.OrderBy(task => task.CreatedAt)
            //};

            var tasks = tasksQuery.ToList();

            ViewBag.StatusFilter = statusFilter;
            ViewBag.Search = search;

            return View(tasks);
        }

    }
}