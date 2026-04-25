# Phase 2 Completion Report
## پلتفرم یکپارچه مدیریت فعالیت‌های کمیته دانشجویی فدراسیون اقتصاد سلامت

---

## ✅ Phase 2: Domain Layer - COMPLETED

**Completion Date:** April 25, 2025

### Summary
Phase 2 has been successfully completed. All domain entities, enumerations, and value objects have been created following Domain-Driven Design principles.

---

## Completed Tasks

### 2.1 Domain Entities ✅
- ✅ `User` - User authentication and authorization
- ✅ `UserProfile` - Extended user information
- ✅ `University` - University/medical school information
- ✅ `Organization` - Syndicate/organization information
- ✅ `Activity` - Student activity records
- ✅ `ActivityFile` - File attachments for activities
- ✅ `News` - News articles
- ✅ `Workshop` - Workshop/seminar information

### 2.2 Enumerations ✅
- ✅ `UserRole` - Admin, Representative, User
- ✅ `ActivityType` - Event, Workshop, Meeting, Project, Competition, Media, Other
- ✅ `ActivityStatus` - Pending, Approved, Rejected

### 2.3 Value Objects ✅
- ✅ `DateRange` - Immutable date range with validation

---

## Domain Model Overview

### Entity Relationships

```
User (1) ──────────── (1) UserProfile
  │
  ├── (1:N) ──────────────> Activity
  ├── (1:N) ──────────────> News
  └── (1:N) ──────────────> Workshop

University (1) ────── (N:1) UserProfile
  │
  └── (1:N) ──────────────> Activity

Organization (1) ──── (N:1) Activity

Activity (1) ────────── (N) ActivityFile
```

---

## Entity Details

### 1. User Entity
**Purpose:** Core authentication and user management

**Properties:**
- `Id` (int) - Primary key
- `Username` (string) - Unique username
- `Email` (string) - Email address
- `PasswordHash` (string) - Hashed password
- `Role` (UserRole) - User role (Admin/Representative/User)
- `IsActive` (bool) - Account status
- `CreatedAt` (DateTime) - Account creation date
- `UpdatedAt` (DateTime?) - Last update date

**Relationships:**
- One-to-One with `UserProfile`
- One-to-Many with `Activity`
- One-to-Many with `News`
- One-to-Many with `Workshop`

---

### 2. UserProfile Entity
**Purpose:** Extended user information and profile details

**Properties:**
- `Id` (int) - Primary key
- `UserId` (int) - Foreign key to User
- `FirstName` (string) - First name
- `LastName` (string) - Last name
- `UniversityId` (int?) - Foreign key to University (optional)
- `Faculty` (string) - Faculty name
- `Major` (string) - Major/field of study
- `EnrollmentYear` (int?) - Year of enrollment
- `Position` (string) - Position/role in committee
- `PhoneNumber` (string) - Contact number
- `ResumeUrl` (string?) - Resume file URL
- `ProfileImageUrl` (string?) - Profile picture URL

**Relationships:**
- Many-to-One with `User`
- Many-to-One with `University`

---

### 3. University Entity
**Purpose:** Medical university/school information

**Properties:**
- `Id` (int) - Primary key
- `Name` (string) - University name
- `Province` (string) - Province location
- `City` (string) - City location
- `LogoUrl` (string?) - University logo URL
- `Description` (string) - University description
- `IsActive` (bool) - Active status
- `CreatedAt` (DateTime) - Creation date
- `UpdatedAt` (DateTime?) - Last update date

**Relationships:**
- One-to-Many with `Activity`
- One-to-Many with `UserProfile`

**Universities (25 total):**
- علوم پزشکی تهران، شهید بهشتی، ایران، بقیه الله، البرز
- آزاد تهران، آزاد آمل، آزاد دامغان
- شیراز، اصفهان، بندرعباس، گیلان، مازندران
- ارومیه، تبریز، زنجان، زابل، مشهد
- کرمان، کرمانشاه، اردبیل، اهواز، یزد، همدان، بیرجند

---

### 4. Organization Entity
**Purpose:** Syndicate/organization information

**Properties:**
- `Id` (int) - Primary key
- `Name` (string) - Organization name
- `LogoUrl` (string?) - Organization logo URL
- `Description` (string) - Organization description
- `IsActive` (bool) - Active status
- `CreatedAt` (DateTime) - Creation date
- `UpdatedAt` (DateTime?) - Last update date

**Relationships:**
- One-to-Many with `Activity`

**Organizations (8 total):**
- سندیکای صاحبان صنایع داروهای انسانی ایران
- انجمن پخش دارو و مکمل‌های انسانی
- سندیکای تولیدکنندگان مکمل‌های رژیمی غذایی ایران
- انجمن پروبیوتیک و غذاهای فراسودمند
- انجمن صنایع آرایشی و بهداشتی ایران
- نسل زد داروسازی
- راسا (رصد اقتصاد سلامت ایران)
- بیوتک

---

### 5. Activity Entity
**Purpose:** Student activity records and submissions

**Properties:**
- `Id` (int) - Primary key
- `Title` (string) - Activity title
- `Description` (string) - Activity description
- `ActivityType` (ActivityType) - Type of activity
- `UniversityId` (int) - Foreign key to University
- `OrganizationId` (int) - Foreign key to Organization
- `UserId` (int) - Foreign key to User (creator)
- `StartDate` (DateTime) - Activity start date
- `EndDate` (DateTime) - Activity end date
- `Status` (ActivityStatus) - Approval status
- `CreatedAt` (DateTime) - Creation date
- `UpdatedAt` (DateTime?) - Last update date

**Relationships:**
- Many-to-One with `University`
- Many-to-One with `Organization`
- Many-to-One with `User`
- One-to-Many with `ActivityFile`

**Workflow:**
1. Representative creates activity (Status: Pending)
2. Admin reviews activity
3. Admin approves (Status: Approved) or rejects (Status: Rejected)
4. Only approved activities are visible publicly

---

### 6. ActivityFile Entity
**Purpose:** File attachments for activities

**Properties:**
- `Id` (int) - Primary key
- `ActivityId` (int) - Foreign key to Activity
- `FileName` (string) - Original file name
- `FilePath` (string) - File storage path
- `FileType` (string) - File MIME type
- `FileSize` (long) - File size in bytes
- `UploadedAt` (DateTime) - Upload timestamp
- `CreatedAt` (DateTime) - Creation date
- `UpdatedAt` (DateTime?) - Last update date

**Relationships:**
- Many-to-One with `Activity`

**Supported File Types:**
- Images: .jpg, .jpeg, .png
- Documents: .pdf, .doc, .docx
- Max size: 10MB (configurable)

---

### 7. News Entity
**Purpose:** News articles and announcements

**Properties:**
- `Id` (int) - Primary key
- `Title` (string) - News title
- `Content` (string) - News content
- `ImageUrl` (string?) - Featured image URL
- `PublishedAt` (DateTime) - Publication date
- `CreatedBy` (int) - Foreign key to User (admin)
- `IsPublished` (bool) - Publication status
- `CreatedAt` (DateTime) - Creation date
- `UpdatedAt` (DateTime?) - Last update date

**Relationships:**
- Many-to-One with `User` (Creator)

**Access:**
- Only admins can create/edit news
- All users can view published news

---

### 8. Workshop Entity
**Purpose:** Workshop and seminar information

**Properties:**
- `Id` (int) - Primary key
- `Title` (string) - Workshop title
- `Description` (string) - Workshop description
- `StartDate` (DateTime) - Workshop start date
- `EndDate` (DateTime) - Workshop end date
- `Location` (string) - Workshop location
- `RegistrationLink` (string?) - Registration URL
- `CreatedBy` (int) - Foreign key to User (admin)
- `IsActive` (bool) - Active status
- `MaxParticipants` (int) - Maximum participants
- `CreatedAt` (DateTime) - Creation date
- `UpdatedAt` (DateTime?) - Last update date

**Relationships:**
- Many-to-One with `User` (Creator)

**Access:**
- Only admins can create/edit workshops
- All users can view active workshops

---

## Enumerations

### UserRole
```csharp
public enum UserRole
{
    User = 0,           // Regular user (view only)
    Representative = 1, // University representative (can submit activities)
    Admin = 2          // System administrator (full access)
}
```

### ActivityType
```csharp
public enum ActivityType
{
    Event = 0,        // رویداد
    Workshop = 1,     // کارگاه
    Meeting = 2,      // جلسه
    Project = 3,      // پروژه
    Competition = 4,  // مسابقه
    Media = 5,        // فعالیت رسانه‌ای
    Other = 6         // سایر
}
```

### ActivityStatus
```csharp
public enum ActivityStatus
{
    Pending = 0,   // در انتظار تایید
    Approved = 1,  // تایید شده
    Rejected = 2   // رد شده
}
```

---

## Value Objects

### DateRange
**Purpose:** Immutable date range with validation and utility methods

**Features:**
- Validates that end date >= start date
- Calculates duration in days
- Checks if a date is within range
- Checks if two date ranges overlap
- Immutable (cannot be changed after creation)

**Methods:**
- `DurationInDays` - Get duration in days
- `IsWithinRange(DateTime)` - Check if date is in range
- `Overlaps(DateRange)` - Check if ranges overlap
- `Equals(object)` - Value equality
- `GetHashCode()` - Hash code generation
- `ToString()` - String representation

**Usage Example:**
```csharp
var range = new DateRange(
    new DateTime(2025, 4, 1),
    new DateTime(2025, 4, 30)
);

int days = range.DurationInDays; // 29
bool isWithin = range.IsWithinRange(new DateTime(2025, 4, 15)); // true
```

---

## Design Principles Applied

### 1. Domain-Driven Design (DDD)
✅ Rich domain model with business logic
✅ Entities with clear identity
✅ Value objects for immutable concepts
✅ Enumerations for type safety

### 2. SOLID Principles
✅ Single Responsibility - Each entity has one purpose
✅ Open/Closed - Extensible through inheritance
✅ Liskov Substitution - BaseEntity can be substituted
✅ Interface Segregation - Will be applied in Application layer
✅ Dependency Inversion - No dependencies on infrastructure

### 3. Clean Architecture
✅ Domain layer has no external dependencies
✅ Pure C# code with no framework references
✅ Business rules encapsulated in domain
✅ Infrastructure concerns separated

---

## Files Created

### Entities (9 files)
1. `BaseEntity.cs` - Base class for all entities
2. `User.cs` - User entity
3. `UserProfile.cs` - User profile entity
4. `University.cs` - University entity
5. `Organization.cs` - Organization entity
6. `Activity.cs` - Activity entity
7. `ActivityFile.cs` - Activity file entity
8. `News.cs` - News entity
9. `Workshop.cs` - Workshop entity

### Enums (3 files)
10. `UserRole.cs` - User role enumeration
11. `ActivityType.cs` - Activity type enumeration
12. `ActivityStatus.cs` - Activity status enumeration

### Value Objects (1 file)
13. `DateRange.cs` - Date range value object

**Total Files:** 13

---

## Next Steps - Phase 3: Application Layer

### Immediate Next Tasks
1. Create repository interfaces for all entities
2. Define DTOs (Data Transfer Objects)
3. Implement service interfaces and implementations
4. Create FluentValidation validators
5. Set up AutoMapper profiles

---

## Validation Rules (To be implemented in Phase 3)

### User
- Username: Required, 3-50 characters, unique
- Email: Required, valid email format, unique
- Password: Required, minimum 8 characters, complexity rules

### Activity
- Title: Required, 5-200 characters
- Description: Required, 10-5000 characters
- StartDate: Required, cannot be in the past
- EndDate: Required, must be >= StartDate
- University: Required
- Organization: Required

### UserProfile
- FirstName: Required, 2-50 characters
- LastName: Required, 2-50 characters
- PhoneNumber: Valid phone format

### News
- Title: Required, 5-200 characters
- Content: Required, 10-10000 characters

### Workshop
- Title: Required, 5-200 characters
- Description: Required, 10-5000 characters
- MaxParticipants: Required, > 0

---

## Database Schema (To be created in Phase 4)

### Tables to be created:
- Users
- UserProfiles
- Universities
- Organizations
- Activities
- ActivityFiles
- News
- Workshops

### Indexes to be created:
- Users: Username, Email (unique)
- Activities: UniversityId, OrganizationId, UserId, Status
- UserProfiles: UserId (unique), UniversityId

---

## Notes

### Design Decisions
1. **BaseEntity:** All entities inherit from BaseEntity for common properties (Id, CreatedAt, UpdatedAt)
2. **Navigation Properties:** Configured for EF Core relationships
3. **Nullable Types:** Used appropriately for optional fields
4. **String Initialization:** Empty strings used to avoid null reference warnings
5. **Collection Initialization:** Collections initialized to empty lists

### Future Enhancements
- Add soft delete functionality (IsDeleted flag)
- Add audit trail (CreatedBy, UpdatedBy)
- Add versioning for entities
- Add domain events for complex workflows

---

## Status: ✅ PHASE 2 COMPLETE

**Ready to proceed to Phase 3: Application Layer Development**

---

**Report Generated:** April 25, 2025  
**Version:** 1.0  
**Status:** Complete ✅
