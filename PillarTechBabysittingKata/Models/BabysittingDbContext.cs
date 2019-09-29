using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PillarTechBabysittingKata.Models
{
    public partial class BabysittingDbContext : DbContext
    {
        public BabysittingDbContext()
        {
        }

        public BabysittingDbContext(DbContextOptions<BabysittingDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Appointments> Appointments { get; set; }
        public virtual DbSet<FamilyPayRates> FamilyPayRates { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=.\\sqlexpress;Database=BabysittingDb;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<Appointments>(entity =>
            {
                entity.Property(e => e.FamilyId)
                    .IsRequired()
                    .HasMaxLength(1);

                entity.Property(e => e.StartDate).HasColumnType("date");
            });

            modelBuilder.Entity<FamilyPayRates>(entity =>
            {
                entity.Property(e => e.FamilyLetter)
                    .IsRequired()
                    .HasMaxLength(1);
            });
        }
    }
}
