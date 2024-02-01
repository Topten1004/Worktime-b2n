using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Worktime.Global;
using Worktime.Models;
using Worktime.ViewModel;
using static Worktime.Global.Droits;

namespace Worktime.Controllers
{

    [Authorize(Roles = nameof(Droits.Roles.MANAGER))]

    public class DailySummaryController : Controller
    {
        private readonly WorktimeDbContext _context;

        public DailySummaryController(WorktimeDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(DateTime majBeginDate, string employeeName)
        {
            if (majBeginDate == DateTime.MinValue)
            {
                if (HttpContext.Session.GetString("majBeginDate") == null) majBeginDate = DateTime.Now.Date;
                else majBeginDate = Convert.ToDateTime(HttpContext.Session.GetString("majBeginDate"));
            }

            if(employeeName == null)
            {
                employeeName = string.Empty;
            }

            HttpContext.Session.SetString("employeeName", employeeName);
            HttpContext.Session.SetString("majBeginDate", majBeginDate.Date.ToString("yyyy-MM-dd"));
            ViewBag.majBeginDate = majBeginDate;
            ViewBag.employeeName = employeeName;

            var result = new MajVM();

            var model = await _context.Employees.Where(l => l.Enable == false).Include(l => l.Passages).ThenInclude(l => l.Pointer).Where(x => x.ReleaseDate == null).OrderBy(x => x.LastName).ToListAsync();

            foreach (var item in model)
            {
                int count = 0;
                int type = 1;
                int absence = 0;
                var newSummary = new SummaryVM();

                var dailyPassages = item.Passages.Where(x => x.LogTime.Date == majBeginDate.Date).OrderBy(x => x.LogTime).ToList();

                if (dailyPassages.Count == 0)
                {
                    newSummary.IsAbsense = true;
                    absence = 1;
                }

                foreach (var passageItem in dailyPassages)
                {
                    count++;
                    int totalCount = dailyPassages.Count();

                    if (totalCount % 2 == 1 && count == totalCount)
                        type = 2;

                    var temp = new PassageVM
                    {
                        Id = item.Id,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        LogTime = passageItem.LogTime,
                        PointerName = passageItem.Pointer.Name,
                        SSN = item.SSN,
                        Type = type
                    };

                    result.Passages.Add(temp);
                }

                TimeSpan totalDuration = TimeSpan.Zero;
                string fullName = item.LastName + " " + item.FirstName;

                // Get duration of each passage
                for (int i = 0; i < dailyPassages.Count - 1; i += 2)
                {
                    var evenPassage = dailyPassages[i];
                    var oddPasssage = dailyPassages[i + 1];

                    if (oddPasssage != null)
                    {
                        TimeSpan duration = oddPasssage.LogTime - evenPassage.LogTime;
                        totalDuration += duration;
                    }
                }

                newSummary.Absense = absence;
                newSummary.EmployeeName = fullName;
                newSummary.Duration = string.Format("{0:%h}h {0:%m}m", totalDuration);

                result.Summaries.Add(newSummary);
            }

            if (employeeName != null && employeeName.Length != 0)
            {
                result.Summaries = result.Summaries.Where(x => x.EmployeeName.ToLower().Contains(employeeName.ToLower())).ToList();
            }

            return View(result);
        }
    }
}
