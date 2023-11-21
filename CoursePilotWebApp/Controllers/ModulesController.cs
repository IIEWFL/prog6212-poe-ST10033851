using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CoursePilotWebApp.Data;
using CoursePilotWebApp.Models.Domain;
using ModuleManagerCL;
using System.Security.Claims;

namespace CoursePilotWebApp.Controllers
{
    public class ModulesController : Controller
    {
        private ModuleManager moduleManager = new ModuleManager();
        private readonly CoursePilotDbContext _context;

        public ModulesController(CoursePilotDbContext context)
        {
            _context = context;
        }

        // GET: Modules
        public async Task<IActionResult> Index()
        {

            string currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            List<Module> userModules = await _context.Modules.Where(m => m.UserID == currentUserId).ToListAsync();

            return _context.Modules != null ? 
                          View(userModules) :
                          Problem("Entity set 'CoursePilotDbContext.Modules'  is null.");
        }

        // GET: Modules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Modules == null)
            {
                return NotFound();
            }

            var @module = await _context.Modules
                .FirstOrDefaultAsync(m => m.ID == id);
            if (@module == null)
            {
                return NotFound();
            }

            return View(@module);
        }

        // GET: Modules/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Modules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserID,ModuleCode,moduleName,credits,weeklyClassHours,weeksInSem,startDate,selfStudyHoursPerWeek")] Module @module)
        {
            
            @module.selfStudyHoursPerWeek = moduleManager.calculateSelfStudyHours(@module.credits, @module.weeksInSem, @module.weeklyClassHours);

            if (@module.selfStudyHoursPerWeek < 0 )
            {
                TempData["ErrorMessage"] = "The entered values result in negative self study hours. Please review your input and make sure it is correct.";
            }
            else
            {
                @module.UserID = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                _context.Add(@module);
                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction(nameof(Index));
            
        }

        // GET: Modules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Modules == null)
            {
                return NotFound();
            }

            var @module = await _context.Modules
                .FirstOrDefaultAsync(m => m.ID == id);
            if (@module == null)
            {
                return NotFound();
            }

            return View(@module);
        }

        // POST: Modules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Modules == null)
            {
                return Problem("Entity set 'CoursePilotDbContext.Modules'  is null.");
            }
            var @module = await _context.Modules.FindAsync(id);
            if (@module != null)
            {
                _context.Modules.Remove(@module);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ModuleExists(int id)
        {
          return (_context.Modules?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
