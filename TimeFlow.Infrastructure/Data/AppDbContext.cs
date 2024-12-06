using Microsoft.EntityFrameworkCore;
using TimeFlow.Domain.Entities;

namespace TimeFlow.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<PomodoroSession> PomodoroSessions { get; set; }
        //Привычки
        public DbSet<Habit> Habits { get; set; }
        public DbSet<HabitStage> HabitStages { get; set; }
        public DbSet<HabitPeriodicity> Periodicities { get; set; }
        public DbSet<HabitRecord> CompletionRecords { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<HabitStage>()
                .HasOne(hs => hs.Habit)
                .WithMany(h => h.Stages)
                .HasForeignKey(hs => hs.HabitId);

            modelBuilder.Entity<HabitPeriodicity>()
                .HasOne(p => p.Habit)
                .WithOne(h => h.Periodicity)
                .HasForeignKey<HabitPeriodicity>(p => p.HabitId);

            modelBuilder.Entity<HabitRecord>()
                .HasOne(cr => cr.Habit)
                .WithMany(h => h.CompletionRecords)
                .HasForeignKey(cr => cr.HabitId);
        }
    }
}
