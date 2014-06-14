using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cmd.EatUp.Data;

namespace Cmd.EatUp.Models
{
    public class MeetingViewModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }

        public string PlaceName { get; set; }

        public List<EmployeeViewModel> Achievements { get; set; }
    }
}