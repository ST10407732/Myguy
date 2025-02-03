//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using MYGUYY.Data;
//using MYGUYY.Models;
//using System.IO;
//using System.Threading.Tasks;

//namespace MYGUYY.Controllers
//{
//    public class DriverProfileController : Controller
//    {
//        private readonly MYGUYYContext _context;

//        public DriverProfileController(MYGUYYContext context)
//        {
//            _context = context;
//        }

//        public async Task<IActionResult> Index()
//        {
//            var profiles = await _context.DriverProfiles.ToListAsync();
//            return View(profiles);
//        }

//        public IActionResult Create()
//        {
//            return View();
//        }

//        [HttpPost]
//        public async Task<IActionResult> Create(DriverProfile profile, IFormFile LicenseDocument, IFormFile IDDocument)
//        {
//            if (ModelState.IsValid)
//            {
//                if (LicenseDocument != null && LicenseDocument.Length > 0)
//                {
//                    var licenseFilePath = Path.Combine("wwwroot/uploads", LicenseDocument.FileName);
//                    using (var stream = new FileStream(licenseFilePath, FileMode.Create))
//                    {
//                        await LicenseDocument.CopyToAsync(stream);
//                    }
//                    profile.LicenseDocumentPath = "/uploads/" + LicenseDocument.FileName;
//                }
//                else
//                {
//                    ViewData["LicenseError"] = "License document is required.";
//                    return View(profile);
//                }

//                if (IDDocument != null && IDDocument.Length > 0)
//                {
//                    var idFilePath = Path.Combine("wwwroot/uploads", IDDocument.FileName);
//                    using (var stream = new FileStream(idFilePath, FileMode.Create))
//                    {
//                        await IDDocument.CopyToAsync(stream);
//                    }
//                    profile.IDDocumentPath = "/uploads/" + IDDocument.FileName;
//                }
//                else
//                {
//                    ViewData["IDError"] = "ID document is required.";
//                    return View(profile);
//                }

//                _context.DriverProfiles.Add(profile);
//                await _context.SaveChangesAsync();
//                return RedirectToAction(nameof(Index));
//            }
//            return View(profile);
//        }

//        public async Task<IActionResult> Edit(int id)
//        {
//            var profile = await _context.DriverProfiles.FindAsync(id);
//            if (profile == null)
//                return NotFound();
//            return View(profile);
//        }

//        [HttpPost]
//        public async Task<IActionResult> Edit(DriverProfile profile)
//        {
//            if (ModelState.IsValid)
//            {
//                _context.DriverProfiles.Update(profile);
//                await _context.SaveChangesAsync();
//                return RedirectToAction(nameof(Index));
//            }
//            return View(profile);
//        }
//    }
//}
