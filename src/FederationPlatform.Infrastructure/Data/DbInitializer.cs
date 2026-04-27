using FederationPlatform.Domain.Entities;
using FederationPlatform.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FederationPlatform.Infrastructure.Data;

public class DbInitializer
{
    public static async Task InitializeAsync(ApplicationDbContext context)
    {
        try
        {
            // Create database if it doesn't exist (without migrations)
            await context.Database.EnsureCreatedAsync();

            // Check if database already has data
            if (await context.Universities.AnyAsync())
                return;

            // Seed Universities
            var universities = GetUniversities();
            await context.Universities.AddRangeAsync(universities);

            // Seed Organizations
            var organizations = GetOrganizations();
            await context.Organizations.AddRangeAsync(organizations);

            // Seed Admin User
            var adminUser = GetAdminUser();
            await context.Users.AddAsync(adminUser);
            await context.SaveChangesAsync();

            // Seed Admin Profile
            var adminProfile = GetAdminProfile(adminUser.Id);
            await context.UserProfiles.AddAsync(adminProfile);

            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing database: {ex.Message}");
            throw;
        }
    }

    private static List<University> GetUniversities()
    {
        return new List<University>
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
    }

    private static List<Organization> GetOrganizations()
    {
        return new List<Organization>
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
    }

    private static User GetAdminUser()
    {
        return new User
        {
            Id = 1,
            Username = "admin",
            Email = "admin@federation.ir",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            Role = UserRole.Admin,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    private static UserProfile GetAdminProfile(int userId)
    {
        return new UserProfile
        {
            Id = 1,
            UserId = userId,
            FirstName = "مدیر",
            LastName = "سیستم",
            UniversityId = 1,
            Faculty = "مدیریت",
            Major = "مدیریت سیستم",
            EnrollmentYear = 2020,
            Position = "مدیر کل",
            PhoneNumber = "09121234567"
        };
    }
}
