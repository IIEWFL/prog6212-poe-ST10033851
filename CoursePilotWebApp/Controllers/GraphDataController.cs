using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CoursePilotWebApp.Data;
using CoursePilotWebApp.Models.Domain;
using System.Security.Claims;

namespace CoursePilotWebApp.Controllers
{
    public class GraphDataController : Controller
    {
        private readonly CoursePilotDbContext _context;

        public GraphDataController(CoursePilotDbContext context)
        {
            _context = context;
        }

        // GET: GraphData
        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

            List<Module> userModules = _context.Modules
                .Where(module => module.UserID == userId)
                .ToList();

            ViewBag.Modules = userModules;

            return _context.GraphData != null ? 
                          View(await _context.GraphData.ToListAsync()) :
                          Problem("Entity set 'CoursePilotDbContext.GraphData'  is null.");
        }

        // GET: GraphData/Details/5
        public async Task<IActionResult> Details(String? Code)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

            if (Code == null || _context.GraphData == null)
            {
                return NotFound();
            }

            var graphData = await _context.GraphData
                .FirstOrDefaultAsync(m => m.ModuleCode == Code && m.UserID == userId);
            if (graphData == null)
            {
                // Display SweetAlert2 error using JavaScript
                TempData["ErrorMessage"] = "Looks like there's no study data available for this module yet. Don't worry! Start studying, and after at least one week, we'll have data to display. Keep up the good work!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // Pass IdealHours to the view
                ViewBag.IdealHours = graphData?.IdealHours;

                HttpContext.Session.SetString("Code", Code);

                return View(graphData);
            }

            
        }

        // POST: GraphData/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public List<object> GetGraphData()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            string moduleCode = HttpContext.Session.GetString("Code");

            //return data;
            List<object> data = new List<object>();
            data.Clear();
            List<DateTime> labels = _context.GraphData
            .Where(p => p.ModuleCode == moduleCode && p.UserID == userId)
            .Select(p => p.WeekStartDate.Date)
            .ToList();
            data.Add(labels);

            List<decimal> HoursStudied = _context.GraphData
                .Where(p => p.ModuleCode == moduleCode && p.UserID == userId)
                .Select(p => p.TotalHoursStudied)
                .ToList();
            data.Add(HoursStudied);

            return data;

        }

        
    }
}
