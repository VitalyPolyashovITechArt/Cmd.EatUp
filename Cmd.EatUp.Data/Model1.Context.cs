﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class eatupEntities1 : DbContext
    {
        public eatupEntities1()
            : base("name=eatupEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Achievement> Achievements { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Meeting> Meetings { get; set; }
        public virtual DbSet<Place> Places { get; set; }
    }
}
