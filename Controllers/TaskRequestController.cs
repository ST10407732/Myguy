//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using MYGUYY.Data;
//using MYGUYY.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace MYGUYY.Controllers
//{
//    [Authorize(Roles = "Client")] // Ensure only clients can access this controller
//    public class TaskRequestController : Controller
//    {
//        private readonly MYGUYYContext _context;

//        public TaskRequestController(MYGUYYContext context)
//        {
//            _context = context;
//        }

//        // GET: Display the list of tasks (Index)
//        [HttpGet]
//        public IActionResult Index()
//        {
//            // Get the ClientId from the logged-in user's claims
//            var clientIdClaim = User.FindFirst("ClientId");
//            if (clientIdClaim == null)
//            {
//                return Unauthorized(); // User not authorized
//            }

//            if (!int.TryParse(clientIdClaim.Value, out int clientId))
//            {
//                return BadRequest("Invalid ClientId"); // Handle invalid ClientId
//            }

//            // Fetch all tasks for the logged-in client
//            var tasks = _context.TaskRequests
//                                .Where(t => t.ClientId == clientId)
//                                .OrderByDescending(t => t.CreatedAt)
//                                .ToList();

//            if (!tasks.Any())
//            {
//                ViewData["Message"] = "No tasks found.";
//            }

//            return View(tasks); // Return the list of tasks to the view
//        }

//        // GET: Display the form for creating a new task
//        [HttpGet]
//        public IActionResult Create()
//        {
//            // Populate the dropdown list for TaskTypes
//            ViewData["TaskTypes"] = new List<string> { "Shopping", "Delivery", "Custom" };
//            return View(); // Return the view to create a new task
//        }

//        // POST: Handle task creation
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public IActionResult Create(TaskRequest taskRequest)
//        {
//            // Check if the user is authorized
//            var clientIdClaim = User.FindFirst("ClientId");
//            if (clientIdClaim == null)
//            {
//                return Unauthorized(); // Prevent unauthorized access
//            }

//            if (!int.TryParse(clientIdClaim.Value, out int clientId))
//            {
//                return BadRequest("Invalid ClientId"); // Handle invalid ClientId
//            }

//            // Set the ClientId for the taskRequest from the user's claim
//            taskRequest.ClientId = clientId;
//            taskRequest.Status = "Pending"; // Default status
//            taskRequest.CreatedAt = DateTime.Now; // Set the creation timestamp

//            // Check if the model state is valid
//            if (ModelState.IsValid)
//            {
//                // Add the taskRequest to the database and save changes
//                _context.TaskRequests.Add(taskRequest);
//                _context.SaveChanges();

//                // Set a success message in TempData
//                TempData["SuccessMessage"] = "Task created successfully!";
//                return RedirectToAction("Index"); // Redirect to the Index page after successful creation
//            }

//            // If the validation fails, reload the view with task types
//            ViewData["TaskTypes"] = new List<string> { "Shopping", "Delivery", "Custom" };
//            return View(taskRequest); // Return to the Create view with validation errors
//        }
//    }
//}
