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
        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

            List<Module> userModules = _context.Modules
                .Where(module => module.UserID == userId)
                .ToList();

            ViewBag.Modules = userModules;

            return _context.StudyRecords != null ? 
                          View(await _context.StudyRecords.ToListAsync()) :
                          Problem("Entity set 'CoursePilotDbContext.studyRecords'  is null.");
        }

        // GET: StudyRecords/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: StudyRecords/Create
        public IActionResult Create()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

            List<Module> userModules = _context.Modules
                .Where(module => module.UserID == userId)
                .ToList();

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
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            studyRecord.UserID = userId!;

            studyRecord.WeekFinished = false;

            var module = _context.Modules.FirstOrDefault(m => m.ModuleCode == studyRecord.ModuleCode && m.UserID == userId);

            var lastStudyDate = _context.StudyRecords
                .OrderByDescending(sr => sr.StudyDate)
                .Where(sr => sr.ModuleCode == studyRecord.ModuleCode)
                .FirstOrDefault();

            if (lastStudyDate != null)
            {
                if (manager.CheckDifferentWeek(lastStudyDate.StudyDate, studyRecord.StudyDate))
                {
                    var previousWeekRecords = _context.StudyRecords
                    .Where(sr => sr.UserID == userId && sr.ModuleCode == studyRecord.ModuleCode)
                    .ToList()  
                    .Where(sr => manager.CheckSameWeek(lastStudyDate.StudyDate, sr.StudyDate))
                    .ToList();

                    var totalHoursStudied = previousWeekRecords.Sum(sr => sr.HoursStudied);

                    // Replace previous week records with a single record
                    foreach (var record in previousWeekRecords)
                    {
                        _context.StudyRecords.Remove(record);
                    }

                    decimal selfStudyHoursLeft = (module!.selfStudyHoursPerWeek) - totalHoursStudied;

                    // Check if selfStudyHoursLeft is negative, and if so, set it to 0
                    if (selfStudyHoursLeft < 0)
                    {
                        selfStudyHoursLeft = 0;
                    }

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

                    _context.GraphData.Add(new GraphData
                    {
                        ModuleCode = studyRecord.ModuleCode,
                        UserID = userId,
                        WeekStartDate = manager.GetStartOfWeek(lastStudyDate.StudyDate),
                        IdealHours = module!.selfStudyHoursPerWeek,
                        TotalHoursStudied = totalHoursStudied
                    });

                    SetDefaultValues(studyRecord, module!.selfStudyHoursPerWeek);

                }
                else
                {
                    studyRecord.IdealHours = module!.selfStudyHoursPerWeek;
                    studyRecord.selfStudyHoursleft = lastStudyDate.selfStudyHoursleft - studyRecord.HoursStudied;
                    _context.StudyRecords.Add(studyRecord);
                }
            }
            else
            {
                SetDefaultValues(studyRecord, module!.selfStudyHoursPerWeek);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

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
