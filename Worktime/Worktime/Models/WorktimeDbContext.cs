using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Worktime.Models
{
    public class WorktimeDbContext : DbContext
    {

        public WorktimeDbContext()
        {

        }

        public WorktimeDbContext(DbContextOptions<WorktimeDbContext> options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            { 
                optionsBuilder.UseMySql("server=localhost;user=root;database=worktime;port=3306;password=;ConvertZeroDateTime=True", ServerVersion.AutoDetect("server=localhost;user=root;database=worktime;port=3306;password=;ConvertZeroDateTime=True"));
                //optionsBuilder.UseMySql("server=172.16.75.101;user=worktime;database=jcp_worktime;port=3306;password=W0rkT1m3:2023;ConvertZeroDateTime=True", ServerVersion.AutoDetect("server=172.16.75.101;user=worktime;database=jcp_worktime;port=3306;password=W0rkT1m3:2023;ConvertZeroDateTime=True"));
                //optionsBuilder.UseMySql("server=172.16.75.101;user=worktime;database=b2n_worktime;port=3306;password=W0rkT1m3:2023;ConvertZeroDateTime=True", ServerVersion.AutoDetect("server=172.16.75.101;user=worktime;database=b2n_worktime;port=3306;password=W0rkT1m3:2023;ConvertZeroDateTime=True"));
                //optionsBuilder.UseMySql("server=172.16.75.101;user=worktime;database=jardideal_worktime;port=3306;password=W0rkT1m3:2023;ConvertZeroDateTime=True", ServerVersion.AutoDetect("server=172.16.75.101;user=worktime;database=jardideal_worktime;port=3306;password=W0rkT1m3:2023;ConvertZeroDateTime=True"));
            }
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Passage>().HasKey(p => p.Id);

            modelBuilder.Entity<Passage>()
                .HasOne(p => p.Employee)
                .WithMany(e => e.Passages)
                .HasForeignKey(p => p.EmployeeId);

            modelBuilder.Entity<Passage>()
                .HasOne(p => p.Pointer)
                .WithMany(d => d.Passages)
                .HasForeignKey(p => p.PointerId);

            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<Employee> Employees { get; set; } = null!;
        public virtual DbSet<Pointer> Pointers { get; set; } = null!;
        public virtual DbSet<Passage> Passages { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Setting> Settings { get; set; } = null!;
        public virtual DbSet<Schedule> Schedules { get; set; } = null!;
    }
}
