using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PrespaEvents.Web.Models.Domain;
using PrespaEvents.Web.Models.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrespaEvents.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<EventApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<EventInCart> EventInCarts { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Event>()
                .Property(z => z.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<Cart>()
                .Property(z => z.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<EventInCart>()
                .HasKey(z => new { z.EventId, z.CartId });

            builder.Entity<EventInCart>()
              .HasOne(z => z.Event)
              .WithMany(z => z.EventInCarts)
              .HasForeignKey(z => z.EventId);

            builder.Entity<EventInCart>()
                .HasOne(z => z.Cart)
                .WithMany(z => z.EventInCarts)
                .HasForeignKey(z => z.CartId);

            builder.Entity<Cart>()
                .HasOne<EventApplicationUser>(z => z.Owner)
                .WithOne(zt => zt.UserCart)
                .HasForeignKey<Cart>(bc => bc.OwnerId);

            builder.Entity<EventInOrder>()
                .HasKey(z => new { z.EventId, z.OrderId });

            builder.Entity<EventInOrder>()
                .HasOne(z => z.SelectedEvent)
                .WithMany(t => t.Orders)
                .HasForeignKey(z => z.EventId);

            builder.Entity<EventInOrder>()
               .HasOne(z => z.UserOrder)
               .WithMany(t => t.Events)
               .HasForeignKey(z => z.OrderId);

            builder.Entity<EventApplicationUser>()
                .HasMany(e => e.MyEvents)
                .WithOne(e => e.Organizer)
                .HasForeignKey(e => e.OrganizerId)
                .IsRequired();
            builder.Entity<Event>()
                .HasOne(e => e.Organizer)
                .WithMany(e => e.MyEvents)
                .HasForeignKey(e => e.OrganizerId)
                .IsRequired();

            builder.Entity<Category>()
                .HasMany(e => e.Events)
                .WithOne(e => e.EventCategory)
                .HasForeignKey(e => e.CategoryId)
                .IsRequired();
            builder.Entity<Event>()
               .HasOne(e => e.EventCategory)
               .WithMany(e => e.Events)
               .HasForeignKey(e => e.CategoryId)
               .IsRequired();
        }


        public DbSet<PrespaEvents.Web.Models.Domain.Category> Category { get; set; }

    }
}
