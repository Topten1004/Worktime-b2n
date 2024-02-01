using Microsoft.AspNetCore.Mvc;
using Worktime.Models;

namespace Worktime.Controllers
{
    public class GeolocationController : Controller
    {
        private readonly WorktimeDbContext _context;

        public GeolocationController(WorktimeDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(DateTime geoBeginDate, DateTime geoEndDate)
        {
            if (geoBeginDate == DateTime.MinValue || geoEndDate == DateTime.MinValue)
            {
                if (HttpContext.Session.GetString("geoBeginDate") == null) geoBeginDate = DateTime.Now.AddMonths(-1);
                else geoBeginDate = Convert.ToDateTime(HttpContext.Session.GetString("geoBeginDate"));
                if (HttpContext.Session.GetString("geoEndDate") == null) geoEndDate = DateTime.Now.AddMonths(1);
                else geoEndDate = Convert.ToDateTime(HttpContext.Session.GetString("geoEndDate"));
            }

            HttpContext.Session.SetString("beginDate", geoBeginDate.Date.ToString("yyyy-MM-dd"));
            HttpContext.Session.SetString("endDate", geoEndDate.Date.ToString("yyyy-MM-dd"));
            ViewBag.geoBeginDate = geoBeginDate;
            ViewBag.geoEndDate = geoEndDate;

            List<string> terminalList = _context.Pointers.Select( p => p.Name).ToList();
            List<string> ssnList = _context.Employees.Select( p => p.SSN).ToList();

            // Pass the terminal list to the ViewBag
            ViewBag.SSNList = ssnList;
            ViewBag.TerminalList = terminalList;
            return View();
        }
    }
}
