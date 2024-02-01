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

    public class PassageController : Controller
    {
        private readonly WorktimeDbContext _context;

        public PassageController(WorktimeDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(DateTime beginDate, DateTime endDate, int page = 1, int pageSize = 20)
        {
            if (beginDate == DateTime.MinValue || endDate == DateTime.MinValue)
            {
                if (HttpContext.Session.GetString("beginDate") == null) beginDate = DateTime.Now.AddMonths(-1);
                else beginDate = Convert.ToDateTime(HttpContext.Session.GetString("beginDate"));
                if (HttpContext.Session.GetString("endDate") == null) endDate = DateTime.Now.AddMonths(1);
                else endDate = Convert.ToDateTime(HttpContext.Session.GetString("endDate"));
            }

            int skip = (page - 1) * pageSize;

            HttpContext.Session.SetString("beginDate", beginDate.Date.ToString("yyyy-MM-dd"));
            HttpContext.Session.SetString("endDate", endDate.Date.ToString("yyyy-MM-dd"));

            var models = _context.Passages
                    .Include(l => l.Employee)
                    .Include(l => l.Pointer)
                    .Where(x => x.LogTime < endDate && x.LogTime > beginDate && x.Employee.ReleaseDate == null)
                    .OrderByDescending(x => x.LogTime)
                    .Skip(skip)
                    .Take(pageSize)
                    .ToList();

            var allModels = _context.Passages
                .Include(l => l.Employee)
                .Include(l => l.Pointer)
                .Where(x => x.LogTime < endDate && x.LogTime > beginDate && x.Employee.ReleaseDate == null)
                .OrderByDescending(x => x.LogTime)
                .ToList();

            ViewBag.beginDate = beginDate;
            ViewBag.endDate = endDate;
            ViewBag.PageSize = 20;
            ViewBag.Page = page;

            ViewBag.TotalItems = allModels.Count();
            ViewBag.TotalPages = ViewBag.TotalItems != null && ViewBag.PageSize != null ? (int)Math.Ceiling((double)ViewBag.TotalItems / (double)ViewBag.PageSize) : 0;
            
            var result = new List<PassageVM>();

            foreach (var item in models)
            {
                var temp = new PassageVM
                {
                    Id = item.Id,
                    FirstName = item.Employee.FirstName,
                    LastName = item.Employee.LastName,
                    LogTime = item.LogTime,
                    PointerName = item.Pointer.Name,
                    SSN = item.Employee.SSN,
                    Type = 0
                };

                result.Add(temp);
            }

            return View(result);
        }
    }
}
