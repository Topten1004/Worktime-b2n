using Microsoft.OpenApi.Writers;
using Worktime.Models;

namespace Worktime.ViewModel
{
    public class SettingVM
    {
        public SettingVM() {

            scheduleList = new List<ScheduleVM>();
        }

        public bool geolocation { get; set; }

        public List<ScheduleVM> scheduleList { get; set; }
    }

    public class ScheduleVM
    {
        public string addresslist { get; set; } = string.Empty;

        public string timelist { get; set; } = string.Empty;
    }
}
