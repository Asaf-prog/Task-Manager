using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using WebApplicationDB.Models.Entity;

namespace WebApplicationDB.Data 
{

    public class TaskContext : DbContext 
    {
        
        public DbSet<Models.Entity.TaskEntity>? Tasks { get; set; }
        public DbSet<TaskDate> TaskDates { get; set; }
        public DbSet<TaskTypeTable> TaskTypes { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
        {
            optionsBuilder.UseSqlServer(@"Server=localhost\SQLEXPRESS;Database=masterTask;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            modelBuilder.Entity<TaskDate>()
                .HasOne(td => td.Task)
                .WithMany(t => t.StartDates)
                .HasForeignKey(td => td.TaskEntityId);

             modelBuilder.Entity<TaskTypeTable>()
                .HasKey(tt => tt.Id);

            modelBuilder.Entity<TaskEntity>()
                .HasOne(td => td.TaskTypeTable)
                .WithMany(t => t.TaskEntities)
                .HasForeignKey(td => td.TaskType);

                modelBuilder.Entity<TaskTypeTable>().HasData(
                new TaskTypeTable { Id = 1, Type = "Personal" },
                new TaskTypeTable { Id = 2, Type = "Study" },
                new TaskTypeTable { Id = 3, Type = "Work" }
            );
        }
    }
}
