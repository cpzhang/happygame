using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace dbcontext
{
    public partial class DBContext : DbContext
    {
        public DBContext()
        {
        }

        public DBContext(DbContextOptions<DBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<User> User { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=localhost,38065;Database=happygame;User ID=sa;Password=Love2016;Integrated Security=false;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => new { e.GameId, e.ChannelId, e.Sdkid })
                    .HasName("NonClusteredIndex-GameID-ChannelID-SDKID")
                    .IsUnique();

                entity.Property(e => e.UserId)
                    .HasColumnName("UserID")
                    .ValueGeneratedNever();

                entity.Property(e => e.ChannelId).HasColumnName("ChannelID");

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.GameId).HasColumnName("GameID");

                entity.Property(e => e.Sdkid)
                    .IsRequired()
                    .HasColumnName("SDKID")
                    .HasMaxLength(64);
            });
        }
    }
}
