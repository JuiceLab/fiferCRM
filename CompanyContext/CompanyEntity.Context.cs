﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CompanyContext
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class CompanyEntities : DbContext
    {
        public CompanyEntities()
            : base("name=CompanyEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<EmployeeTimesheet> EmployeeTimesheets { get; set; }
        public virtual DbSet<LegalEntity> LegalEntities { get; set; }
        public virtual DbSet<Passport> Passports { get; set; }
        public virtual DbSet<Position> Positions { get; set; }
        public virtual DbSet<WorkGroup> WorkGroups { get; set; }
        public virtual DbSet<WebSite> WebSites { get; set; }
    }
}
