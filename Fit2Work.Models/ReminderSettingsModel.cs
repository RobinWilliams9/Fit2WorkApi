using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnvilGroup.Services.Fit2Work.Models
{
    public class ReminderSettingsModel
    {
        public int DayNo { get; set; }
        public string Day { get; set; }
        public bool IsEnabled { get; set; }
        public TimeSpan TimeSet { get; set; }

    }

    public class ReminderSettingsReceiver
    {
        public int UserId { get; set; }
        public string ReminderSettings { get; set; }
    }
}
