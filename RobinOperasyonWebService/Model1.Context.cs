﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RobinOperasyonWebService
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class rpa_robin01Entities : DbContext
    {
        public rpa_robin01Entities()
            : base("name=rpa_robin01Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<EMPTOR_RBN_TICKET> EMPTOR_RBN_TICKET { get; set; }
        public virtual DbSet<RBN_EMPTOR_API_USERS> RBN_EMPTOR_API_USERS { get; set; }
        public virtual DbSet<RBN_EMPTOR_SIGNATURE> RBN_EMPTOR_SIGNATURE { get; set; }
        public virtual DbSet<RBN_EMPTOR_AUTOTICKETCLOSEDScheduler> RBN_EMPTOR_AUTOTICKETCLOSEDScheduler { get; set; }
        public virtual DbSet<RBN_EMPTOR_AUTOCLOSEDTICKET> RBN_EMPTOR_AUTOCLOSEDTICKET { get; set; }
    }
}
