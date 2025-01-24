namespace DataAccessLayer.Context
{
    using DomainModel.Entities;
    using log4net;
    using Microsoft.EntityFrameworkCore;
    public class UniversityDbContext : DbContext
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(UniversityDbContext));

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            try
            {
                Logger.Info("Configuring database connection.");
                optionsBuilder.UseNpgsql("Host=localhost;Database=ASSE;Username=postgres;Password=q1w2e3");
            }
            catch (Exception ex)
            {
                Logger.Error("Error configuring database connection.", ex);
                throw;
            }
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<CourseSemester> CourseSemesters { get; set; }
        public DbSet<CoursePrerequisite> CoursePrerequisites { get; set; }
        public DbSet<CourseDiscount> CourseDiscounts { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Exam> Exams { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuring the M:N relationship between Course and Semester through CourseSemester
            modelBuilder.Entity<CourseSemester>()
                .HasOne(cs => cs.Course)
                .WithMany(c => c.CourseSemesters)
                .HasForeignKey(cs => cs.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CourseSemester>()
                .HasOne(cs => cs.Semester)
                .WithMany(s => s.CourseSemesters)
                .HasForeignKey(cs => cs.SemesterId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuring the self-referencing M:N relationship for CoursePrerequisite
            modelBuilder.Entity<CoursePrerequisite>()
                .HasOne(cp => cp.Course)
                .WithMany(c => c.Prerequisites)
                .HasForeignKey(cp => cp.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CoursePrerequisite>()
                .HasOne(cp => cp.PrerequisiteCourse)
                .WithMany(c => c.DependentCourses)
                .HasForeignKey(cp => cp.PrereqId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuring the 1:N relationship between Course and CourseDiscount
            modelBuilder.Entity<CourseDiscount>()
                .HasOne(cd => cd.Course)
                .WithMany(c => c.CourseDiscounts)
                .HasForeignKey(cd => cd.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuring the 1:N relationship between Student and Enrollment
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuring the 1:N relationship between Course and Enrollment
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuring the 1:N relationship between Semester and Enrollment
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Semester)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.SemesterId)
                .OnDelete(DeleteBehavior.Cascade);

            // Uniqueness constraint for Enrollment (a student can only be enrolled in a course in a specific semester once)
            modelBuilder.Entity<Enrollment>()
                .HasIndex(e => new { e.StudentId, e.CourseId, e.SemesterId })
                .IsUnique();

            // Configuring the 1:N relationship between Student and Exam
            modelBuilder.Entity<Exam>()
                .HasOne(ex => ex.Student)
                .WithMany(s => s.Exams)
                .HasForeignKey(ex => ex.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuring the 1:N relationship between Course and Exam
            modelBuilder.Entity<Exam>()
                .HasOne(ex => ex.Course)
                .WithMany(c => c.Exams)
                .HasForeignKey(ex => ex.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuration for Student 
            modelBuilder.Entity<Student>()
                .Property(s => s.Emails)
                .HasColumnType("text[]");

            modelBuilder.Entity<Student>()
                .Property(s => s.PhoneNumbers)
                .HasColumnType("text[]");


        }
    }
}
