﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ControlTravelAgencySystem.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class TravelSystemEntities : DbContext
    {
        public TravelSystemEntities()
            : base("name=TravelSystemEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<airline> airlines { get; set; }
        public virtual DbSet<airport> airports { get; set; }
        public virtual DbSet<airseat> airseats { get; set; }
        public virtual DbSet<airticket> airtickets { get; set; }
        public virtual DbSet<callout> callouts { get; set; }
        public virtual DbSet<callout_order> callout_order { get; set; }
        public virtual DbSet<callout_room> callout_room { get; set; }
        public virtual DbSet<city> cities { get; set; }
        public virtual DbSet<country> countries { get; set; }
        public virtual DbSet<employee> employees { get; set; }
        public virtual DbSet<excursion> excursions { get; set; }
        public virtual DbSet<excursion_order> excursion_order { get; set; }
        public virtual DbSet<flight> flights { get; set; }
        public virtual DbSet<food> foods { get; set; }
        public virtual DbSet<hotel> hotels { get; set; }
        public virtual DbSet<hotel_service> hotel_service { get; set; }
        public virtual DbSet<hotel_service_order> hotel_service_order { get; set; }
        public virtual DbSet<person> people { get; set; }
        public virtual DbSet<room> rooms { get; set; }
        public virtual DbSet<route> routes { get; set; }
        public virtual DbSet<tour> tours { get; set; }
        public virtual DbSet<transfer> transfers { get; set; }
    }
}
