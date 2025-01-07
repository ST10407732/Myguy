//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using MYGUYY.Data;  // Ensure that the Data namespace is imported
//using MYGUYY.Models; // Ensure the Models namespace is imported

//namespace MYGUYY.Controllers
//{
//    public class UserTaskController : Controller
//    {
//        private readonly MYGUYYContext _context;

//        // Constructor to inject the DbContext
//        public UserTaskController(MYGUYYContext context)
//        {
//            _context = context;
//        }

//        // Index action to display all UserTasks
//        public IActionResult Index()
//        {
//            //// Fetch all UserTasks from the database
//            var userTasks = _context.UserTasks.ToList();

//            // Pass the tasks to the view
//            return View(userTasks);
//        }

//        // Create action to show the creation form
//        public IActionResult Create()
//        {
//            return View();
//        }

//        // Post action to handle the form submission
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public IActionResult Create(UserTask userTask)
//        {
//            if (ModelState.IsValid)
//            {
//                // Add the new task to the database
//                _context.UserTasks.Add(userTask);
//                _context.SaveChanges();

//                // Redirect to the Index view
//                return RedirectToAction(nameof(Index));
//            }

//            // If model validation fails, return to the create view with the invalid model
//            return View(userTask);
//        }
//    }
//}
