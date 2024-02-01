using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Worktime.Global;
using Worktime.Models;
using Worktime.ViewModel;
using static Worktime.Global.Droits;

namespace Worktime.Controllers
{

    [Authorize(Roles = nameof(Droits.Roles.MANAGER))]

    public class PointerController : Controller
    {

        private readonly WorktimeDbContext _context;
        private readonly IMapper _mapper;

        public PointerController(WorktimeDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var model = _context.Pointers.ToList();
            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name, Code")] Pointer pointer)
        {
            string pattern = @"^\d{5}$";

            // Check if the input matches the pattern
            bool isMatch = Regex.IsMatch(pointer.Code, pattern);

            if (isMatch)
            {
                var check = _context.Pointers.Where(x => x.Code == pointer.Code || x.Code == pointer.Code).ToList();

                if(check.Count == 0)
                    {
                    _context.Add(pointer);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(pointer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name, Code")] Pointer pointer)
        {
            var _pointer = _context.Pointers.Where(x => x.Id == id).FirstOrDefault();

            if (_pointer == null)
            {
                return NotFound();
            }

            string pattern = @"^\d{5}$";

            // Check if the input matches the pattern
            bool isMatch = Regex.IsMatch(pointer.Code, pattern);

            if (isMatch)
            {
                var check = _context.Pointers.Where(x => x.Code == pointer.Code || x.Code == pointer.Code).ToList();

                if (check.Count == 0)
                {
                     _pointer.Code = pointer.Code;
                     _pointer.Name = pointer.Name;

                     _context.Update(_pointer);
                     await _context.SaveChangesAsync();

                     return RedirectToAction(nameof(Index));
                }
            }

            return View(pointer);
        }

        // GET: Pointer/Edit/5
        public IActionResult Edit(int? id)
        {
            var model = _context.Pointers.Where(x => x.Id == id).FirstOrDefault();
            return View(model);
        }

        // GET: Pointer/Details/5
        public IActionResult Details(int? Id)
        {

            var model = _context.Pointers.Where(x => x.Id == Id).FirstOrDefault();
            return View(model);
        }

        // POST: Pointer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Pointers == null)
            {
                return Problem("Entity set 'WorktimeDbContext.Pointers'  is null.");
            }
            var model = await _context.Pointers.FindAsync(id);
            if (model != null)
            {
                _context.Pointers.Remove(model);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Pointer/Delete/5
        public IActionResult Delete(int? Id)
        {
            if (Id == null || _context.Pointers == null)
            {
                return NotFound();
            }

            var model = _context.Pointers.Where(x => x.Id == Id).FirstOrDefault();
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }
    }
}
