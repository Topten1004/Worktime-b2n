using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Worktime.Global;
using Worktime.Models;
using Worktime.ViewModel;
using static Worktime.Global.Droits;

namespace Worktime.Controllers
{

    [Authorize(Roles = nameof(Droits.Roles.MANAGER) + "," + nameof(Droits.Roles.VIEWER))]

    public class MajPassageController : Controller
    {
        private readonly WorktimeDbContext _context;

        public MajPassageController(WorktimeDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(DateTime majBeginDate, int page = 1, int pageSize = 20)
        {
            if (majBeginDate == DateTime.MinValue)
            {
                if (HttpContext.Session.GetString("majBeginDate") == null) majBeginDate = DateTime.Now.Date;
                else majBeginDate = Convert.ToDateTime(HttpContext.Session.GetString("majBeginDate"));
            }

            HttpContext.Session.SetString("majBeginDate", majBeginDate.Date.ToString("yyyy-MM-dd"));
            ViewBag.majBeginDate = majBeginDate;

            int skip = (page - 1) * pageSize;

            var totalItems = new List<PassageVM>();

            var model = await _context.Employees.Where(l => l.Enable == false).Include(l => l.Passages).ThenInclude(l => l.Pointer).Where(x => x.ReleaseDate == null).ToListAsync();

            foreach (var item in model)
            {
                int count = 0;
                int type = 1;

                var dailyPassages = item.Passages.Where( x => x.LogTime.Date == majBeginDate.Date).OrderBy(x => x.LogTime).ToList();

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

                    totalItems.Add(temp);
                }
            }

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

        public async Task<IActionResult> Details([FromQuery]string SSN, DateTime majBeginDate)
        {
            if (majBeginDate == DateTime.MinValue)
            {
                if (HttpContext.Session.GetString("beginDate") == null) majBeginDate = DateTime.Now.Date;
                else majBeginDate = Convert.ToDateTime(HttpContext.Session.GetString("beginDate"));
            }

            HttpContext.Session.SetString("majBeginDate", majBeginDate.Date.ToString("yyyy-MM-dd"));
            HttpContext.Session.SetString("SSN", SSN);

            ViewBag.SSN = SSN;
            ViewBag.majBeginDate = majBeginDate;

            var result = new List<PassageVM>();

            var model = await _context.Employees.Include(l => l.Passages).ThenInclude(l => l.Pointer).Where( x => x.SSN == SSN && x.ReleaseDate == null).OrderBy(x => x.LastName).ToListAsync();

            foreach (var item in model)
            {
                int count = 0;
                int type = 1;

                var dailyPassages = item.Passages.Where(x => x.LogTime.Date == majBeginDate.Date).OrderBy(x => x.LogTime).ToList();

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

                    result.Add(temp);

                }
            }

            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddPassage(DateTime majBeginDate, string SSN, string time)
        {
            var employee = await _context.Employees.Where(x => x.SSN == SSN).FirstOrDefaultAsync();

            var pointer = await _context.Pointers.Where( x => x.Name == "Web Manager").FirstOrDefaultAsync();

            if (pointer == null)
            {
                pointer = new Pointer
                {
                    Code = "99998",
                    Name = "Web Manager"
                };

                _context.Add(pointer);

                await _context.SaveChangesAsync();
            }

            string format = "H:mm"; // Specify the format of the input string

            DateTime currentTime = DateTime.ParseExact(time, format, CultureInfo.InvariantCulture);

            // Combine the time with the current date
            DateTime dateTimeWithTime = majBeginDate.Add(currentTime.TimeOfDay);

            if (employee != null && pointer != null)
            {
                var passage = new Passage
                {
                    EmployeeId = employee.Id,
                    PointerId = pointer.Id,
                    LogTime = dateTimeWithTime,
                };

                _context.Passages.Add(passage);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new {beginDate = majBeginDate, SSN = SSN});
        }
    }
}
