//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using MYGUYY.Data;
//using MYGUYY.Models;
//using System.Linq;

//namespace MYGUYY.Controllers
//{
//    [Authorize(Roles = "Admin")]
//    public class UserController : Controller
//    {
//        private readonly MYGUYYContext _context;

//        public UserController(MYGUYYContext context)
//        {
//            _context = context;
//        }

//        // List all users
//        public IActionResult Index()
//        {
//            var users = _context.Users.ToList();
//            return View(users);
//        }

//        // Display details of a user
//        public IActionResult Details(int id)
//        {
//            var user = _context.Users.FirstOrDefault(u => u.Id == id);
//            if (user == null) return NotFound();
//            return View(user);
//        }

//        // Create a user (GET)
//        [HttpGet]
//        public IActionResult Create()
//        {
//            return View();
//        }

//        // Create a user (POST)
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public IActionResult Create(User user)
//        {
//            if (ModelState.IsValid)
//            {
//                _context.Users.Add(user);
//                _context.SaveChanges();
//                return RedirectToAction(nameof(Index));
//            }
//            return View(user);
//        }

//        // Edit a user (GET)
//        [HttpGet]
//        public IActionResult Edit(int id)
//        {
//            var user = _context.Users.FirstOrDefault(u => u.Id == id);
//            if (user == null) return NotFound();
//            return View(user);
//        }

//        // Edit a user (POST)
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public IActionResult Edit(int id, User updatedUser)
//        {
//            if (id != updatedUser.Id) return NotFound();

//            if (ModelState.IsValid)
//            {
//                _context.Users.Update(updatedUser);
//                _context.SaveChanges();
//                return RedirectToAction(nameof(Index));
//            }
//            return View(updatedUser);
//        }

//        // Deactivate a user
//        [HttpPost]
//        public IActionResult Deactivate(int id)
//        {
//            var user = _context.Users.FirstOrDefault(u => u.Id == id);
//            if (user == null) return NotFound();

//            user.IsActive = false;
//            _context.SaveChanges();
//            return RedirectToAction(nameof(Index));
//        }

//        // Reactivate a user
//        [HttpPost]
//        public IActionResult Reactivate(int id)
//        {
//            var user = _context.Users.FirstOrDefault(u => u.Id == id);
//            if (user == null) return NotFound();

//            user.IsActive = true;
//            _context.SaveChanges();
//            return RedirectToAction(nameof(Index));
//        }
//    }
//}