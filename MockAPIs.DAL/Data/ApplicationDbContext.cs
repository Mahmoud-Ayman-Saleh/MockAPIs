using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MockAPIs.DAL.Models;

namespace MockAPIs.DAL.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Field> Fields { get; set; }
        public DbSet<MockRecord> MockRecords { get; set; }
        public DbSet<EndpointConfig> EndpointConfigs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); //

            // 1- project -> User relationship
            builder.Entity<Project>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<Resource>()
                .HasOne(r => r.Project)
                .WithMany(p => p.Resources)
                .HasForeignKey(r => r.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
            

            builder.Entity<Field>()
                .HasOne(f => f.Resource)
                .WithMany(r => r.Fields)
                .HasForeignKey(f => f.ResourceId)
                .OnDelete(DeleteBehavior.Cascade);
            

            builder.Entity<MockRecord>()
                .HasOne(m => m.Resource)
                .WithMany(r => r.MockRecords)
                .HasForeignKey(m => m.ResourceId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<EndpointConfig>()
                .HasOne(e => e.Resource)
                .WithOne(r => r.EndpointConfig)
                .HasForeignKey<EndpointConfig>(e => e.ResourceId)
                .OnDelete(DeleteBehavior.Cascade);

            
            builder.Entity<MockRecord>()
            .Property(m => m.Data)
            .HasColumnType("jsonb");

            builder.Entity<Field>()
            .Property(f => f.DataType)
            .HasConversion<string>();
            
            builder.Entity<AppUser>()
            .Property(e => e.Plan)
            .HasConversion<string>();

            builder.Entity<Project>()
            .HasIndex(p => p.Token)
            .IsUnique();

            builder.Entity<Project>()
            .HasIndex(p => p.Slug)
            .IsUnique();

            builder.Entity<Resource>()
            .HasIndex(r => new{r.ProjectId, r.Slug})
            .IsUnique();

            builder.Entity<EndpointConfig>()
            .HasIndex(e => e.ResourceId)
            .IsUnique();
        }
    }
}