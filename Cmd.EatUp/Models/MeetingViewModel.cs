using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cmd.EatUp.Data;

namespace Cmd.EatUp.Models
{
    public class MeetingViewModel
    {
        public DateTime Date { get; set; }

        public string PlaceName { get; set; }

        public DateTime StartPreferredTime { get; set; }
        public DateTime FinishPreferredTime { get; set; }
        public List<Achievement> Achievements { get; set; }

        public List<Meeting> Invitations { get; set; }
    }
}