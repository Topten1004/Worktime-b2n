using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using Worktime.Global;
using Worktime.Models;
using Worktime.ViewModel;
using static Worktime.Global.Droits;

namespace Worktime.Controllers
{

    [Authorize(Roles = nameof(Droits.Roles.MANAGER) + "," + nameof(Droits.Roles.VIEWER))]

    public class PassageReportController : Controller
    {
        private readonly WorktimeDbContext _context;

        public PassageReportController(WorktimeDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string Month, string Year, int page = 1, int pageSize = 20)
        {
            if (string.IsNullOrEmpty(Month))
            {
                Month = DateTime.Now.Month.ToString();
            }

            if (string.IsNullOrEmpty(Year))
            {
                Year = DateTime.Now.Year.ToString();
            }

            HttpContext.Session.SetString("Year", Year);
            HttpContext.Session.SetString("Month", Month);
            ViewBag.Month = Month;
            ViewBag.Year = Year;

            int skip = (page - 1) * pageSize;

            int intMonth = Convert.ToInt32(Month);
            int intYear = Convert.ToInt32(Year);

            var totalItems = new List<PassageVM>();

            var model = await _context.Employees.Where(l => l.Enable == false).Include(l => l.Passages).ThenInclude(l => l.Pointer).ToListAsync();

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

                            if( check <= 0)
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
                                    totalItems.Add(temp);
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

                        if( check <= 0)
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
                                totalItems.Add(temp);
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

                    totalItems.Add(temp);
                }
            }

            totalItems = totalItems.OrderBy(x => x.LogTime).ToList();

            var result = totalItems
                .Skip(skip)
                .Take(pageSize)
                .ToList();

            ViewBag.PageSize = 20;
            ViewBag.Page = page;

            ViewBag.TotalItems = totalItems.Count();
            ViewBag.TotalPages = ViewBag.TotalItems != null && ViewBag.PageSize != null ? (int)Math.Ceiling((double)ViewBag.TotalItems / (double)ViewBag.PageSize) : 0;

            return View(result);
        }

        public async Task<IActionResult> Details(string? SSN, DateTime? day)
        {
            var result = new List<PassageVM>();

            var model = await _context.Employees.Include(l => l.Passages).ThenInclude(l => l.Pointer).Where(x => x.ReleaseDate == null).OrderBy(x => x.LastName).ToListAsync();

            if (!string.IsNullOrEmpty(SSN))
            {
                model = model.Where( x => x.SSN == SSN ).ToList();
            }

            foreach (var item in model)
            {
                int count = 0;
                int type = 1;

                var passageList = item.Passages.OrderBy( x => x.LogTime).ToList();

                if(day != null)
                {
                    passageList = passageList.Where( x => x.LogTime.Date == day?.Date).ToList();
                } 
                else if ( SSN == null )
                {
                    return View();
                }

                foreach (var passageItem in passageList)
                {
                    count++;
                    int totalCount = passageList.Count();

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

                    result.Add(temp);

                }
            }

            return View(result);
        }
    }
}
