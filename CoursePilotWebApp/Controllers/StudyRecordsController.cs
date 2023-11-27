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
using ModuleManagerCL;

namespace CoursePilotWebApp.Controllers
{
    public class StudyRecordsController : Controller
    {
        private readonly CoursePilotDbContext _context;
        ModuleManager manager = new ModuleManager();
        public StudyRecordsController(CoursePilotDbContext context)
        {
            _context = context;
        }

        // GET: StudyRecords
        // This method represents an asynchronous action that will be invoked when the 'Index' view is accessed
        public async Task<IActionResult> Index()
        {
            // Retrieves the user's ID from the claims associated with the current user
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

            // Retrieves a list of modules associated with the specified user ID from the database asynchronously
            List<Module> userModules = _context.Modules
                .Where(module => module.UserID == userId)
                .ToList();

            //Saves  the use user modules in a viewb ag to use in the index view
            
            ViewBag.Modules = userModules;
            ViewBag.UserId = userId;

            // Checks if the 'Modules' entity in the database is not null
            // If its not null, it will return the 'Index' view with the retrieved user modules
            // If it is null, it will return a problem response with a specific message
            return _context.StudyRecords != null ? 
                          View(await _context.StudyRecords.ToListAsync()) :
                          Problem("Entity set 'CoursePilotDbContext.studyRecords'  is null.");
        }

        // GET: StudyRecords/Details/5
        // This method represents an asynchronous action that will be invoked when the 'Details' view is accessed
        public async Task<IActionResult> Details(int? id)
        {
            // Checks if the provided 'id' is null or if the 'studyrecords' entity in the database is null
            // If its true, it will return a 'NotFound' response indicating that the data was not found
            if (id == null || _context.StudyRecords == null)
            {
                return NotFound();
            }

            // Attempts to retrieve a studyRecord with the specified 'id' from the database asynchronously
            var studyRecords = await _context.StudyRecords
                .FirstOrDefaultAsync(m => m.ID == id);
            if (studyRecords == null)
            {
                return NotFound();
            }
            // Returns the 'Details' view with the retrieved studyrecord
            return View(studyRecords);
        }

        // GET: StudyRecords/Create
        // This method represents an asynchronous action that will be invoked when the 'Create' view is accessed
        public IActionResult Create()
        {
            // Retrieves the user's ID from the claims associated with the current user
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

            // Retrieves a list of modules associated with the specified user ID from the database asynchronously
            List<Module> userModules = _context.Modules
                .Where(module => module.UserID == userId)
                .ToList();

            //Stores the usermodules in the viewBag to use in the create view
            ViewBag.Modules = userModules;

            return View();
        }

        // POST: StudyRecords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,ModuleCode,UserID,StudyDate,HoursStudied,selfStudyHoursleft,IdealHours")] StudyRecords studyRecord)
        {
            // Retrieves the user's ID from the claims associated with the current user
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            studyRecord.UserID = userId!;

            studyRecord.WeekFinished = false;

            //Stores the Modules that match the module code entered and the user id
            var module = _context.Modules.FirstOrDefault(m => m.ModuleCode == studyRecord.ModuleCode && m.UserID == userId);

            //This gets the date from the previously entered record that matches the users chosen module code
            var lastStudyDate = _context.StudyRecords
                .OrderByDescending(sr => sr.StudyDate)
                .Where(sr => sr.ModuleCode == studyRecord.ModuleCode)
                .FirstOrDefault();

            //If the date is not null, checks if the the current date entered and last study date are in different weeks
            if (lastStudyDate != null)
            {
                if (manager.CheckDifferentWeek(lastStudyDate.StudyDate, studyRecord.StudyDate))
                {
                    //If the dates are in different weeks, the previous weeks records are saved in the previousWeekRecords field
                    //by checking if those records are in the same week
                    var previousWeekRecords = _context.StudyRecords
                    .Where(sr => sr.UserID == userId && sr.ModuleCode == studyRecord.ModuleCode)
                    .ToList()  
                    .Where(sr => manager.CheckSameWeek(lastStudyDate.StudyDate, sr.StudyDate))
                    .ToList();

                    //Adds up all hours studied values to get the total hours studied for the previous week
                    var totalHoursStudied = previousWeekRecords.Sum(sr => sr.HoursStudied);

                    // Replaces previous week records with a single record
                    foreach (var record in previousWeekRecords)
                    {
                        _context.StudyRecords.Remove(record);
                    }

                    //Calculates the self study hours that are left
                    decimal selfStudyHoursLeft = (module!.selfStudyHoursPerWeek) - totalHoursStudied;

                    // Check if selfStudyHoursLeft is negative, and if so, sets it to 0
                    if (selfStudyHoursLeft < 0)
                    {
                        selfStudyHoursLeft = 0;
                    }

                    //If the weeks are different, then the previous weeks record combined into one and is added to the database
                    //The total hours studied is stored as the hours studied and the start of the week is stored in the studydate
                    //The week finished is also set to true to indicate that the user has completed the previous week
                    _context.StudyRecords.Add(new StudyRecords
                    {
                        ModuleCode = studyRecord.ModuleCode,
                        UserID = userId,
                        StudyDate = manager.GetStartOfWeek(lastStudyDate.StudyDate),
                        HoursStudied = totalHoursStudied,
                        IdealHours = module!.selfStudyHoursPerWeek,
                        selfStudyHoursleft = selfStudyHoursLeft,
                        WeekFinished = true
                    });

                    //If the weeks are different, the previous weeks data is entered into the graph data table.
                    // This means that the 
                    _context.GraphData.Add(new GraphData
                    {
                        ModuleCode = studyRecord.ModuleCode,
                        UserID = userId,
                        WeekStartDate = manager.GetStartOfWeek(lastStudyDate.StudyDate),
                        IdealHours = module!.selfStudyHoursPerWeek,
                        TotalHoursStudied = totalHoursStudied
                    });

                    //Sets the default values for the new study week
                    SetDefaultValues(studyRecord, module!.selfStudyHoursPerWeek);

                }
                //If the weeks are not different then the ideal hours is set the the modules selfStudyHoursPerWeek
                //and the selfStudyHoursleft is equal the the previous records hours - the current hours entered
                else
                {
                    studyRecord.IdealHours = module!.selfStudyHoursPerWeek;
                    studyRecord.selfStudyHoursleft = lastStudyDate.selfStudyHoursleft - studyRecord.HoursStudied;
                    if (studyRecord.selfStudyHoursleft < 0)
                    {
                        studyRecord.selfStudyHoursleft = 0;
                    }
                    _context.StudyRecords.Add(studyRecord);
                }
            }
            //If there is not a last study date, it means that the user has not entered any records for that module yet
            //and just sets the default values
            else
            {
                SetDefaultValues(studyRecord, module!.selfStudyHoursPerWeek);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //This methods sets the default values for the new week
        private void SetDefaultValues(StudyRecords studyRecord, decimal idealHours)
        {
            studyRecord.IdealHours = idealHours;
            studyRecord.selfStudyHoursleft = studyRecord.IdealHours - studyRecord.HoursStudied;
            _context.StudyRecords.Add(studyRecord);
        }


        // GET: StudyRecords/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.StudyRecords == null)
            {
                return NotFound();
            }

            var studyRecords = await _context.StudyRecords
                .FirstOrDefaultAsync(m => m.ID == id);
            if (studyRecords == null)
            {
                return NotFound();
            }

            return View(studyRecords);
        }

        // POST: StudyRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.StudyRecords == null)
            {
                return Problem("Entity set 'CoursePilotDbContext.studyRecords'  is null.");
            }
            var studyRecords = await _context.StudyRecords.FindAsync(id);
            if (studyRecords != null)
            {
                _context.StudyRecords.Remove(studyRecords);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudyRecordsExists(int id)
        {
          return (_context.StudyRecords?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
