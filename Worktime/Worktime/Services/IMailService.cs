using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Worktime.Models;

namespace Worktime.Services
{
    public interface IMailService
    {
        Task SendMajReport(MajReportRequest request);
    }
}
