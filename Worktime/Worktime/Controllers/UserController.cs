using AutoMapper;
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

    [Authorize(Roles = nameof(Droits.Roles.MANAGER))]

    public class UserController : Controller
    {
        private readonly WorktimeDbContext _context;
        private readonly IMapper _mapper;

        public UserController(WorktimeDbContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public IActionResult Index()
        {
            var models = _context.Users.OrderBy(x => x.LastName).ToList();
            return View(models);
        }

        public IActionResult Create()
        {
            ViewData["Role"] = new SelectList(new[] { "MANAGER", "EMPLOYEE", "VIEWER" });

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id, FirstName, LastName, Email, Login, Role, Mdp")] User user)
        {
            try
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            } catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                ViewData["Role"] = new SelectList(new[] { "MANAGER", "EMPLOYEE", "VIEWER" });
                return View(user);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, FirstName, LastName, Email, Login, Mdp, Role")] User user)
        {
            ViewData["Role"] = new SelectList(new[] { "MANAGER", "EMPLOYEE", "VIEWER" });

            var _user = _context.Users.Where(x => x.Id == user.Id).FirstOrDefault();

            if (_user == null)
            {
                return NotFound();
            }

            _user.Email = user.Email;
            _user.FirstName = user.FirstName;
            _user.LastName = user.LastName;
            _user.Login = user.Login;
            _user.Mdp = user.Mdp;
            _user.Role = user.Role;

            _context.Update(_user);
            await _context.SaveChangesAsync();

            if (user.Id != null && user.LastName != null && user.FirstName != null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        // GET: user/Edit/5
        public IActionResult Edit(int? id)
        {
            ViewData["Role"] = new SelectList(new[] { "MANAGER", "EMPLOYEE", "VIEWER" });

            var model = _context.Users.Where(x => x.Id == id).FirstOrDefault();
            return View(model);
        }

        // GET: user/Details/5
        public IActionResult Details(int? Id)
        {
            var model = _context.Users.Where(x => x.Id == Id).FirstOrDefault();
            return View(model);
        }

        // POST: user/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'WorktimeDbContext.Users'  is null.");
            }
            var model = await _context.Users.FindAsync(id);
            if (model != null)
            {
                _context.Users.Remove(model);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: user/Delete/5
        public IActionResult Delete(int? Id)
        {
            if (Id == null || _context.Users == null)
            {
                return NotFound();
            }

            var model = _context.Users.Where(x => x.Id == Id).FirstOrDefault();
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }
    }
}
