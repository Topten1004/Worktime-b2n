using Microsoft.AspNetCore.Mvc;
using Worktime.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using Worktime.ViewModel;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using static Worktime.Global.Droits;
using Worktime.Global;
using Worktime.Services;

namespace Worktime.Controllers
{
    [Authorize(Roles = nameof(Droits.Roles.MANAGER))]

    public class EmployeeController : Controller
    {
        private readonly WorktimeDbContext _context;
        private readonly IMapper _mapper;

        public EmployeeController(WorktimeDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var model = _context.Employees.OrderBy(x => x.LastName).ToList();

            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SSN,Tag,FirstName,LastName,EntryDate, Info, WebAccess, Enable")] EmployeeVM employeeVM)
        {
            Employee employee = _mapper.Map<Employee>(employeeVM);

            if(employeeVM.FirstName != null && employeeVM.LastName != null && employeeVM.SSN != null) {

                if (employeeVM.EntryDate == null)
                    employeeVM.EntryDate = DateTime.UtcNow;

                if (employeeVM.Enable == true)
                    employeeVM.ReleaseDate = DateTime.UtcNow;

                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }

            return View(employeeVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SSN,Tag,FirstName,LastName,EntryDate,ReleaseDate,Info,WebAccess,Enable")] Employee employee)
        {
            var _employee = _context.Employees.Where(x => x.Id == employee.Id).FirstOrDefault();

            if(_employee == null) {
                return NotFound();
            }

            _employee.SSN = employee.SSN;
            _employee.FirstName = employee.FirstName;
            _employee.LastName = employee.LastName;
            _employee.Tag = employee.Tag;
            _employee.EntryDate = employee.EntryDate;
            _employee.ReleaseDate = employee.ReleaseDate;
            _employee.WebAccess = employee.WebAccess;
            _employee.Info = employee.Info;

            if (_employee.EntryDate == null)
                _employee.EntryDate = DateTime.Now;

            if (_employee.Enable == false && employee.Enable == true)
                _employee.ReleaseDate = DateTime.Now;

            if (_employee.Enable == true && employee.Enable == false)
                _employee.ReleaseDate = null;

            if (employee.ReleaseDate != null)
                _employee.Enable = true;

            _employee.Enable = employee.Enable;

            _context.Update(_employee); 
            await _context.SaveChangesAsync();

            if(employee.Id != null && employee.LastName != null && employee.FirstName != null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(employee);
        }

        // GET: Employee/Edit/5
        public IActionResult Edit(int? id)
        {
            var model = _context.Employees.Where(x => x.Id == id).FirstOrDefault();
            return View(model);
        }

        // GET: Employee/Details/5
        public IActionResult Details(int? Id) {

            var model = _context.Employees.Where(x => x.Id == Id).FirstOrDefault();
            return View(model);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Employees == null)
            {
                return Problem("Entity set 'WorktimeDbContext.Employees'  is null.");
            }
            var model = await _context.Employees.FindAsync(id);
            if (model != null)
            {
                _context.Employees.Remove(model);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Employee/Delete/5
        public async Task<IActionResult> Delete(int? Id)
        {
            if (Id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var model = _context.Employees.Where(x => x.Id == Id).FirstOrDefault();
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }
    }
}