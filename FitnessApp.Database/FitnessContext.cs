using FitnessApp.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Reflection.Metadata;
namespace FitnessApp.Database
{
    public class FitnessContext : DbContext
    {
        public FitnessContext(DbContextOptions<FitnessContext> options)
        : base(options)
        {
        }
        public DbSet<ExerciseModel> ExerciseModels { get; set; }
        public DbSet<ProgramEntryModel> ProgramLogModels { get; set; }
        public DbSet<ProgramModel> ProgramModels { get; set; }
        public DbSet<RecordLogModel> RecordLogModels { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(@"Host=db;Username=lmb;Password=YVFt4huiVSI8ja;Database=FitnessappDB");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region ExerciseModel
            modelBuilder.Entity<ExerciseModel>().Property(e => e.Type).IsRequired();
            modelBuilder.Entity<ExerciseModel>().Property(e => e.Category).IsRequired();
            modelBuilder.Entity<ExerciseModel>().Property(e => e.Exercise).IsRequired();
            modelBuilder.Entity<ExerciseModel>().Property(e => e.DefaultDuration).IsRequired();
            modelBuilder.Entity<ExerciseModel>().Property(e => e.Description).IsRequired();
            modelBuilder.Entity<ExerciseModel>().HasKey(e => e.Id);
            modelBuilder.Entity<ExerciseModel>().HasMany(e => e.RecordLogs).WithOne(e => e.Exercise).HasForeignKey(e => e.ExerciseId).HasPrincipalKey(e => e.Id);
            #endregion
            
            #region ProgramEntryModel
            modelBuilder.Entity<ProgramEntryModel>().Property(e => e.Order).IsRequired();
            modelBuilder.Entity<ProgramEntryModel>().Property(e => e.ProgramId).IsRequired();
            modelBuilder.Entity<ProgramEntryModel>().Property(e => e.ExerciseId).IsRequired();
            modelBuilder.Entity<ProgramEntryModel>().HasKey(e => e.Id);
            modelBuilder.Entity<ProgramEntryModel>().HasOne(e => e.Program).WithMany(e => e.ProgramEntries).HasForeignKey(e => e.ProgramId).HasPrincipalKey(e => e.Id);
            modelBuilder.Entity<ProgramEntryModel>().HasOne(e => e.Exercise).WithMany(e => e.ProgramEntries).HasForeignKey(e => e.ExerciseId).HasPrincipalKey(e => e.Id);
            #endregion

            #region ProgramModel
            modelBuilder.Entity<ProgramModel>().Property(e => e.Name).IsRequired();
            modelBuilder.Entity<ProgramModel>().HasKey(e => e.Id);
            #endregion

            #region RecordLogModel
            modelBuilder.Entity<RecordLogModel>().Property(e => e.Date).IsRequired();
            modelBuilder.Entity<RecordLogModel>().Property(e => e.Reps).IsRequired();
            modelBuilder.Entity<RecordLogModel>().Property(e => e.Sets).IsRequired();
            modelBuilder.Entity<RecordLogModel>().Property(e => e.Weight).IsRequired();
            modelBuilder.Entity<RecordLogModel>().Property(e => e.ExerciseId).IsRequired();
            modelBuilder.Entity<RecordLogModel>().HasKey(e => e.Id);
            #endregion
            // run this each time the database is updated:
            // dotnet ef migrations add Initial --project FitnessApp.Database --startup-project FitnessApp --output-dir Migrations
        }
    }
}
