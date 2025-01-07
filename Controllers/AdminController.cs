//using Microsoft.AspNetCore.Mvc;
//using MYGUYY.Data;

//namespace MYGUYY.Controllers
//{
//    public class AdminController : Controller
//    {
//        private readonly MYGUYYContext _context;

//        public AdminController(MYGUYYContext context)
//        {
//            _context = context;
//        }

//        public IActionResult ManageUsers()
//        {
//            var users = _context.Users.ToList();
//            return View(users);
//        }

//        public IActionResult ManageTasks()
//        {
//            var tasks = _context.TaskRequests.ToList();
//            return View(tasks);
//        }

//        public IActionResult DeactivateUser(int userId)
//        {
//            var user = _context.Users.Find(userId);
//            if (user != null)
//            {
//                user.IsActive = false;
//                _context.SaveChanges();
//            }

//            return RedirectToAction("ManageUsers");
//        }
//    }
//}
