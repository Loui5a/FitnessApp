using FitnessApp.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Reflection.Metadata;
namespace FitnessApp.Database
{
    public class FitnessContext : DbContext
    {
        public DbSet<ExerciseModel> ExerciseModels { get; set; }
        public DbSet<ProgramLogModel> ProgramLogModels { get; set; }
        public DbSet<ProgramModel> ProgramModels { get; set; }
        public DbSet<RecordLogModel> RecordLogModels { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(@"Host=;Username=;Password=;Database=");
    }
}
