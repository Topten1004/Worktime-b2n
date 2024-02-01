using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Worktime.Global;
using Worktime.Models;
using Worktime.ViewModel;

namespace Worktime.Controllers
{
    public class CheckController : Controller
    {
        private readonly WorktimeDbContext _context;

        public CheckController(WorktimeDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var user = _context.Users.Where( x => x.Id == userId).FirstOrDefault();

            var model = new CheckVM();

            DateTime gmt11Date = TimeConvert.ConvertToGMT11(DateTime.UtcNow).Date;

            if (user != null)
            {
                var employee = _context.Employees.Where( x => x.FirstName == user.FirstName && x.LastName == user.LastName).FirstOrDefault();

                if (employee != null)
                {
                    var passage = _context.Passages.Where( x => x.EmployeeId == employee.Id && x.LogTime.Date == gmt11Date.Date).ToList();                   

                    model.Check = passage.Count % 2;

                    if (passage.Count > 0)
                    {
                        model.LastTime = TimeConvert.ConvertToGMT11(passage.LastOrDefault().LogTime).ToString("dd/MM : HH:mm");
                    }
                    else model.LastTime = "";

                    model.EmployeeName = employee.FirstName + " " + employee.LastName;
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();

            if (user != null)
            {
                var employee = _context.Employees.Where(x => x.FirstName == user.FirstName && x.LastName == user.LastName).FirstOrDefault();

                Pointer pointer = new Pointer();
                pointer = _context.Pointers.Where( x => x.Name == "Web Employee").FirstOrDefault();

                if (pointer == null)
                {
                    pointer = new Pointer
                    {
                        Code = "000001",
                        Name = "Web Employee"
                    };

                    _context.Update(pointer);

                    await _context.SaveChangesAsync();
                }
                if (employee != null)
                {
                    var passage = new Passage();

                    passage.LogTime = DateTime.Now;
                    passage.EmployeeId = employee.Id;
                    passage.PointerId = pointer.Id;

                    _context.Update(passage);

                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
