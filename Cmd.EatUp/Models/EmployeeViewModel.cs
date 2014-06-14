using System.Collections.Generic;

namespace Cmd.EatUp.Models
{
    public class PreferrablePeopleViewModel
    {
        public IEnumerable<PreferrablePersonViewModel> PreferrablePeople { get; set; }
    }

    public class PreferrablePersonViewModel
    {
        public string ImageUrl { get; set; }

        public string FullName { get; set; }
    }
}