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
using MYGUYY.Services;


namespace MYGUYY.Controllers
{
    using Microsoft.AspNetCore.SignalR;

    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.Options;
    using Microsoft.Win32;
    using MYGUYY.Models;
    using System.Net.Mail;
    using System.Net;
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
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly EmailService _emailService;

        // Combine all dependencies into one constructor
        public AccountController(MYGUYYContext context, IHubContext<NotificationHub> hubContext, EmailService emailService)
        {
            _context = context;
            _hubContext = hubContext;
            _emailService = emailService;
        }

        // Send verification email
        public IActionResult DriverTasks()
        {
            // Retrieve the driver's ID from the logged-in user's claims
            var driverId = int.Parse(User.FindFirst("UserId")?.Value);

            // Check if the driverId is valid
            if (driverId <= 0)
            {
                // Handle invalid driverId (e.g., redirect or show an error message)
                return RedirectToAction("Index", "Home"); // Or show an error view
            }

            // Fetch tasks assigned to the driver that are not completed
            var tasks = _context.TaskRequests
                                .Where(t => t.DriverId == driverId && t.Status != "Completed")
                                .OrderBy(t => t.CreatedAt) // Optional: Sort tasks by creation date
                                .ToList();

            // Handle case where no tasks are found
            if (tasks == null || tasks.Count == 0)
            {
                ViewBag.Message = "You have no active tasks at the moment.";
            }

            return View(tasks); // Return a view with the list of tasks assigned to the driver
        }
        public IActionResult TrackDriver(int taskId)
        {
            var taskRequest = _context.TaskRequests
                .Include(t => t.Driver)
                .FirstOrDefault(t => t.Id == taskId);

            if (taskRequest == null)
            {
                return NotFound();
            }

            return View(taskRequest); // Pass the task request to the view
        }
        [Authorize]
        public async Task<IActionResult> Directions(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid task ID.");
            }

            var task = await _context.TaskRequests.FindAsync(id);
            if (task == null)
            {
                return NotFound("Task not found.");
            }

            return View(task);
        }


        // GET: Render the registration page
        public IActionResult TaskGroup()
        {
            // Set ViewData for title
            ViewData["Title"] = "Task Groups";
            return View();  // Return the view
        }
        public IActionResult TrackTask()
        {
            // Get the logged-in user's ID (assuming User.Identity.Name stores the email or ID)
            int userId = Convert.ToInt32(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

            if (userId == 0)
            {
                return Unauthorized("User not logged in.");
            }

            // Find the task assigned to the logged-in user (either as a driver or client)
            var task = _context.TaskRequests
                        .FirstOrDefault(t => t.ClientId == userId || t.DriverId == userId);

            if (task == null)
            {
                return NotFound("No task found for this user.");
            }

            // Pass task locations to the view using ViewBag
            ViewBag.PickupLat = task.PickupLatitude;
            ViewBag.PickupLng = task.PickupLongitude;
            ViewBag.DropoffLat = task.DropoffLatitude;
            ViewBag.DropoffLng = task.DropoffLongitude;
            ViewBag.DriverLat = task.DriverLatitude ?? 0;
            ViewBag.DriverLng = task.DriverLongitude ?? 0;

            return View();
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
        public async Task<IActionResult> CreateTask(TaskRequest taskRequest, List<Stop> stops)
        {
            try
            {
                // Set ClientId and other properties as before
                taskRequest.ClientId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                taskRequest.Status = string.IsNullOrEmpty(taskRequest.Status) ? "Pending" : taskRequest.Status;
                taskRequest.DriverId = taskRequest.DriverId ?? null;

                // Validate fields
                if (string.IsNullOrWhiteSpace(taskRequest.Description))
                {
                    ModelState.AddModelError("Description", "Task description is required.");
                }
                if (taskRequest.PickupLatitude == 0 || taskRequest.PickupLongitude == 0)
                {
                    ModelState.AddModelError("PickupLocation", "Pick-up location is invalid.");
                }
                if (taskRequest.DropoffLatitude == 0 || taskRequest.DropoffLongitude == 0)
                {
                    ModelState.AddModelError("DropoffLocation", "Drop-off location is invalid.");
                }
                if (stops != null && stops.Any(s => s.Latitude == 0 || s.Longitude == 0))
                {
                    ModelState.AddModelError("Stops", "All stops must have valid coordinates.");
                }

                if (!ModelState.IsValid)
                {
                    // Collect validation errors and return to view
                    ViewBag.ValidationErrors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                    return View(taskRequest);
                }

                // Generate a unique confirmation code for the task
                taskRequest.ConfirmationCode = GenerateUniqueConfirmationCode();

                // Add stops to the task
                taskRequest.Stops = stops?.Select((s, index) => new Stop
                {
                    Latitude = s.Latitude,
                    Longitude = s.Longitude,
                    Order = index + 1
                }).ToList();

                // Calculate task cost using helper methods
                double totalDistance = CalculateTotalDistance(taskRequest.PickupLatitude, taskRequest.PickupLongitude, taskRequest.DropoffLatitude, taskRequest.DropoffLongitude, stops);
                double cost = totalDistance * GetRateForVehicle(taskRequest.VehicleType);

                taskRequest.Cost = cost;
                taskRequest.CreatedAt = DateTime.Now;

                // Save the task to the database
                _context.TaskRequests.Add(taskRequest);
                await _context.SaveChangesAsync();

                // Create notification message
                string notificationMessage = $"A new task has been created: {taskRequest.Description}";

                // Save notifications for all users (drivers and clients)
                var users = await _context.Users.Where(u => u.Role == "Driver" || u.Role == "Client").ToListAsync();
                List<Notification> notifications = new List<Notification>();

                foreach (var user in users)
                {
                    notifications.Add(new Notification
                    {
                        UserId = user.Id,  // Using UserId instead of DriverId
                        Message = notificationMessage,
                        IsRead = false,
                        CreatedAt = DateTime.Now,
                        TaskId = taskRequest.Id
                    });
                }

                // Attempt to save notifications
                _context.Notifications.AddRange(notifications);
                var notificationSaveResult = await _context.SaveChangesAsync();

                // Check if notifications were successfully saved
                if (notificationSaveResult == 0)
                {
                    ModelState.AddModelError(string.Empty, "Task was created, but notifications could not be saved.");
                    return View(taskRequest);
                }

                // Attempt to send notifications via SignalR (updated for UserId)
                try
                {
                    await _hubContext.Clients.All.SendAsync("ReceiveNotification", notificationMessage);
                    // If notifications are sent successfully, show a success message
                    TempData["Notification"] = "Task created successfully. All users (drivers and clients) have been notified.";
                }
                catch (Exception ex)
                {
                    // If SignalR fails, show an error message for the real-time notifications
                    ModelState.AddModelError(string.Empty, "Task was created, but real-time notifications failed to send.");
                    TempData["Error"] = "Task was created, but real-time notifications could not be sent.";
                    return View(taskRequest);
                }

                // Successfully created task and sent notifications
                return RedirectToAction("ClientTasks");
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes (optional)
                // _logger.LogError(ex, "Error creating task");

                TempData["Error"] = "An error occurred while creating the task. Please try again later.";
                return RedirectToAction("ClientTasks");
            }
        }


        // Helper methods to calculate total distance
        private double CalculateTotalDistance(double pickupLat, double pickupLng, double dropoffLat, double dropoffLng, List<Stop> stops)
        {
            double totalDistance = 0;

            // Calculate distance from pickup to first stop (if stops exist)
            if (stops != null && stops.Any())
            {
                totalDistance += CalculateDistance(pickupLat, pickupLng, stops.First().Latitude, stops.First().Longitude);
            }

            // Calculate distance between stops
            for (int i = 0; i < stops.Count - 1; i++)
            {
                totalDistance += CalculateDistance(stops[i].Latitude, stops[i].Longitude, stops[i + 1].Latitude, stops[i + 1].Longitude);
            }

            // Calculate distance from last stop to dropoff
            if (stops != null && stops.Any())
            {
                totalDistance += CalculateDistance(stops.Last().Latitude, stops.Last().Longitude, dropoffLat, dropoffLng);
            }

            return totalDistance;
        }

        // Helper method to calculate distance between two points in kilometers
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Radius of the Earth in km
            double dLat = (lat2 - lat1) * Math.PI / 180;
            double dLon = (lon2 - lon1) * Math.PI / 180;

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = R * c;

            return distance; // Returns distance in kilometers
        }

        // Get rate for vehicle type
        private double GetRateForVehicle(string vehicleType)
        {
            switch (vehicleType)
            {
                case "Bike": return 1.0; // Rate per km for bike
                case "Car": return 1.5; // Rate per km for car
                case "Van": return 2.0; // Rate per km for van
                default: return 1.0;
            }
        }

        // Helper method to generate a unique confirmation code
        private string GenerateUniqueConfirmationCode()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 8); // Example: 8-character unique code
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
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> TaskRequestsForDriver()
        {
            try
            {
                var driverId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

                if (driverId == 0)
                {
                    TempData["Error"] = "Invalid driver ID.";
                    return RedirectToAction("Index", "Home");
                }

                // Fetch unread notifications for the logged-in driver (where IsRead is false)
                var unreadNotifications = await _context.Notifications
                    .Where(n => n.UserId == driverId && !n.IsRead) // Ensure notifications are unread
                    .OrderByDescending(n => n.CreatedAt) // Order by the latest notification
                    .ToListAsync();

                // Mark notifications as read after fetching
                foreach (var notification in unreadNotifications)
                {
                    notification.IsRead = true;
                }
                await _context.SaveChangesAsync();

                // Passing the unread notifications to the view
                ViewBag.Notifications = unreadNotifications;

                // Fetching task requests for the driver (Pending or Accepted tasks)
                var tasks = await _context.TaskRequests
                    .Where(t => t.DriverId == driverId || t.Status == "Pending") // Filter tasks for the driver
                    .Include(t => t.Client) // Include client details
                    .ToListAsync();

                // Fetch the most recent task for the driver
                var recentTask = await _context.TaskRequests
                    .Where(t => t.DriverId == driverId)
                    .OrderByDescending(t => t.CreatedAt)
                    .FirstOrDefaultAsync();

                ViewBag.RecentTask = recentTask;

                return View(tasks);
            }
            catch (Exception ex)
            {
                //// Log the exception for debugging purposes
                //_logger.LogError(ex, "An error occurred while fetching task requests for driver.");

                TempData["Error"] = "An error occurred while fetching task requests.";
                return RedirectToAction("Index", "Home");
            }
        }


        //[Authorize(Roles = "Driver")]
        //public async Task<IActionResult> AcceptTask(int id)
        //{
        //    var task = await _context.TaskRequests.FirstOrDefaultAsync(t => t.Id == id);

        //    if (task == null || task.DriverId != null || task.Status != "Pending")
        //    {
        //        TempData["Message"] = task == null ? "Task not found." : "Task already accepted or invalid status.";
        //        return RedirectToAction("TaskRequestsForDriver");
        //    }

        //    task.DriverId = int.Parse(User.FindFirst("UserId").Value);
        //    task.Status = "Accepted";
        //    await _context.SaveChangesAsync();

        //    TempData["Message"] = "Task accepted.";
        //    return RedirectToAction("ViewMessagesForDriver", new { taskId = task.Id });
        //}
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
            return RedirectToAction("AgreementForm", new { taskId = task.Id });
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

            return RedirectToAction("AgreementForm");
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

        [HttpGet]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> AgreementForm(int taskId)
        {
            var driverId = int.Parse(User.FindFirst("UserId").Value);

            var task = await _context.TaskRequests
                .Include(t => t.Client) // Include client info for display
                .FirstOrDefaultAsync(t => t.Id == taskId && t.DriverId == driverId && t.Status == "Accepted");

            if (task == null)
            {
                return NotFound("Task not found or not assigned to you.");
            }

            return View(task);
        }
        [HttpPost]
        public IActionResult ConfirmAgreement(TaskRequest model)
        {
            if (!ModelState.IsValid)
            {
                // Handle invalid model state
                ViewBag.ErrorMessage = "Please ensure all fields are filled correctly.";
                return View(model);
            }

            // Fetch the task from the database
            var task = _context.TaskRequests
                .FirstOrDefault(t => t.Id == model.Id);

            if (task == null)
            {
                ViewBag.ErrorMessage = "Task not found.";
                return View(model);
            }

            // Validate the confirmation code
            if (model.ConfirmationCode != task.ConfirmationCode)
            {
                ViewBag.ErrorMessage = "The confirmation code does not match. Please try again.";
                return View(model);
            }

            // Ensure the collected amount is greater than or equal to the recommended cost
            if (model.AmountCollected < task.RecommendedCost)
            {
                ViewBag.ErrorMessage = $"The amount collected cannot be less than the recommended cost of R{task.RecommendedCost}.";
                return View(model);
            }

            // Update the task details
            task.AmountCollected = model.AmountCollected;
            task.Status = "Confirmed"; // Mark the task as confirmed
            task.StartedAt = DateTime.Now; // Record the confirmation time

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Agreement successfully confirmed.";
            return RedirectToAction("TaskDetails", new { id = task.Id });
        }

        [HttpPost]
        [Authorize(Roles = "Driver")]
        public IActionResult StartTrip(int taskId)
        {
            var driverId = int.Parse(User.FindFirst("UserId").Value);

            var task = _context.TaskRequests.FirstOrDefault(t => t.Id == taskId && t.DriverId == driverId && t.Status == "Confirmed");

            if (task == null)
            {
                ViewBag.ErrorMessage = "Task not found or not assigned to you.";
                return View();
            }

            // Assuming that the task's status is updated when the trip starts
            task.Status = "In Progress"; // Update task status
            task.StartedAt = DateTime.Now; // Set the start time of the trip

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while starting the trip. Please try again.";
                return View();
            }

            TempData["SuccessMessage"] = "Trip started successfully.";
            return RedirectToAction("DriverTasks"); // Redirect to DriverTasks page
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
        // POST: Admin/DeleteUser
        // POST: Admin/DeleteTask
        // POST: Admin/UpdateTaskStatus
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateTaskStatus(int id, string status)
        {
            var task = _context.TaskRequests.Find(id);
            if (task != null)
            {
                task.Status = status;
                _context.SaveChanges(); // Save the status update to the database
            }

            return RedirectToAction("ManageTasks"); // Redirect back to the tasks page
        }
        // POST: Admin/DeleteTask
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteTask(int id)
        {
            var task = _context.TaskRequests.Find(id);
            if (task != null)
            {
                _context.TaskRequests.Remove(task);
                _context.SaveChanges(); // Commit the changes to the database
            }

            return RedirectToAction("ManageTasks"); // Redirect back to the tasks page
        }

    }
}