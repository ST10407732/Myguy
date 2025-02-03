using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MYGUYY.Data;
using MYGUYY.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MYGUYY.Controllers
{
    [Authorize(Roles = "Admin")] // Ensures only Admin can access this controller
    public class AdminController : Controller
    {
        private readonly MYGUYYContext _context;

        public AdminController(MYGUYYContext context)
        {
            _context = context;
        }

        // GET: Admin/ManageTasks
        public async Task<IActionResult> ManageTasks(string search = "", string statusFilter = "All")
        {
            // Get tasks based on filters
            var tasksQuery = _context.TaskRequests.Include(t => t.Client).Include(t => t.Driver)
                .AsQueryable();

            // Apply search filter if search text is provided
            if (!string.IsNullOrEmpty(search))
            {
                tasksQuery = tasksQuery.Where(t => t.Description.Contains(search) || t.Client.Username.Contains(search));
            }

            // Apply status filter if selected filter is not "All"
            if (statusFilter != "All")
            {
                tasksQuery = tasksQuery.Where(t => t.Status == statusFilter);
            }

            // Pagination (optional, if needed)
            var tasks = await tasksQuery.ToListAsync();

            // Pass search and statusFilter values to the view for retaining form values
            ViewBag.Search = search;
            ViewBag.StatusFilter = statusFilter;
            ViewBag.Status = statusFilter;

            return View(tasks);
        }

        // GET: Admin/AssignDriver/5
        public async Task<IActionResult> AssignDriver(int taskId)
        {
            var task = await _context.TaskRequests.Include(t => t.Client).FirstOrDefaultAsync(t => t.Id == taskId);
            if (task == null)
            {
                return NotFound();
            }

            // List all available drivers for assignment
            var drivers = await _context.Users.Where(u => u.Role == "Driver").ToListAsync();
            ViewBag.Drivers = drivers;

            return View(task);
        }

        // POST: Admin/AssignDriver/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignDriver(int taskId, int driverId)
        {
            var task = await _context.TaskRequests.FirstOrDefaultAsync(t => t.Id == taskId);
            if (task == null)
            {
                return NotFound();
            }

            task.DriverId = driverId;
            task.Status = "Accepted"; // Change status when driver is assigned
            task.AcceptedAt = DateTime.UtcNow; // Mark the time when the task was accepted

            _context.Update(task);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ManageTasks));
        }

        // GET: Admin/ViewTask/5
        public async Task<IActionResult> ViewTask(int taskId)
        {
            var task = await _context.TaskRequests.Include(t => t.Client).Include(t => t.Driver).FirstOrDefaultAsync(t => t.Id == taskId);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // GET: Admin/EditTask/5
        public async Task<IActionResult> EditTask(int taskId)
        {
            var task = await _context.TaskRequests.FirstOrDefaultAsync(t => t.Id == taskId);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // POST: Admin/EditTask/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTask(int taskId, [Bind("Id,Description,Status,PickupLatitude,PickupLongitude,DropoffLatitude,DropoffLongitude,AmountCollected,DriverConfirmationCode,IsAgreementConfirmed,Cost,VehicleType,RecommendedCost,ConfirmationCode")] TaskRequest task)
        {
            if (taskId != task.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(task);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.TaskRequests.Any(t => t.Id == task.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ManageTasks));
            }
            return View(task);
        }

        // GET: Admin/DeleteTask/5
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.TaskRequests.Include(t => t.Client).Include(t => t.Driver).FirstOrDefaultAsync(t => t.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // POST: Admin/DeleteTask/5
        [HttpPost, ActionName("DeleteTask")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task = await _context.TaskRequests.FindAsync(id);
            if (task != null)
            {
                _context.TaskRequests.Remove(task);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ManageTasks));
        }
    }
}
