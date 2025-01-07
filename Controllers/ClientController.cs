//using Microsoft.AspNetCore.Mvc;
//using MYGUYY.Data;
//using MYGUYY.Models;
//using System;

//namespace MYGUYY.Controllers
//{
//    public class ClientController : Controller
//    {
//        private readonly MYGUYYContext _context;

//        public ClientController(MYGUYYContext context)
//        {
//            _context = context;
//        }

//        // GET: Display the form for creating a new client
//        [HttpGet]
//        public IActionResult Create()
//        {
//            return View();
//        }

//        // POST: Handle client creation form submission
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public IActionResult Create(Client client)
//        {
//            if (ModelState.IsValid)
//            {
//                // Set the client status as active by default (or based on your logic)
//                client.IsActive = true;
//                client.CreatedAt = DateTime.Now;

//                _context.Clients.Add(client);
//                _context.SaveChanges();

//                // Redirect to TaskRequest Create page after successful client creation
//                return RedirectToAction("Create", "TaskRequest");
//            }

//            return View(client); // Return the view with errors if any
//        }
//    }
//}
