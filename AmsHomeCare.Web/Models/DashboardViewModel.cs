using System.Collections.Generic;
using AmsHomeCare.Core.Entities;

namespace AmsHomeCare.Web.Models
{
    public class DashboardViewModel
    {
        public int TotalEmployees { get; set; }
        public int TotalPatients { get; set; }
        public int PresentToday { get; set; }
        public int OnLeaveToday { get; set; }
        public IEnumerable<DutyAssignment> TodaySchedule { get; set; } = new List<DutyAssignment>();
        public IEnumerable<string> RecentActivities { get; set; } = new List<string>();
    }
}
