using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeliverNET.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DeliverNET.Models
{
    public class DeliverNETContext : IdentityDbContext<DeliverNETUser>
    {
        public DeliverNETContext(DbContextOptions<DeliverNETContext> options)
            : base(options)
        {
        }

        public DbSet<Business> Businesses { get; set; }
        public DbSet<BusinessOwner> BusinessOwners { get; set; }
        public DbSet<BusinessCashier> BusinessCashiers { get; set; }
        public DbSet<Deliverer> Deliverers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<Business>().HasKey(b => b.Id);
            builder.Entity<BusinessOwner>().HasKey(bo => bo.DeliverNetUserId);
            builder.Entity<BusinessCashier>().HasKey(bc => bc.DeliverNetUserId);
            builder.Entity<Deliverer>().HasKey(d => d.DeliverNetUserId);
            builder.Entity<Order>().HasKey(o => o.Id);
            builder.Entity<Rating>().HasKey(r => r.Id);

            builder.Entity<DeliverNETUser>()
                .HasOne(u => u.BusinessOwner)
                .WithOne(bo => bo.DeliverNETUser)
                .HasForeignKey<BusinessOwner>(bo => bo.DeliverNetUserId);

            builder.Entity<DeliverNETUser>()
               .HasOne(u => u.BusinessCashier)
               .WithOne(bo => bo.DeliverNETUser)
               .HasForeignKey<BusinessCashier>(bo => bo.DeliverNetUserId);

            builder.Entity<DeliverNETUser>()
               .HasOne(u => u.Deliverer)
               .WithOne(d => d.DeliverNETUser)
               .HasForeignKey<Deliverer>(d => d.DeliverNetUserId);


            builder.Entity<Business>()
                .HasOne(b => b.BusinessOwner)
                .WithOne(bo => bo.Business)
                .HasForeignKey<BusinessOwner>(bo => bo.BusinessId);

        }
    }
}
