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

    public class MonthlySummaryController : Controller
    {
        private readonly WorktimeDbContext _context;

        public MonthlySummaryController(WorktimeDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string Month, string Year, string employeeName)
        {
            if (string.IsNullOrEmpty(Month))
            {
                Month = DateTime.Now.Month.ToString();
            }

            if (string.IsNullOrEmpty(Year))
            {
                Year = DateTime.Now.Year.ToString();
            }

            if (string.IsNullOrEmpty(employeeName))
            {
                employeeName = string.Empty;
            }

            HttpContext.Session.SetString("Year", Year);
            HttpContext.Session.SetString("Month", Month);
            HttpContext.Session.SetString("employeeName", employeeName);

            ViewBag.Month = Month;
            ViewBag.Year = Year;
            ViewBag.employeeName = employeeName;

            int intMonth = Convert.ToInt32(Month);
            int intYear = Convert.ToInt32(Year);

            var result = new PassageReportVM();

            var model = await _context.Employees.Where(l => l.Enable == false).Include(l => l.Passages).ThenInclude(l => l.Pointer).OrderBy(x => x.LastName).ToListAsync();

            foreach (var item in model)
            {
                int count = 0;
                int type = 1;
                int absense = 0;

                var newSummary = new SummaryVM();

                int daysInMonth = DateTime.DaysInMonth(intYear, intMonth);
                var monthlyPassages = item.Passages.Where(x => x.LogTime.Month == intMonth && x.LogTime.Year == intYear).OrderBy(x => x.LogTime).ToList();

                if (monthlyPassages.Count == 0)
                {
                    newSummary.IsAbsense = true;
                }

                if (intYear <= DateTime.UtcNow.Year && intMonth < DateTime.UtcNow.Month)
                {
                    // If no passage on oneday, add passage
                    for (int i = 1; i <= daysInMonth; i++)
                    {
                        var dailyPassages = monthlyPassages.Where(x => x.LogTime.Day == i).ToList();

                        var startDate = new DateTime(intYear, intMonth, i);
                        int check = DateTime.Compare(item.EntryDate, startDate);

                        if (check <= 0)
                        {
                            if (dailyPassages.Count == 0)
                            {
                                var newDate = new DateTime(intYear, intMonth, i);
                                var temp = new PassageVM
                                {
                                    Id = item.Id,
                                    FirstName = item.FirstName,
                                    LastName = item.LastName,
                                    LogTime = newDate,
                                    PointerName = "",
                                    SSN = item.SSN,
                                    Type = 0
                                };

                                ++absense;
                                result.Passages.Add(temp);
                            }
                        }
                    }
                }
                else if (intYear == DateTime.UtcNow.Year && intMonth == DateTime.UtcNow.Month)
                {
                    for (int i = 1; i <= DateTime.Now.Day; i++)
                    {
                        var dailyPassages = monthlyPassages.Where(x => x.LogTime.Day == i).ToList();

                        var startDate = new DateTime(intYear, intMonth, i);
                        int check = DateTime.Compare(item.EntryDate, startDate);

                        if (check <= 0)
                        {
                            if (dailyPassages.Count == 0)
                            {
                                var newDate = new DateTime(intYear, intMonth, i);
                                var temp = new PassageVM
                                {
                                    Id = item.Id,
                                    FirstName = item.FirstName,
                                    LastName = item.LastName,
                                    LogTime = newDate,
                                    PointerName = "",
                                    SSN = item.SSN,
                                    Type = 0
                                };

                                ++absense;
                                result.Passages.Add(temp);
                            }
                        }
                    }
                }
                foreach (var passageItem in monthlyPassages)
                {
                    count++;
                    int totalCount = monthlyPassages.Count();

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
                for (int i = 0; i < monthlyPassages.Count - 1; i += 2)
                {
                    var evenPassage = monthlyPassages[i];
                    var oddPasssage = monthlyPassages[i + 1];

                    if (oddPasssage != null)
                    {
                        TimeSpan duration = oddPasssage.LogTime - evenPassage.LogTime;
                        totalDuration += duration;
                    }
                }

                newSummary.Absense = absense;
                newSummary.EmployeeName = fullName;
                newSummary.Duration = string.Format("{0:%h}h {0:%m}m", totalDuration);

                result.Summaries.Add(newSummary);
            }

            result.Passages = result.Passages.OrderBy(x => x.LogTime).ToList();

            if (employeeName != null && employeeName.Length != 0)
            {
                result.Summaries = result.Summaries.Where(x => x.EmployeeName.ToLower().Contains(employeeName.ToLower())).ToList();
            }

            return View(result);
        }
    }
}
