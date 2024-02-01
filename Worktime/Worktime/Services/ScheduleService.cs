using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NCrontab;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Worktime.Models;
using Worktime.ViewModel;

namespace Worktime.Services
{
    public class ScheduleService : BackgroundService
    {
        //        * * * * *
        //- - - - -
        //| | | | |
        //| | | | +----- day of week(0 - 6) (Sunday=0)
        //| | | +------- month(1 - 12)
        //| | +--------- day of month(1 - 31)
        //| +----------- hour(0 - 23)
        //+------------- min(0 - 59)

        //* * * * * *
        //- - - - - -
        //| | | | | |
        //| | | | | +--- day of week(0 - 6) (Sunday=0)
        //| | | | +----- month(1 - 12)
        //| | | +------- day of month(1 - 31)
        //| | +--------- hour(0 - 23)
        //| +----------- min(0 - 59)
        //+------------- sec(0 - 59)

        //Toute les 10 secondes */10 * * * * *
        //Lundi à 9h : 0 10 * * MON
        //A 17h : 0 17 * * *
        //A la 23ème minutes : */23 * * * *

        //private CrontabSchedule _schedule;
        //private DateTime _nextRun;

        private readonly WorktimeDbContext context;
        private readonly IMailService mailService;

        public ScheduleService(IServiceScopeFactory _scopeFactory, IMailService _mailService)
        {
            context = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<WorktimeDbContext>();
            mailService = _mailService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                do
                {
                    var schedules = await context.Schedules.AsNoTracking().ToListAsync();
                    
                    DateTime NowTime = DateTime.Now;

                    foreach (var item in schedules)
                    {
                        int hour = 0;
                        int minute = 0;

                        string timeString = item.timelist;
                        string[] timeParts = timeString.Split(':');

                        hour = Convert.ToInt32(timeParts[0]);
                        minute = Convert.ToInt32((timeParts[1]));

                        Console.WriteLine($"Hours: {hour}, Minutes: {minute}");

                        var cronExpression = $"{minute} {hour} * * *"; // Create Crontab expression for the stored time
                        var schedule = CrontabSchedule.Parse(cronExpression);

                        DateTime now = DateTime.Now;
                        DateTime nextRun = schedule.GetNextOccurrence(now);

                        if (now.Hour == nextRun.Hour && now.Minute == nextRun.Minute)
                        {
                            // The current time is very close to the specified time in the schedule
                            // Run your function here
                            MajReportRequest majReportRequest = new MajReportRequest();

                            var model = await context.Employees
                                .AsNoTracking() // Add AsNoTracking() here
                                .Where(l => l.Enable == false)
                                .Include(l => l.Passages)
                                .ThenInclude(l => l.Pointer)
                                .ToListAsync();

                            foreach (var employeeItem in model)
                            {
                                var dailyPassages = employeeItem.Passages.Where(x => x.LogTime.Date.Day == now.Date.Day && x.LogTime.TimeOfDay <= now.TimeOfDay)
                                    .OrderBy(x => x.LogTime)
                                    .ToList();

                                if (dailyPassages.Count == 0)
                                {
                                    majReportRequest.Absents.Add(employeeItem.FirstName + " " + employeeItem.LastName);
                                }
                                else
                                {
                                    int count = 0;
                                    int type = 1;
                                    foreach (var passageItem in dailyPassages)
                                    {
                                        count++;
                                        int totalCount = dailyPassages.Count();

                                        if (totalCount % 2 == 1 && count == totalCount)
                                            type = 2;

                                        var temp = new PassageVM
                                        {
                                            Id = employeeItem.Id,
                                            FirstName = employeeItem.FirstName,
                                            LastName = employeeItem.LastName,
                                            LogTime = passageItem.LogTime,
                                            PointerName = passageItem.Pointer.Name,
                                            SSN = employeeItem.SSN,
                                            Type = type
                                        };

                                        majReportRequest.Passages.Add(temp);
                                    }
                                }
                            }

                            majReportRequest.ToEmail = item.addresslist;

                            majReportRequest.ScheduleTime = DateTime.Now;

                            await mailService.SendMajReport(majReportRequest);
                        }
                    }

                    await Task.Delay(60 * 1000, stoppingToken); //15 seconds delay
                }
                while (!stoppingToken.IsCancellationRequested);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }
    }
}