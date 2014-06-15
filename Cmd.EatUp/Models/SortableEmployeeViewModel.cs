using System.Collections.Generic;

namespace Cmd.EatUp.Models
{

    public class SortableEmployeeViewModel
    {
        public int Index { get; set; }

        public int Id { get; set; }

        public string ImageUrl { get; set; }

        public string FullName { get; set; }
    }
}