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
        // This method represents an asynchronous action that will be invoked when the 'Index' view is accessed
        public async Task<IActionResult> Index()
        {
            // Retrieve the user ID from the claims associated with the current user
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

            // Stores a list of modules associated with the specified user ID from the database
            List<Module> userModules = _context.Modules
                .Where(module => module.UserID == userId)
                .ToList();

            // Pass the list of user modules to the ViewBag, making it available to the index view
            ViewBag.Modules = userModules;

            // Checks if the GraphData entity in the database is not null
            // If its not null, it will retrieve the data and pass it to the view
            // If it is null, it will return a error message
            return _context.GraphData != null ?
                View(await _context.GraphData.ToListAsync()) :
                Problem("Entity set 'CoursePilotDbContext.GraphData' is null.");
        }


        // GET: GraphData/Details/5
        // This method represents an asynchronous action that will be invoked when the 'Details' endpoint is accessed.
        public async Task<IActionResult> Details(String? Code)
        {
            // Retrieves the user ID from the claims associated with the current user
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

            // Check if the provided Code is null or if the 'GraphData' entity in the database is null
            // If its true, it return a 'NotFound' response indicating that the data was not found
            if (Code == null || _context.GraphData == null)
            {
                return NotFound();
            }

            // Attempts to retrieve the graph data associated with the specified Code and user ID from the database
            var graphData = await _context.GraphData
                .FirstOrDefaultAsync(m => m.ModuleCode == Code && m.UserID == userId);

            // If no graph data is found, it redirects to the Index view
            // and displays a SweetAlert2 error message using TempData
            if (graphData == null)
            {
                TempData["ErrorMessage"] = "Looks like there's no study data available for this module yet. Don't worry! Start studying, and after at least one week, we'll have data to display. Keep up the good work!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // Passes the 'IdealHours' property of the graph data to the view using ViewBag
                ViewBag.IdealHours = graphData?.IdealHours;

                // Stores the Code in the session for later use
                HttpContext.Session.SetString("Code", Code);

                // Returns the 'Details' view with the retrieved graph data
                return View(graphData);
            }
        }


        // POST: GraphData/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        // This method retrieves the graph data for a specific module and user
        public List<object> GetGraphData()
        {
            // Retrieves the user ID from the claims associated with the current user
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

            // Retrieves the module code from the session
            string moduleCode = HttpContext.Session.GetString("Code");

            // Creates a list to store the data for the graph
            List<object> data = new List<object>();

            // Retrieves a list of unique WeekStartDate values for the specified module and user from the database
            List<DateTime> labels = _context.GraphData
                .Where(p => p.ModuleCode == moduleCode && p.UserID == userId)
                .Select(p => p.WeekStartDate.Date)
                .ToList();

            // Adds the list of WeekStartDate values to the data list
            data.Add(labels);

            // Retrieves a list of TotalHoursStudied values for the specified module and the user from the database
            List<decimal> hoursStudied = _context.GraphData
                .Where(p => p.ModuleCode == moduleCode && p.UserID == userId)
                .Select(p => p.TotalHoursStudied)
                .ToList();

            // Adds the list of TotalHoursStudied values to the data list
            data.Add(hoursStudied);

            // Returns the populated data list
            return data;
        }



    }
}
