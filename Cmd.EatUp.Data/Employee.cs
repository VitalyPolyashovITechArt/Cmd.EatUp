//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Cmd.EatUp.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Employee
    {
        public Employee()
        {
            this.Achievements = new HashSet<Achievement>();
            this.Meetings = new HashSet<Meeting>();
            this.Invites = new HashSet<Meeting>();
        }
    
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ImagePath { get; set; }
        public string Room { get; set; }
        public int DepartmentId { get; set; }
        public Nullable<System.DateTime> Time { get; set; }
        public Nullable<int> PlaceId { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public string Position { get; set; }
        public Nullable<int> ProfileId { get; set; }
        public string StringId { get; set; }
        public DateTime Birthday { get; set; }

        public virtual ICollection<Achievement> Achievements { get; set; }
        public virtual ICollection<Meeting> Meetings { get; set; }
        public virtual ICollection<Meeting> Invites { get; set; }
    }
}
