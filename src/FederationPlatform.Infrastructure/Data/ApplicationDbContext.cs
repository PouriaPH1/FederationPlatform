using FederationPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FederationPlatform.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    #region DbSets
    public DbSet<User> Users { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<University> Universities { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<Activity> Activities { get; set; }
    public DbSet<ActivityFile> ActivityFiles { get; set; }
    public DbSet<News> News { get; set; }
    public DbSet<Workshop> Workshops { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<ActivityLog> ActivityLogs { get; set; }
    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User Configuration
        modelBuilder.Entity<User>()
            .HasKey(u => u.Id);
        
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();
        
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
        
        modelBuilder.Entity<User>()
            .Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(100);
        
        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);
        
        modelBuilder.Entity<User>()
            .Property(u => u.PasswordHash)
            .IsRequired();

        // UserProfile Configuration
        modelBuilder.Entity<UserProfile>()
            .HasKey(up => up.Id);
        
        modelBuilder.Entity<UserProfile>()
            .HasOne(up => up.User)
            .WithOne(u => u.UserProfile)
            .HasForeignKey<UserProfile>(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<UserProfile>()
            .HasOne(up => up.University)
            .WithMany(u => u.UserProfiles)
            .HasForeignKey(up => up.UniversityId)
            .OnDelete(DeleteBehavior.SetNull);
        
        modelBuilder.Entity<UserProfile>()
            .Property(up => up.FirstName)
            .IsRequired()
            .HasMaxLength(100);
        
        modelBuilder.Entity<UserProfile>()
            .Property(up => up.LastName)
            .IsRequired()
            .HasMaxLength(100);

        // University Configuration
        modelBuilder.Entity<University>()
            .HasKey(u => u.Id);
        
        modelBuilder.Entity<University>()
            .Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        modelBuilder.Entity<University>()
            .Property(u => u.Province)
            .IsRequired()
            .HasMaxLength(100);
        
        modelBuilder.Entity<University>()
            .Property(u => u.City)
            .IsRequired()
            .HasMaxLength(100);
        
        modelBuilder.Entity<University>()
            .Property(u => u.Description)
            .HasMaxLength(1000);
        
        modelBuilder.Entity<University>()
            .HasMany(u => u.Activities)
            .WithOne(a => a.University)
            .HasForeignKey(a => a.UniversityId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<University>()
            .HasMany(u => u.UserProfiles)
            .WithOne(up => up.University)
            .HasForeignKey(up => up.UniversityId)
            .OnDelete(DeleteBehavior.SetNull);

        // Organization Configuration
        modelBuilder.Entity<Organization>()
            .HasKey(o => o.Id);
        
        modelBuilder.Entity<Organization>()
            .Property(o => o.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        modelBuilder.Entity<Organization>()
            .Property(o => o.Description)
            .HasMaxLength(1000);
        
        modelBuilder.Entity<Organization>()
            .HasMany(o => o.Activities)
            .WithOne(a => a.Organization)
            .HasForeignKey(a => a.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Activity Configuration
        modelBuilder.Entity<Activity>()
            .HasKey(a => a.Id);
        
        modelBuilder.Entity<Activity>()
            .Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(200);
        
        modelBuilder.Entity<Activity>()
            .Property(a => a.Description)
            .IsRequired();
        
        modelBuilder.Entity<Activity>()
            .HasOne(a => a.User)
            .WithMany(u => u.Activities)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Activity>()
            .HasOne(a => a.University)
            .WithMany(u => u.Activities)
            .HasForeignKey(a => a.UniversityId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Activity>()
            .HasOne(a => a.Organization)
            .WithMany(o => o.Activities)
            .HasForeignKey(a => a.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Activity>()
            .HasMany(a => a.ActivityFiles)
            .WithOne(af => af.Activity)
            .HasForeignKey(af => af.ActivityId)
            .OnDelete(DeleteBehavior.Cascade);

        // ActivityFile Configuration
        modelBuilder.Entity<ActivityFile>()
            .HasKey(af => af.Id);
        
        modelBuilder.Entity<ActivityFile>()
            .Property(af => af.FileName)
            .IsRequired()
            .HasMaxLength(255);
        
        modelBuilder.Entity<ActivityFile>()
            .Property(af => af.FilePath)
            .IsRequired();
        
        modelBuilder.Entity<ActivityFile>()
            .Property(af => af.FileType)
            .IsRequired()
            .HasMaxLength(50);
        
        modelBuilder.Entity<ActivityFile>()
            .HasOne(af => af.Activity)
            .WithMany(a => a.ActivityFiles)
            .HasForeignKey(af => af.ActivityId)
            .OnDelete(DeleteBehavior.Cascade);

        // News Configuration
        modelBuilder.Entity<News>()
            .HasKey(n => n.Id);
        
        modelBuilder.Entity<News>()
            .Property(n => n.Title)
            .IsRequired()
            .HasMaxLength(200);
        
        modelBuilder.Entity<News>()
            .Property(n => n.Content)
            .IsRequired();
        
        modelBuilder.Entity<News>()
            .HasOne(n => n.CreatedByUser)
            .WithMany(u => u.CreatedNews)
            .HasForeignKey(n => n.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        // Workshop Configuration
        modelBuilder.Entity<Workshop>()
            .HasKey(w => w.Id);
        
        modelBuilder.Entity<Workshop>()
            .Property(w => w.Title)
            .IsRequired()
            .HasMaxLength(200);
        
        modelBuilder.Entity<Workshop>()
            .Property(w => w.Description)
            .IsRequired();
        
        modelBuilder.Entity<Workshop>()
            .Property(w => w.Location)
            .HasMaxLength(200);
        
        modelBuilder.Entity<Workshop>()
            .HasOne(w => w.CreatedByUser)
            .WithMany(u => u.CreatedWorkshops)
            .HasForeignKey(w => w.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        // Feedback Configuration
        modelBuilder.Entity<Feedback>()
            .HasKey(f => f.Id);

        modelBuilder.Entity<Feedback>()
            .HasOne(f => f.Activity)
            .WithMany()
            .HasForeignKey(f => f.ActivityId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Feedback>()
            .HasOne(f => f.User)
            .WithMany()
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // ActivityLog Configuration
        modelBuilder.Entity<ActivityLog>()
            .HasKey(al => al.Id);

        modelBuilder.Entity<ActivityLog>()
            .HasOne(al => al.User)
            .WithMany()
            .HasForeignKey(al => al.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        ApplySeedData(modelBuilder);
    }

    private void ApplySeedData(ModelBuilder modelBuilder)
    {
        // Seed Universities (25 universities)
        var universities = new List<University>
        {
            new University { Id = 1, Name = "دانشگاه علوم پزشکی تهران", Province = "تهران", City = "تهران", IsActive = true, CreatedAt = DateTime.UtcNow },
            new University { Id = 2, Name = "دانشگاه علوم پزشکی شهید بهشتی", Province = "تهران", City = "تهران", IsActive = true, CreatedAt = DateTime.UtcNow },
            new University { Id = 3, Name = "دانشگاه علوم پزشکی ایران", Province = "تهران", City = "تهران", IsActive = true, CreatedAt = DateTime.UtcNow },
            new University { Id = 4, Name = "دانشگاه علوم پزشکی بقیه الله", Province = "تهران", City = "تهران", IsActive = true, CreatedAt = DateTime.UtcNow },
            new University { Id = 5, Name = "دانشگاه علوم پزشکی البرز", Province = "تهران", City = "کرج", IsActive = true, CreatedAt = DateTime.UtcNow },
            new University { Id = 6, Name = "دانشگاه آزاد اسلامی تهران", Province = "تهران", City = "تهران", IsActive = true, CreatedAt = DateTime.UtcNow },
            new University { Id = 7, Name = "دانشگاه آزاد اسلامی آمل", Province = "مازندران", City = "آمل", IsActive = true, CreatedAt = DateTime.UtcNow },
            new University { Id = 8, Name = "دانشگاه آزاد اسلامی دامغان", Province = "سمنان", City = "دامغان", IsActive = true, CreatedAt = DateTime.UtcNow },
            new University { Id = 9, Name = "دانشگاه علوم پزشکی شیراز", Province = "فارس", City = "شیراز", IsActive = true, CreatedAt = DateTime.UtcNow },
            new University { Id = 10, Name = "دانشگاه علوم پزشکی اصفهان", Province = "اصفهان", City = "اصفهان", IsActive = true, CreatedAt = DateTime.UtcNow },
            new University { Id = 11, Name = "دانشگاه علوم پزشکی بندرعباس", Province = "هرمزگان", City = "بندرعباس", IsActive = true, CreatedAt = DateTime.UtcNow },
            new University { Id = 12, Name = "دانشگاه علوم پزشکی گیلان", Province = "گیلان", City = "رشت", IsActive = true, CreatedAt = DateTime.UtcNow },
            new University { Id = 13, Name = "دانشگاه علوم پزشکی مازندران", Province = "مازندران", City = "ساری", IsActive = true, CreatedAt = DateTime.UtcNow },
            new University { Id = 14, Name = "دانشگاه علوم پزشکی ارومیه", Province = "اردبیل", City = "ارومیه", IsActive = true, CreatedAt = DateTime.UtcNow },
            new University { Id = 15, Name = "دانشگاه علوم پزشکی تبریز", Province = "اردبیل", City = "تبریز", IsActive = true, CreatedAt = DateTime.UtcNow },
            new University { Id = 16, Name = "دانشگاه علوم پزشکی زنجان", Province = "زنجان", City = "زنجان", IsActive = true, CreatedAt = DateTime.UtcNow },
            new University { Id = 17, Name = "دانشگاه علوم پزشکی زابل", Province = "سیستان و بلوچستان", City = "زابل", IsActive = true, CreatedAt = DateTime.UtcNow },
            new University { Id = 18, Name = "دانشگاه علوم پزشکی مشهد", Province = "خراسان رضوی", City = "مشهد", IsActive = true, CreatedAt = DateTime.UtcNow },
            new University { Id = 19, Name = "دانشگاه علوم پزشکی کرمان", Province = "کرمان", City = "کرمان", IsActive = true, CreatedAt = DateTime.UtcNow },
            new University { Id = 20, Name = "دانشگاه علوم پزشکی کرمانشاه", Province = "کرمانشاه", City = "کرمانشاه", IsActive = true, CreatedAt = DateTime.UtcNow },
            new University { Id = 21, Name = "دانشگاه علوم پزشکی اردبیل", Province = "اردبیل", City = "اردبیل", IsActive = true, CreatedAt = DateTime.UtcNow },
            new University { Id = 22, Name = "دانشگاه علوم پزشکی اهواز", Province = "خوزستان", City = "اهواز", IsActive = true, CreatedAt = DateTime.UtcNow },
            new University { Id = 23, Name = "دانشگاه علوم پزشکی یزد", Province = "یزد", City = "یزد", IsActive = true, CreatedAt = DateTime.UtcNow },
            new University { Id = 24, Name = "دانشگاه علوم پزشکی همدان", Province = "همدان", City = "همدان", IsActive = true, CreatedAt = DateTime.UtcNow },
            new University { Id = 25, Name = "دانشگاه علوم پزشکی بیرجند", Province = "خراسان جنوبی", City = "بیرجند", IsActive = true, CreatedAt = DateTime.UtcNow }
        };
        modelBuilder.Entity<University>().HasData(universities);

        // Seed Organizations (8 organizations)
        var organizations = new List<Organization>
        {
            new Organization { Id = 1, Name = "سندیکای صاحبان صنایع داروهای انسانی ایران", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Organization { Id = 2, Name = "انجمن پخش دارو و مکمل‌های انسانی", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Organization { Id = 3, Name = "سندیکای تولیدکنندگان مکمل‌های رژیمی غذایی ایران", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Organization { Id = 4, Name = "انجمن پروبیوتیک و غذاهای فراسودمند", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Organization { Id = 5, Name = "انجمن صنایع آرایشی و بهداشتی ایران", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Organization { Id = 6, Name = "نسل زد داروسازی", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Organization { Id = 7, Name = "راسا (رصد اقتصاد سلامت ایران)", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Organization { Id = 8, Name = "بیوتک", IsActive = true, CreatedAt = DateTime.UtcNow }
        };
        modelBuilder.Entity<Organization>().HasData(organizations);

        // Seed Admin User
        var adminUser = new User
        {
            Id = 1,
            Username = "admin",
            Email = "admin@federation.ir",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            Role = FederationPlatform.Domain.Enums.UserRole.Admin,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        modelBuilder.Entity<User>().HasData(adminUser);

        // Seed Admin Profile
        var adminProfile = new UserProfile
        {
            Id = 1,
            UserId = 1,
            FirstName = "مدیر",
            LastName = "سیستم",
            UniversityId = 1,
            Faculty = "مدیریت",
            Major = "مدیریت سیستم",
            EnrollmentYear = 2020,
            Position = "مدیر کل",
            PhoneNumber = "09121234567"
        };
        modelBuilder.Entity<UserProfile>().HasData(adminProfile);
    }
}
