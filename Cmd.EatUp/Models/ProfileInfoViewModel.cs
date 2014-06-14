using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cmd.EatUp.Data;

namespace Cmd.EatUp.Models
{
    public class ProfileInfoViewModel
    {
        public string FullName { get; set; }

        public string ImagePath { get; set; }

        public DateTime StartPreferredTime { get; set; }
        public DateTime FinishPreferredTime { get; set; }
//        public List<Achievement> Achievements { get; set; }

        public MeetingViewModel CurrentMeeting { get; set; }

        public List<MeetingViewModel> Invitations { get; set; }
    }
}