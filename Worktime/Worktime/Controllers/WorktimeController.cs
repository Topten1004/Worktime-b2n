using AutoMapper.Configuration.Conventions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using Worktime.Global;
using Worktime.Models;
using Worktime.ViewModel;

namespace Worktime.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class WorktimeController : ControllerBase
    {
        private readonly WorktimeDbContext _db;

        public WorktimeController(WorktimeDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [Route("Employee")]
        [Authorize(AuthenticationSchemes = "BasicAuthentication")]
        public async Task<List<Employee>> GetEmployees()
        {
            var results = await _db.Employees.ToListAsync();

            return results;
        }

        [HttpGet]
        [Route("Employee/tag")]

        public async Task<List<EmployeeDTO>> GetNotagEmployees()
        {
            var results = new List<EmployeeDTO>();

            var models = await _db.Employees.Where(x => x.Tag == null || x.Tag.Length == 0).ToListAsync();
            foreach(var item in models)
            {
                var tempItem = new EmployeeDTO
                {
                    Id = item.Id,
                    Name = item.FirstName + " " + item.LastName,
                };

                results.Add(tempItem);
            }

            return results;
        }

        [HttpPost]
        [Route("Get/JwtToken")]

        public async Task<string> GetJwtToken(GetJwtTokenVM user)
        {
            var getUser = await _db.Users.Where(x => x.Login == user.Login && x.Mdp == user.MDP).FirstOrDefaultAsync();

            if (getUser != null)
            {
                var token = TokenJWT.creerTokenJWT(user.Login, user.MDP, _db);
                return token.RawData;
            }
            else
                return "";
        }

        [HttpPost]
        [Route("Employee/register")]

        public async Task<int> SaveTag(PostTag tag)
        {
            var employee = await _db.Employees.Where(x => x.Id == tag.EmployeeId).FirstOrDefaultAsync();

            if (employee != null)
            {
                employee.Tag = tag.Tag;

                _db.Update(employee);
                await _db.SaveChangesAsync();

                return 0;
            }
            else
                return -999;
        }

        [HttpPost]
        [Route("Passagelocation")]
        
        public async Task<List<Geolocation>> GetGeolocationLists([FromBody]GetGeolocationVM model)
        {
            var results = new List<Geolocation>();

            var pointerId = _db.Pointers.Where(x => x.Name == model.PointerName).Select(x => x.Id).FirstOrDefault();
            var employeeId = _db.Employees.Where(x => x.SSN == model.SSN).Select(x => x.Id).FirstOrDefault();

            var passages = _db.Passages.Where(x => x.LogTime > model.StartTime && x.LogTime < model.EndTime);

            if (pointerId != 0)
                passages = passages.Where(x => x.PointerId == pointerId);

            if (employeeId != 0)
                passages = passages.Where(x => x.EmployeeId == employeeId);

            results = passages.Select(x => new Geolocation{
                Longitude = x.Longitude,
                Latitude = x.Latitude
            }).ToList();

            return results;
        }

        [HttpPost]
        [Route("Passage")]

        public async Task<ScanSuccess?> SavePassage(PostPassage post)
        {
            var pointer = await _db.Pointers.Where( x => x.Id == post.PointerId ).FirstOrDefaultAsync();
            var employee = await _db.Employees.Where( x => x.Tag == post.Tag && x.Enable != true && x.ReleaseDate == null).FirstOrDefaultAsync();

            DateTime logTime = DateTime.ParseExact(post.LogTime, "yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture);

            if (pointer == null)
                return null;

            if (employee != null)
            {
                var Passage = new Passage
                {
                    PointerId = pointer.Id,
                    EmployeeId = employee.Id,
                    Longitude = post.Longitude,
                    Latitude = post.Latitude,
                    LogTime = logTime
                };

                string logTimeString = Passage.LogTime.ToString("MM-dd HH:mm");

                var result = new ScanSuccess
                {
                    PointerName = pointer.Name,
                    UserName = employee.FirstName + " " + employee.LastName,
                    LogTime = logTimeString,
                };

                await _db.AddAsync(Passage);
                await _db.SaveChangesAsync();

                return result;
            } 
            else if(employee == null)   // to register unknown tag
            {
                var result = new ScanSuccess
                {
                    PointerName = pointer.Name,
                    UserName = "",
                    LogTime = DateTime.UtcNow.ToString("MM-dd HH:mm"),
                };

                return result;
            }

            return null;
        }

        [HttpGet]
        [Route("Pointer/{code}")]
        public async Task<PointerDTO?> PointerLogin(string code)
        {
            Pointer point = await _db.Pointers.Where(x => x.Code == code).FirstOrDefaultAsync();

            if (point != null)
            {
                return new PointerDTO { 
                    Id = point.Id,
                    Name = point.Name
                };
            }
            else
                return null;
        }

        [HttpGet]
        [Route("Passage")]
        [Authorize(AuthenticationSchemes = "BasicAuthentication")]
        public List<PassageVM> GetAllPassages()
        {
            var models = _db.Passages.Include(l => l.Employee).Include(l => l.Pointer).ToList();

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
                    SSN = item.Employee.SSN
                };

                result.Add(temp);
            }

            return result;
        }

        [HttpGet]
        [Route("Setting")]

        public async Task<SettingVM> GetSettingsAsync()
        {
            SettingVM data = new SettingVM();

            var check = await _db.Settings.FirstOrDefaultAsync();

            if (check != null)
            {
                data.geolocation = check.geolocation != 0 ? true : false;
            }
            else
                data.geolocation = false;

            var schedules = await _db.Schedules.ToListAsync();
            foreach(var item in schedules)
            {
                var temp = new ScheduleVM
                {
                    addresslist = item.addresslist,
                    timelist = item.timelist
                };

                data.scheduleList.Add(temp);
            }

            return data;
        }


        [HttpGet]
        [Route("Geolocation")]
        public async Task<bool> GetGeolocation()
        {
            var geolocation = await _db.Settings.FirstOrDefaultAsync();
            
            if(geolocation != null) {
                bool result = geolocation.geolocation != 0 ? true : false;

                return result;
            }

            return false;
        }

        [HttpPost]
        [Route("SaveSchedule")]
        public async Task SaveScheduleSettingAsync([FromBody] SettingVM data)
        {
            var schedulesToDelete = _db.Schedules.ToList();
            _db.Schedules.RemoveRange(schedulesToDelete);
            _db.SaveChanges();

            var setting = _db.Settings.FirstOrDefault();
            int value = data.geolocation ? 1 : 0;

            if (setting != null)
            {
                setting.geolocation = value;
            }
            else
            {
                // Create a new Setting instance with the updated geolocation value
                setting = new Setting
                {
                    geolocation = value
                };

                _db.Settings.Add(setting);
            }

            foreach (var item in data.scheduleList)
            {
                var schedule = new Schedule
                {
                    addresslist = item.addresslist,
                    timelist = item.timelist
                };
                _db.Schedules.Add(schedule);
            }

            await _db.SaveChangesAsync();
        }

        [HttpGet]
        [Route("Passage/Filter")]
        [Authorize(AuthenticationSchemes = "BasicAuthentication")]

        public async Task<IActionResult> GetPassageFilter([FromQuery(Name = "From")] DateTime start, [FromQuery(Name = "End")] DateTime end, [FromQuery(Name = "Employee")] string? ssn, [FromQuery(Name = "Dual")] string? dual)
        {

            if (dual == "yes")
            {
                var result = new List<PassageFilterDualVM>();

                var model = await _db.Employees.Where(l => l.Enable == false).Include(l => l.Passages).ThenInclude(l => l.Pointer).ToListAsync();

                if (ssn != null)
                    model = model.Where(l => l.SSN == ssn).ToList();

                foreach (var item in model)
                {
                    if(start.Month != end.Month)
                    {
                        for(int j = start.Month; j <= end.Month; j++)
                        {
                            var newSummary = new SummaryVM();

                            int daysInMonth = DateTime.DaysInMonth(start.Year, j);

                            for (int i = 1; i <= daysInMonth; i++)
                            {
                                var dailyPassages = item.Passages.Where(x => x.LogTime.Day == i && x.LogTime.Month == j && x.LogTime > start && x.LogTime < end).OrderBy(x => x.LogTime).ToList();

                                var startDate = new DateTime(start.Year, j, i);
                                int check = DateTime.Compare(item.EntryDate, startDate);

                                if (check <= 0)
                                {
                                    for (int k = 0; k < dailyPassages.Count - 1; k += 2)
                                    {
                                        var oddPassage = dailyPassages[k];
                                        var evenPassage = dailyPassages[k + 1];

                                        var newItem = new PassageFilterDualVM();

                                        newItem.Info = item.Info;
                                        newItem.SSN = item.SSN;
                                        newItem.EmployeeName = item.FirstName + " " + item.LastName;

                                        newItem.OddDate = oddPassage.LogTime.Date.ToString("dd/M/yyyy");
                                        newItem.OddLogTime = oddPassage.LogTime.TimeOfDay.ToString(@"hh\:mm\:ss");
                                        newItem.OddPointerName = oddPassage.Pointer.Name;

                                        newItem.EvenDate = evenPassage.LogTime.Date.ToString("dd/M/yyyy");
                                        newItem.EvenLogTime = evenPassage.LogTime.TimeOfDay.ToString(@"hh\:mm\:ss");
                                        newItem.EvenPointerName = evenPassage.Pointer.Name;

                                        result.Add(newItem);
                                    }

                                    if(dailyPassages.Count%2 == 1)
                                    {
                                        var oddPassage = dailyPassages[dailyPassages.Count - 1];

                                        var newItem = new PassageFilterDualVM();

                                        newItem.Info = item.Info;
                                        newItem.SSN = item.SSN;
                                        newItem.EmployeeName = item.FirstName + " " + item.LastName;

                                        newItem.OddDate = oddPassage.LogTime.Date.ToString("dd/M/yyyy");
                                        newItem.OddLogTime = oddPassage.LogTime.TimeOfDay.ToString(@"hh\:mm\:ss");
                                        newItem.OddPointerName = oddPassage.Pointer.Name;

                                        newItem.EvenDate = "";
                                        newItem.EvenLogTime = "";
                                        newItem.EvenPointerName = "";

                                        result.Add(newItem);
                                    }
                                }
                            }

                        }
                    }
                    else
                    {
                        for (int i = start.Day; i <= end.Day; i++)
                        {
                            var dailyPassages = item.Passages.Where(x => x.LogTime.Day == i && x.LogTime.Month == start.Month && x.LogTime > start && x.LogTime < end).OrderBy(x => x.LogTime).ToList();

                            var startDate = new DateTime(start.Year, start.Month, i);
                            int check = DateTime.Compare(item.EntryDate, startDate);

                            if (check <= 0)
                            {
                                for (int k = 0; k < dailyPassages.Count - 1; k += 2)
                                {
                                    var oddPassage = dailyPassages[k];
                                    var evenPassage = dailyPassages[k + 1];

                                    var newItem = new PassageFilterDualVM();

                                    newItem.Info = item.Info;
                                    newItem.SSN = item.SSN;
                                    newItem.EmployeeName = item.FirstName + " " + item.LastName;

                                    newItem.OddDate = oddPassage.LogTime.Date.ToString("dd/M/yyyy");
                                    newItem.OddLogTime = oddPassage.LogTime.ToString(@"hh\:mm\:ss");
                                    newItem.OddPointerName = oddPassage.Pointer.Name;

                                    newItem.EvenDate = evenPassage.LogTime.Date.ToString("dd/M/yyyy");
                                    newItem.EvenLogTime = evenPassage.LogTime.TimeOfDay.ToString(@"hh\:mm\:ss");
                                    newItem.EvenPointerName = evenPassage.Pointer.Name;

                                    result.Add(newItem);
                                }

                                if (dailyPassages.Count % 2 == 1)
                                {
                                    var oddPassage = dailyPassages[dailyPassages.Count - 1];

                                    var newItem = new PassageFilterDualVM();

                                    newItem.Info = item.Info;
                                    newItem.SSN = item.SSN;
                                    newItem.EmployeeName = item.FirstName + " " + item.LastName;

                                    newItem.OddDate = oddPassage.LogTime.Date.ToString("dd/M/yyyy");
                                    newItem.OddLogTime = oddPassage.LogTime.TimeOfDay.ToString(@"hh\:mm\:ss");
                                    newItem.OddPointerName = oddPassage.Pointer.Name;

                                    newItem.EvenDate = "";
                                    newItem.EvenLogTime = "";
                                    newItem.EvenPointerName = "";

                                    result.Add(newItem);
                                }
                            }
                        }
                    }

                }

                return Ok(result);
            }

            var models = _db.Passages.Include(l => l.Employee).Include(l => l.Pointer).ToList();

            if (ssn != null)
                models = models.Where(x => x.LogTime > start && x.LogTime < end && (x.Employee.SSN == ssn)).OrderBy( x => x.LogTime).ToList();
            else
                models = models.Where(x => x.LogTime > start && x.LogTime < end).OrderBy( x => x.LogTime).ToList();

            var resultNo = new List<PassageFilterVM>();

            foreach (var item in models)
            {
                var temp = new PassageFilterVM
                {
                    Info = item.Employee.Info,
                    LogDate = item.LogTime.Date.ToString("dd/M/yyyy"),
                    EmployeeName = item.Employee.FirstName + " " + item.Employee.LastName,
                    LogTime = item.LogTime.TimeOfDay.ToString(@"hh\:mm\:ss"),
                    PointerName = item.Pointer.Name,
                    SSN = item.Employee.SSN
                };

                resultNo.Add(temp);
            }

            return Ok(resultNo);
        }
    }
}
