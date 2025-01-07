//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore; // Add this to fix the Include issue
//using MYGUYY.Data;
//using MYGUYY.Models;

//namespace MYGUYY.Controllers
//{
//    public class PaymentController : Controller
//    {
//        private readonly MYGUYYContext _context;

//        public PaymentController(MYGUYYContext context)
//        {
//            _context = context;
//        }

//        public IActionResult Index()
//        {
//            // Use Include to eagerly load related TaskRequest data
//            var payments = _context.Payments.Include(p => p.TaskRequest).ToList();
//            return View(payments);
//        }

       

//        [HttpPost]
//        public IActionResult CompletePayment(int taskId)
//        {
//            var payment = new Payment
//            {
//                TaskRequestId = taskId,
//                Amount = _context.TaskRequests.Find(taskId)?.EstimatedCharge ?? 0,
//                PaymentStatus = "Completed"
//            };

//            _context.Payments.Add(payment);
//            _context.SaveChanges();
//            return RedirectToAction("Index");
//        }
//    }
//}
