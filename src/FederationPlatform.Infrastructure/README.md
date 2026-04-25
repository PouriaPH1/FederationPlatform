# FederationPlatform.Infrastructure

Infrastructure layer for the Federation Platform. Handles data access, file storage, and authentication/authorization.

## Architecture

```
FederationPlatform.Infrastructure
├── Data/                          # Database context and initialization
│   ├── ApplicationDbContext.cs    # EF Core DbContext
│   └── DbInitializer.cs          # Automatic initialization & seeding
├── Repositories/                 # Data access layer
│   ├── RepositoryBase.cs         # Generic repository base
│   ├── UserRepository.cs
│   ├── UserProfileRepository.cs
│   ├── UniversityRepository.cs
│   ├── OrganizationRepository.cs
│   ├── ActivityRepository.cs
│   ├── ActivityFileRepository.cs
│   ├── NewsRepository.cs
│   ├── WorkshopRepository.cs
│   └── UnitOfWork.cs             # Transaction & repository management
├── Identity/                      # Authentication & authorization
│   ├── IIdentityService.cs       # Identity service interface
│   ├── IdentityService.cs        # Authentication implementation
│   ├── ApplicationUserManager.cs # Custom user manager
│   └── ApplicationRoleManager.cs # Role management
├── Services/                      # Business services
│   ├── IFileService.cs           # File service interface
│   └── FileService.cs            # File upload/download handling
└── DependencyInjection.cs        # Service registration

```

## Installation & Setup

### Prerequisites
- .NET 8.0 SDK or later
- SQL Server (local or remote)

### Connection String

Update `appsettings.json` in `FederationPlatform.Web`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=FederationPlatformDb;Trusted_Connection=true;TrustServerCertificate=true"
  }
}
```

### Database Initialization

The database is automatically created and seeded on application startup via `DbInitializer`.

## Usage

### 1. Repositories

All repositories implement specialized query methods:

```csharp
// Inject IUnitOfWork
public class MyService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public MyService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task GetUserByUsername(string username)
    {
        var user = await _unitOfWork.Users.GetByUsernameAsync(username);
        return user;
    }
    
    public async Task GetPendingActivities()
    {
        var activities = await _unitOfWork.Activities.GetPendingAsync();
        return activities;
    }
    
    public async Task SaveChanges()
    {
        await _unitOfWork.SaveChangesAsync();
    }
}
```

### 2. Transactions

```csharp
public async Task ProcessActivityAsync(Activity activity)
{
    try
    {
        await _unitOfWork.BeginTransactionAsync();
        
        // Multiple operations
        await _unitOfWork.Activities.AddAsync(activity);
        await _unitOfWork.SaveChangesAsync();
        
        await _unitOfWork.CommitTransactionAsync();
    }
    catch
    {
        await _unitOfWork.RollbackTransactionAsync();
        throw;
    }
}
```

### 3. Authentication

```csharp
// Register
var (success, message) = await _identityService.RegisterAsync(
    "username", 
    "email@example.com", 
    "password"
);

// Login
var (success, user, message) = await _identityService.LoginAsync(
    "username", 
    "password"
);

// Promote to representative
await _identityService.PromoteToRepresentativeAsync(userId);

// Ban user
await _identityService.BanUserAsync(userId);
```

### 4. File Management

```csharp
// Upload profile image
var (success, filePath, error) = await _fileService.UploadProfileImageAsync(
    formFile, 
    userId
);

if (success)
{
    // Use filePath in database
    userProfile.ProfileImageUrl = filePath;
}

// Upload activity file
var (success, filePath, error) = await _fileService.UploadActivityFileAsync(
    formFile, 
    activityId
);

// Get file
var (success, fileContent, error) = await _fileService.GetFileAsync(filePath);

// Delete file
var (success, error) = await _fileService.DeleteFileAsync(filePath);
```

## Repository Methods

### UserRepository
- `GetByIdAsync(int id)`
- `GetByUsernameAsync(string username)`
- `GetByEmailAsync(string email)`
- `GetAllAsync()`
- `GetByRoleAsync(UserRole role)`
- `AddAsync(User user)`
- `UpdateAsync(User user)`
- `DeleteAsync(int id)`
- `ExistsAsync(int id)`
- `UsernameExistsAsync(string username)`
- `EmailExistsAsync(string email)`
- `CountAsync()`

### ActivityRepository
- All base methods plus:
- `GetByStatusAsync(ActivityStatus status)`
- `GetByUniversityAsync(int universityId)`
- `GetByUserAsync(int userId)`
- `GetByOrganizationAsync(int organizationId)`
- `GetApprovedAsync()`
- `GetPendingAsync()`
- `GetCountByStatusAsync(ActivityStatus status)`
- `GetCountByUniversityAsync(int universityId)`

### Other Repositories
- Similar specialized methods for each entity
- See individual repository files for complete documentation

## Configuration

### ApplicationDbContext

Configured with:
- SQL Server connection
- Fluent API for entity relationships
- Automatic timestamp tracking
- Cascade delete rules
- Unique constraints

### DbInitializer

Automatically:
- Creates database if it doesn't exist
- Runs pending migrations
- Seeds 25 universities
- Seeds 8 organizations
- Creates default admin user

### File Service

Configuration:
- Upload folder: `wwwroot/uploads/`
- Profile images: `wwwroot/uploads/profile-images/`
- Activity files: `wwwroot/uploads/activity-files/`
- Max file size: 10 MB (general), 5 MB (images)
- Allowed types: .pdf, .doc, .docx, .jpg, .jpeg, .png, .gif, .xls, .xlsx, .zip

## Security

### Password Security
- BCrypt hashing (work factor 12)
- Minimum 6 characters required
- Passwords never stored in plain text

### File Upload Security
- File type validation
- File size limits
- Unique file naming with Guid
- Organized storage by file type

### Database Security
- Unique constraints on email and username
- Foreign key relationships
- Cascade delete rules
- User status tracking (IsActive flag)

## Entity Relationships

```
User (1) ──────────── (1) UserProfile
  │
  ├── (1:N) ──────────────> Activity
  ├── (1:N) ──────────────> News
  └── (1:N) ──────────────> Workshop

University (1) ────── (N) UserProfile
  │
  └── (1:N) ──────────────> Activity

Organization (1) ──── (N) Activity

Activity (1) ────────── (N) ActivityFile
```

## Error Handling

The infrastructure layer provides Persian error messages:

```csharp
var (success, message) = await _identityService.RegisterAsync(...);

if (!success)
{
    // message example: "نام کاربری قبلاً ثبت شده است."
    Console.WriteLine(message);
}
```

## Best Practices

1. **Always use UnitOfWork** instead of calling repositories directly
2. **Always dispose** of UnitOfWork to release database connections
3. **Use transactions** for multi-step operations
4. **Check return values** from file operations
5. **Validate input** before repository operations
6. **Use async/await** for all I/O operations
7. **Log errors** for debugging and monitoring

## Testing

### Unit Tests
```csharp
[Fact]
public async Task RegisterAsync_WithValidData_ReturnsSuccess()
{
    // Arrange
    var identityService = new IdentityService(_unitOfWork);
    
    // Act
    var (success, message) = await identityService.RegisterAsync(
        "testuser", 
        "test@example.com", 
        "password123"
    );
    
    // Assert
    Assert.True(success);
}
```

## Migrations

See [MIGRATIONS_GUIDE.md](MIGRATIONS_GUIDE.md) for detailed migration instructions.

## Dependencies

- Entity Framework Core 8.0.4
- Microsoft.AspNetCore.Identity.EntityFrameworkCore 8.0.4
- BCrypt.Net-Next 4.0.3

## License

This project is part of the Federation Platform.

---

**For more information, see [PHASE4_COMPLETED.md](PHASE4_COMPLETED.md)**
