# Admin Dashboard Features - اضافات جدید

## 📋 خلاصه تغییرات

تمام قابلیت‌های مدیریت کاربران و فعالیت‌های نماینده‌ها به داشبورد ادمین اضافه شد.

---

## ✅ ویژگی‌های اضافه‌شده

### 1. **مسدود کردن کاربران (Ban User)**

#### 🔧 Endpoints و Services:
- **Controller:** `AdminController.BanUser(string userId)`
- **Service:** `UserService.BanUserAsync(int id)` 
- **View:** دکمه "مسدود" در جدول کاربران

#### 📨 Notifications:
- اطلاع‌رسانی در سیستم (`SendUserBannedNotificationAsync`)
- ایمیل اطلاع‌رسانی (`SendUserBannedEmailAsync`)

#### 📝 Activity Logging:
- ثبت تمام عملیات مسدود کردن در `ActivityLog`
- شامل شناسه ادمین، IP، User Agent

#### مثال استفاده:
```
POST /Admin/BanUser?userId=5
```

---

### 2. **فعال کردن کاربران مسدود (Activate User)**

#### 🔧 Endpoints و Services:
- **Controller:** `AdminController.ActivateUser(string userId)`
- **Service:** `UserService.ActivateUserAsync(int id)`
- **View:** دکمه "فعال" در جدول (برای کاربران مسدود)

#### 📨 Notifications:
- اطلاع‌رسانی در سیستم (`SendUserActivatedNotificationAsync`)
- ایمیل اطلاع‌رسانی (`SendUserActivatedEmailAsync`)

#### 📝 Activity Logging:
- ثبت تمام عملیات فعال‌سازی

#### مثال استفاده:
```
POST /Admin/ActivateUser?userId=5
```

---

### 3. **ترفیع کاربر به نماینده (Promote to Representative)**

#### 🔧 Endpoints و Services:
- **Controller:** `AdminController.PromoteToRepresentativeAsync(string userId)`
- **Service:** `UserService.PromoteToRepresentativeAsync(int id)`
- **View:** دکمه "ترفیع" در جدول (برای کاربران عادی)

#### 📨 Notifications:
- اطلاع‌رسانی در سیستم (`SendUserPromotedNotificationAsync`)
- ایمیل اطلاع‌رسانی (`SendUserPromotedEmailAsync`)
- پیام شامل نقش جدید و توضیحات

#### 📝 Activity Logging:
- ثبت تمام عملیات ترفیع

#### مثال استفاده:
```
POST /Admin/PromoteToRepresentative?userId=5
```

---

### 4. **نمایش وضعیت کاربر (Status Display)**

#### 🎨 UI Updates:
- ستون جدید "وضعیت" در جدول کاربران
- نشان‌دادن وضعیت فعال/مسدود با رنگ‌های مختلف
  - ✅ فعال (سبز)
  - ❌ مسدود (قرمز)

---

## 📁 فایل‌های تغییر‌یافته

### 1. **Controllers**
```
src/FederationPlatform.Web/Controllers/AdminController.cs
```
- ✅ اضافه شدن متد `BanUser(string userId)`
- ✅ اضافه شدن متد `ActivateUser(string userId)`
- ✅ اضافه شدن متد `PromoteToRepresentative(string userId)`
- ✅ بهتر شدن متد `DeleteUser` با Activity Logging

### 2. **Services - Interfaces**
```
src/FederationPlatform.Application/Services/IEmailService.cs
src/FederationPlatform.Application/Services/INotificationService.cs
```
- ✅ اضافه شدن `SendUserBannedEmailAsync()`
- ✅ اضافه شدن `SendUserActivatedEmailAsync()`
- ✅ اضافه شدن `SendUserPromotedEmailAsync()`
- ✅ اضافه شدن `SendUserBannedNotificationAsync()`
- ✅ اضافه شدن `SendUserActivatedNotificationAsync()`
- ✅ اضافه شدن `SendUserPromotedNotificationAsync()`

### 3. **Services - Implementations**
```
src/FederationPlatform.Infrastructure/Services/EmailService.cs
src/FederationPlatform.Application/Services/NotificationService.cs
```
- ✅ پیاده‌سازی تمام متدهای ایمیل جدید
- ✅ پیاده‌سازی تمام متدهای اطلاع‌رسانی جدید

### 4. **Views**
```
src/FederationPlatform.Web/Views/Admin/Users.cshtml
```
- ✅ اضافه شدن دکمه‌های جدید (Ban, Activate, Promote)
- ✅ اضافه شدن ستون وضعیت
- ✅ بهتر شدن نمایش وضعیت کاربران
- ✅ شرط‌های صحیح برای نمایش دکمه‌های مناسب

---

## 🔐 محدودیت‌های دسترسی (Access Control)

تمام Endpoints جدید با `[Authorize(Roles = "Admin")]` محافظت می‌شوند:

```csharp
[HttpPost]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> BanUser(string userId)
{
    // فقط Admin می‌تواند استفاده کند
}
```

---

## 📊 مثال جریان استفاده (Workflow)

### سناریو 1: مسدود کردن کاربری نامناسب
```
1. ادمین وارد داشبورد شود
2. به صفحه "مدیریت کاربران" برود
3. کاربر مربوط را پیدا کند
4. دکمه "مسدود" را کلیک کند
5. سیستم:
   - کاربر را مسدود کند (IsActive = false)
   - اطلاع‌رسانی به کاربر ارسال کند
   - ایمیل تنبیهی ارسال کند
   - عملیات را لاگ کند
```

### سناریو 2: ترفیع کاربر به نماینده
```
1. ادمین کاربر عادی را انتخاب کند
2. دکمه "ترفیع" را کلیک کند
3. سیستم:
   - نقش کاربر را به "Representative" تغییر دهد
   - اطلاع‌رسانی مبارک‌بادی ارسال کند
   - ایمیل خوش‌آمدگویی ارسال کند
   - عملیات را لاگ کند
```

---

## 📧 الگوهای ایمیل

### مسدود کردن
```
موضوع: ⚠️ حساب شما مسدود شد
محتوا: 
- دلیل مسدود کردن
- درخواست تماس با پشتیبانی
```

### فعال‌سازی
```
موضوع: ✅ حساب شما فعال شد
محتوا:
- تأیید فعال‌سازی
- درخواست برای ادامه کار
```

### ترفیع
```
موضوع: 🎉 ترفیع شغلی شما
محتوا:
- تبریک ترفیع
- نقش جدید (نماینده)
- توضیح مسئولیت‌های جدید
```

---

## 🧪 تست‌های پیشنهادی

```csharp
// تست مسدود کردن
[Fact]
public async Task BanUser_ValidUserId_UserBanned()
{
    // Arrange
    var userId = 1;
    
    // Act
    var result = await _adminController.BanUser(userId.ToString());
    
    // Assert
    Assert.NotNull(result);
    var user = await _userService.GetUserByIdAsync(userId);
    Assert.False(user.IsActive);
}

// تست ترفیع
[Fact]
public async Task PromoteToRepresentative_ValidUserId_UserPromoted()
{
    // Arrange
    var userId = 1;
    
    // Act
    var result = await _adminController.PromoteToRepresentative(userId.ToString());
    
    // Assert
    var user = await _userService.GetUserByIdAsync(userId);
    Assert.Equal("Representative", user.Role);
}
```

---

## 📋 Checklist پایان

- ✅ Endpoints تمام شده
- ✅ Services تمام شده
- ✅ Views تمام شده  
- ✅ Notifications تمام شده
- ✅ Email Services تمام شده
- ✅ Activity Logging تمام شده
- ✅ Access Control تمام شده
- ✅ No Build Errors

---

## 🚀 نحوه استفاده

1. **لاگین با حساب Admin**
2. **رفتن به Dashboard**
3. **کلیک بر "مدیریت کاربران"**
4. **انتخاب کاربر و کلیک بر دکمه مورد نیاز**

---

## 📝 توضیحات اضافی

### چرا IsActive استفاده شد؟
- بدون حذف‌کردن دائمی (soft delete)
- سهولت بازگرداندن کاربر
- حفظ تاریخچه

### چرا Activity Logging؟
- پیگیری کار ادمین‌ها
- امنیت و شفافیت
- Audit Trail

### چرا Notifications؟
- اطلاع فوری کاربر
- بهتر شدن تجربه کاربر
- ارتباط بهتر

---

## 📞 سوالات و مسائل

اگر مسئله‌ای پیش‌آمد:

1. **ایمیل ارسال نشود:** تنظیمات SMTP را بررسی کنید
2. **Notification ظاهر نشود:** مطمئن شوید `INotificationService` registered است
3. **دسترسی رد شود:** مطمئن شوید کاربر Admin است

---

**تاریخ تکمیل:** 28 اپریل 2026
**نسخه:** 1.0
