# راهنمای جریان کار توسعه
## Development Workflow Guide

---

## 🎯 دو روش برای توسعه

### روش 1: با Hot Reload (توصیه میشه) ⭐

**مزایا:**
- ✅ تغییرات خودکار اعمال میشه
- ✅ سریع‌تر
- ✅ مناسب برای تغییرات مکرر

**نحوه استفاده:**
```bash
# Windows
start-dev.bat

# Linux/Mac
./start-dev.sh
```

**چی میشه:**
1. SQL Server با Docker اجرا میشه
2. برنامه با `dotnet watch` اجرا میشه
3. هر تغییری که بدی، خودکار اعمال میشه

---

### روش 2: با Docker (کامل)

**مزایا:**
- ✅ محیط کامل (Database, Email, Redis)
- ✅ شبیه Production
- ✅ مناسب برای تست کامل

**نحوه استفاده:**
```bash
# Windows
start-local.bat

# Linux/Mac
./start-local.sh
```

**برای اعمال تغییرات:**
```bash
docker-compose -f docker-compose.dev.yml restart web
```

---

## 📝 انواع تغییرات و نحوه اعمال

### 1. تغییر در View ها (.cshtml)

**مثال:** تغییر در `Views/Home/Index.cshtml`

**با Hot Reload:**
```bash
# فقط فایل رو ذخیره کن
# تغییرات خودکار اعمال میشه
```

**با Docker:**
```bash
docker-compose -f docker-compose.dev.yml restart web
# یا
docker restart federation-web-dev
```

**زمان:** 5-10 ثانیه

---

### 2. تغییر در CSS/JavaScript

**مثال:** تغییر در `wwwroot/css/site.css`

**هر دو روش:**
```
فقط صفحه رو Refresh کن (F5)
```

**نکته:** اگر تغییرات رو نمیبینی:
```
Ctrl + F5 (Hard Refresh)
```

**زمان:** فوری

---

### 3. تغییر در Controller

**مثال:** تغییر در `Controllers/ActivityController.cs`

**با Hot Reload:**
```bash
# فقط فایل رو ذخیره کن
# برنامه خودکار Rebuild و Restart میشه
```

خروجی:
```
watch : File changed: Controllers/ActivityController.cs
watch : Building...
Build succeeded.
watch : Started
```

**با Docker:**
```bash
docker-compose -f docker-compose.dev.yml restart web
```

**زمان:** 10-15 ثانیه

---

### 4. تغییر در Service یا Repository

**مثال:** تغییر در `Services/ActivityService.cs`

**با Hot Reload:**
```bash
# خودکار اعمال میشه
```

**با Docker:**
```bash
docker-compose -f docker-compose.dev.yml restart web
```

**زمان:** 10-15 ثانیه

---

### 5. تغییر در Model یا Entity

**مثال:** اضافه کردن Property به `Activity.cs`

**مراحل:**
```bash
# 1. تغییر در Model
# 2. ساخت Migration
cd src/FederationPlatform.Web
dotnet ef migrations add AddNewPropertyToActivity

# 3. اعمال Migration
dotnet ef database update

# 4. Restart (اگر با Docker کار میکنی)
docker-compose -f docker-compose.dev.yml restart web
```

**زمان:** 20-30 ثانیه

---

### 6. اضافه کردن Package جدید

**مثال:** نصب `Newtonsoft.Json`

```bash
# 1. نصب Package
cd src/FederationPlatform.Web
dotnet add package Newtonsoft.Json

# 2. اگر با Hot Reload کار میکنی
# خودکار Restore میشه

# 3. اگر با Docker کار میکنی
docker-compose -f docker-compose.dev.yml down
docker-compose -f docker-compose.dev.yml up -d --build
```

**زمان:** 2-3 دقیقه (با Docker)

---

### 7. تغییر در appsettings.json

**مثال:** تغییر Connection String

**با Hot Reload:**
```bash
# فقط ذخیره کن
# خودکار Restart میشه
```

**با Docker:**
```bash
docker-compose -f docker-compose.dev.yml restart web
```

**زمان:** 5-10 ثانیه

---

## 🔄 دستورات مفید

### مشاهده لاگ‌ها

**با Hot Reload:**
```bash
# لاگ‌ها در Console نمایش داده میشه
```

**با Docker:**
```bash
# تمام لاگ‌ها
docker-compose -f docker-compose.dev.yml logs -f

# فقط Web
docker-compose -f docker-compose.dev.yml logs -f web

# 100 خط آخر
docker-compose -f docker-compose.dev.yml logs --tail=100 web
```

---

### Restart سریع

**فقط Web:**
```bash
docker-compose -f docker-compose.dev.yml restart web
```

**همه سرویس‌ها:**
```bash
docker-compose -f docker-compose.dev.yml restart
```

---

### پاک کردن و شروع مجدد

**با Docker:**
```bash
# پاک کردن همه چیز (Database هم پاک میشه)
docker-compose -f docker-compose.dev.yml down -v

# شروع مجدد
./start-local.bat
```

**با Hot Reload:**
```bash
# فقط Database رو پاک کن
cd src/FederationPlatform.Web
dotnet ef database drop --force
dotnet ef database update
```

---

### چک کردن وضعیت

**Docker:**
```bash
# لیست Container ها
docker-compose -f docker-compose.dev.yml ps

# مصرف منابع
docker stats federation-web-dev federation-sqlserver-dev
```

**برنامه:**
```bash
# Health Check
curl http://localhost:5000/health
```

---

## 🎨 جریان کار توصیه شده

### برای تغییرات Frontend (View, CSS, JS)

```bash
1. اجرا با Hot Reload: start-dev.bat
2. تغییرات رو بده
3. ذخیره کن
4. صفحه رو Refresh کن (F5)
```

**سریع و راحت!** ⚡

---

### برای تغییرات Backend (Controller, Service)

```bash
1. اجرا با Hot Reload: start-dev.bat
2. تغییرات رو بده
3. ذخیره کن
4. منتظر Rebuild بمون (10 ثانیه)
5. صفحه رو Refresh کن
```

**خودکار!** 🎯

---

### برای تغییرات Database (Model, Migration)

```bash
1. تغییر در Model
2. dotnet ef migrations add MigrationName
3. dotnet ef database update
4. Restart (اگر لازمه)
```

---

### برای تست کامل (قبل از Commit)

```bash
1. اجرا با Docker: start-local.bat
2. تست همه قابلیت‌ها
3. چک کردن Email در MailHog
4. اجرای تست‌ها: dotnet test
```

---

## 🐛 مشکلات رایج و راه‌حل

### مشکل 1: تغییرات اعمال نمیشه

**راه‌حل:**
```bash
# با Hot Reload
Ctrl + C
dotnet watch run

# با Docker
docker-compose -f docker-compose.dev.yml restart web
```

---

### مشکل 2: خطای Compilation

**راه‌حل:**
```bash
# پاک کردن Build
dotnet clean
dotnet build

# یا
docker-compose -f docker-compose.dev.yml down
docker-compose -f docker-compose.dev.yml up -d --build
```

---

### مشکل 3: Database خراب شده

**راه‌حل:**
```bash
# پاک کردن و ساخت مجدد
cd src/FederationPlatform.Web
dotnet ef database drop --force
dotnet ef database update
```

---

### مشکل 4: Port اشغاله

**راه‌حل:**
```bash
# Windows
netstat -ano | findstr :5000
taskkill /PID <PID> /F

# Linux/Mac
lsof -i :5000
kill -9 <PID>
```

---

## 📊 مقایسه روش‌ها

| ویژگی | Hot Reload | Docker |
|-------|-----------|--------|
| سرعت اعمال تغییرات | ⚡ خیلی سریع | 🐢 کند |
| محیط کامل | ❌ فقط Web + DB | ✅ همه چیز |
| مصرف RAM | 💚 کم (2GB) | 💛 زیاد (4GB) |
| مناسب برای | توسعه روزمره | تست کامل |
| Email Testing | ❌ نیاز به تنظیم | ✅ MailHog |
| Redis Cache | ❌ ندارد | ✅ دارد |

---

## 🎯 توصیه نهایی

**برای توسعه روزمره:**
```bash
start-dev.bat
```
سریع، راحت، و تغییرات خودکار اعمال میشه! ⭐

**برای تست قبل از Commit:**
```bash
start-local.bat
```
محیط کامل و شبیه Production! 🚀

---

## 📞 کمک بیشتر

اگر سوالی داشتی:
- چک کن: `README_LOCAL_SETUP.md`
- چک کن: `docs/LOCAL_SETUP_GUIDE.md`
- لاگ‌ها رو ببین
- Health Check رو چک کن: http://localhost:5000/health

---

**موفق باشی!** 🎉
