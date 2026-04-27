# راهنمای عیب‌یابی - برنامه استارت نمیشه
## Troubleshooting Guide

---

## ❌ مشکل: وقتی http://localhost:5000 رو باز میکنم، چیزی نمایش داده نمیشه

---

## 🔍 چک کردن وضعیت

### مرحله 1: چک کردن Docker

**در Windows PowerShell یا CMD:**

```cmd
docker ps
```

**اگر خطا داد:**
- Docker Desktop نصب نیست
- یا Docker Desktop اجرا نشده

**اگر لیست خالی بود:**
- Container ها اجرا نشدن

---

### مرحله 2: چک کردن .NET

```cmd
dotnet --version
```

**اگر خطا داد:**
- .NET SDK نصب نیست

---

## ✅ راه‌حل‌ها

### راه‌حل 1: استفاده از Docker (توصیه میشه)

#### گام 1: نصب Docker Desktop

1. دانلود از: https://www.docker.com/products/docker-desktop
2. نصب و Restart کامپیوتر
3. Docker Desktop رو باز کن و منتظر بمون تا کاملاً بالا بیاد

#### گام 2: چک کردن Docker

```cmd
docker --version
```

باید چیزی شبیه این ببینی:
```
Docker version 24.0.x, build xxxxx
```

#### گام 3: اجرای برنامه

**در Command Prompt یا PowerShell:**

```cmd
cd C:\Users\leon\Desktop\uni_project\FederationPlatform
start-local.bat
```

**منتظر بمون تا:**
- SQL Server بالا بیاد (30 ثانیه)
- Web Application بالا بیاد (20 ثانیه)
- Migration ها اجرا بشن

#### گام 4: چک کردن Container ها

```cmd
docker ps
```

باید 3-4 Container ببینی:
- federation-web-dev
- federation-sqlserver-dev
- mailhog

#### گام 5: باز کردن مرورگر

```
http://localhost:5000
```

---

### راه‌حل 2: استفاده از .NET مستقیم

#### گام 1: نصب .NET 8.0 SDK

1. دانلود از: https://dotnet.microsoft.com/download/dotnet/8.0
2. نصب کن
3. Restart کن Command Prompt

#### گام 2: چک کردن نصب

```cmd
dotnet --version
```

باید چیزی شبیه این ببینی:
```
8.0.xxx
```

#### گام 3: نصب SQL Server با Docker

```cmd
docker run -d --name sqlserver-dev ^
  -e "ACCEPT_EULA=Y" ^
  -e "SA_PASSWORD=Dev@Passw0rd123" ^
  -p 1433:1433 ^
  mcr.microsoft.com/mssql/server:2022-latest
```

#### گام 4: اجرای Migration

```cmd
cd C:\Users\leon\Desktop\uni_project\FederationPlatform\src\FederationPlatform.Web
dotnet ef database update
```

#### گام 5: اجرای برنامه

```cmd
dotnet run
```

یا با Hot Reload:

```cmd
dotnet watch run
```

#### گام 6: باز کردن مرورگر

```
http://localhost:5000
```

---

## 🐛 مشکلات رایج

### مشکل 1: Docker Desktop اجرا نمیشه

**علت:**
- WSL 2 نصب نیست (Windows)
- Virtualization غیرفعاله

**راه‌حل:**

1. فعال کردن Virtualization در BIOS
2. نصب WSL 2:

```cmd
wsl --install
```

3. Restart کامپیوتر

---

### مشکل 2: Port 5000 اشغاله

**چک کردن:**

```cmd
netstat -ano | findstr :5000
```

**راه‌حل:**

```cmd
# پیدا کردن PID از خروجی بالا
taskkill /PID <PID> /F
```

---

### مشکل 3: SQL Server Connection Error

**خطا:**
```
Cannot connect to SQL Server
```

**راه‌حل:**

```cmd
# چک کردن SQL Server
docker ps | findstr sqlserver

# اگر نبود، اجرا کن
docker start sqlserver-dev

# یا از اول بساز
docker run -d --name sqlserver-dev ^
  -e "ACCEPT_EULA=Y" ^
  -e "SA_PASSWORD=Dev@Passw0rd123" ^
  -p 1433:1433 ^
  mcr.microsoft.com/mssql/server:2022-latest
```

---

### مشکل 4: Migration Error

**خطا:**
```
Unable to create migration
```

**راه‌حل:**

```cmd
# نصب EF Core Tools
dotnet tool install --global dotnet-ef

# اجرای مجدد
cd src\FederationPlatform.Web
dotnet ef database update
```

---

### مشکل 5: Build Error

**خطا:**
```
Build failed
```

**راه‌حل:**

```cmd
# پاک کردن و Build مجدد
dotnet clean
dotnet restore
dotnet build
```

---

## 📋 چک‌لیست قبل از اجرا

- [ ] Docker Desktop نصب و اجرا شده
- [ ] یا .NET 8.0 SDK نصب شده
- [ ] Port 5000 آزاد است
- [ ] SQL Server در حال اجراست
- [ ] Migration ها اجرا شدن
- [ ] فایروال برنامه رو بلاک نکرده

---

## 🎯 دستورات مفید برای عیب‌یابی

### چک کردن Container ها

```cmd
docker ps -a
```

### مشاهده لاگ‌ها

```cmd
docker logs federation-web-dev
docker logs federation-sqlserver-dev
```

### Restart همه چیز

```cmd
docker-compose -f docker-compose.dev.yml down
docker-compose -f docker-compose.dev.yml up -d
```

### پاک کردن کامل و شروع مجدد

```cmd
docker-compose -f docker-compose.dev.yml down -v
start-local.bat
```

---

## 📞 اگر هنوز کار نکرد

1. **لاگ‌ها رو چک کن:**
```cmd
docker logs federation-web-dev
```

2. **خطاها رو کپی کن و بفرست**

3. **اطلاعات سیستم:**
```cmd
docker --version
dotnet --version
systeminfo | findstr /C:"OS"
```

---

## ✅ تست موفقیت

وقتی همه چیز درست کار کنه، باید:

1. **Docker Container ها اجرا باشن:**
```cmd
docker ps
```
باید 3-4 Container ببینی

2. **Health Check موفق باشه:**
```cmd
curl http://localhost:5000/health
```
باید پاسخ JSON بگیری

3. **صفحه اصلی باز بشه:**
```
http://localhost:5000
```
باید صفحه لاگین رو ببینی

---

**موفق باشی!** 🚀
