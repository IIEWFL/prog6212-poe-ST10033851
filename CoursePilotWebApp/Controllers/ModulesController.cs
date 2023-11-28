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
//Pathan, F. (27 Aug 2019). How To Get Current User Claims In ASP.NET Identity. [online] www.c-sharpcorner.com.
//Available at: https://www.c-sharpcorner.com/blogs/how-to-get-current-user-claims-in-asp-net-identity [Accessed 12 Oct. 2023].
//mjrousos (n.d.). Securing .NET Microservices and Web Applications. [online] learn.microsoft.com.
//Available at: https://learn.microsoft.com/en-us/dotnet/architecture/microservices/secure-net-microservices-web-applications/. [Accessed 12 Oct. 2023].

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
        // This method represents an asynchronous action that will be invoked when the 'Index' endpoint is accessed
        public async Task<IActionResult> Index()
        {
            // Retrieves the user's ID from the claims associated with the current user
            string currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Retrieves a list of modules associated with the specified user ID from the database asynchronously
            List<Module> userModules = await _context.Modules.Where(m => m.UserID == currentUserId).ToListAsync();

            // Checks if the 'Modules' entity set in the database is not null
            // If its not null, it will return the 'Index' view with the retrieved user modules
            // If it is null, it will return a problem response with a specific message
            return _context.Modules != null ?
                View(userModules) :
                Problem("Entity set 'CoursePilotDbContext.Modules' is null.");
        }


        // GET: Modules/Details/5
        // This method represents an asynchronous action that will be invoked when the 'Details' endpoint is accessed
        public async Task<IActionResult> Details(int? id)
        {
            // Checks if the provided 'id' is null or if the 'Modules' entity in the database is null
            // If its true, it will return a 'NotFound' response indicating that the data was not found
            if (id == null || _context.Modules == null)
            {
                return NotFound();
            }

            // Attempts to retrieve a module with the specified 'id' from the database asynchronously
            var @module = await _context.Modules
                .FirstOrDefaultAsync(m => m.ID == id);

            // If no module was found for the specified 'id', return a 'NotFound' error
            if (@module == null)
            {
                return NotFound();
            }

            // Returns the 'Details' view with the retrieved module
            return View(@module);
        }


        // GET: Modules/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Modules/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        // This method represents an asynchronous action that will be invoked when the 'Create' endpoint is accessed
        public async Task<IActionResult> Create([Bind("UserID,ModuleCode,moduleName,credits,weeklyClassHours,weeksInSem,startDate,selfStudyHoursPerWeek")] Module @module)
        {
            // Calculates the  self-study hours using the provided input values from the user
            @module.selfStudyHoursPerWeek = moduleManager.calculateSelfStudyHours(@module.credits, @module.weeksInSem, @module.weeklyClassHours);

            // Checks if the calculated self-study hours are negative
            if (@module.selfStudyHoursPerWeek < 0)
            {
                // Sets an error message in TempData if negative self-study hours are calculated
                TempData["ErrorMessage"] = "The entered values result in negative self-study hours. Please review your input and make sure it is correct.";
            }
            else
            {
                // Sets the user ID for the module to the current user's ID
                @module.UserID = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                // Adds the module to the context and save changes to the database asynchronously
                _context.Add(@module);
                await _context.SaveChangesAsync();
            }

            // Redirect to the 'Index' view
            return RedirectToAction(nameof(Index));
        }


        // GET: Modules/Delete/5
        // This method represents an asynchronous action that will be invoked when the 'Delete' view is accessed
        public async Task<IActionResult> Delete(int? id)
        {
            // Checks if the provided 'id' is null or if the 'Modules' entity set in the database is null
            // If its true, then it will return a 'NotFound' response indicating that the data was not found
            if (id == null || _context.Modules == null)
            {
                return NotFound();
            }

            // Attempts to retrieve a module with the specified 'id' from the database asynchronously
            var @module = await _context.Modules
                .FirstOrDefaultAsync(m => m.ID == id);

            // If no module is found for the specified 'id', return a 'NotFound' response
            if (@module == null)
            {
                return NotFound();
            }

            // Returns the 'Delete' view with the retrieved module for confirmation
            return View(@module);
        }


        // POST: Modules/Delete/5
        //This method is used to check if the data has been deleted from the database and returns an error message if the data data was null
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
