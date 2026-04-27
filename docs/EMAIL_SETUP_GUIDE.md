# راهنمای راه‌اندازی سرویس ایمیل
## Email Service Setup Guide

---

## برای محیط Development (توسعه)

### استفاده از MailHog (پیش‌فرض)

وقتی با Docker کار میکنید، MailHog به صورت خودکار راه‌اندازی میشه:

```bash
docker-compose -f docker-compose.dev.yml up -d
```

**دسترسی به MailHog:**
- URL: http://localhost:8025
- SMTP Port: 1025

همه ایمیل‌هایی که برنامه میفرسته، در رابط وب MailHog نمایش داده میشه.

**نیازی به تنظیم خاصی نیست!** ✅

---

## برای محیط Production (تولید)

### گزینه 1: Gmail (برای تست و پروژه‌های کوچک)

#### مرحله 1: فعال کردن 2-Step Verification

1. برو به: https://myaccount.google.com/security
2. پیدا کن: "2-Step Verification"
3. فعالش کن و مراحل رو طی کن

#### مرحله 2: ساخت App Password

1. برو به: https://myaccount.google.com/apppasswords
2. در قسمت "Select app" انتخاب کن: **Mail**
3. در قسمت "Select device" انتخاب کن: **Other (Custom name)**
4. اسم بذار: **Federation Platform**
5. روی "Generate" کلیک کن
6. پسورد 16 رقمی رو کپی کن (مثل: `abcd efgh ijkl mnop`)

#### مرحله 3: تنظیم در پروژه

**ویرایش فایل `.env`:**
```bash
SMTP_SERVER=smtp.gmail.com
SMTP_PORT=587
SMTP_USERNAME=your-email@gmail.com
SMTP_PASSWORD=abcdefghijklmnop
```

**یا ویرایش `appsettings.Production.json`:**
```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "your-email@gmail.com",
    "SenderName": "پلتفرم فدراسیون",
    "Username": "your-email@gmail.com",
    "Password": "abcdefghijklmnop",
    "EnableSsl": true
  }
}
```

#### محدودیت‌های Gmail:
- ⚠️ حداکثر 500 ایمیل در روز
- ⚠️ ممکنه Gmail ایمیل‌ها رو به عنوان Spam علامت بزنه
- ✅ مناسب برای تست و پروژه‌های کوچک

---

### گزینه 2: سرویس‌های ایرانی (توصیه میشه)

#### 1. MailFa (میل‌فا)

**مشخصات:**
- وب‌سایت: https://mailfa.com
- قیمت: از 50,000 تومان/ماه
- پلن رایگان: 1000 ایمیل/ماه
- پشتیبانی فارسی

**مراحل راه‌اندازی:**

1. ثبت‌نام در MailFa
2. دریافت اطلاعات SMTP از پنل کاربری
3. تنظیم در پروژه:

```bash
# .env
SMTP_SERVER=smtp.mailfa.com
SMTP_PORT=587
SMTP_USERNAME=your-mailfa-username
SMTP_PASSWORD=your-mailfa-password
```

```json
// appsettings.Production.json
{
  "EmailSettings": {
    "SmtpServer": "smtp.mailfa.com",
    "SmtpPort": 587,
    "SenderEmail": "noreply@yourdomain.ir",
    "SenderName": "پلتفرم فدراسیون",
    "Username": "your-mailfa-username",
    "Password": "your-mailfa-password",
    "EnableSsl": true
  }
}
```

#### 2. Parsmail (پارس‌میل)

**مشخصات:**
- وب‌سایت: https://parsmail.com
- قیمت: از 30,000 تومان/ماه
- پشتیبانی فارسی

**تنظیمات:**
```bash
SMTP_SERVER=smtp.parsmail.com
SMTP_PORT=587
SMTP_USERNAME=your-parsmail-username
SMTP_PASSWORD=your-parsmail-password
```

---

### گزینه 3: سرویس‌های بین‌المللی

#### 1. SendGrid

**مشخصات:**
- وب‌سایت: https://sendgrid.com
- پلن رایگان: 100 ایمیل/روز
- قابل اعتماد و سریع

**مراحل راه‌اندازی:**

1. ثبت‌نام در SendGrid
2. ساخت API Key:
   - Settings → API Keys → Create API Key
   - دسترسی: Full Access
   - کپی کردن API Key

3. تنظیم در پروژه:

```bash
# .env
SMTP_SERVER=smtp.sendgrid.net
SMTP_PORT=587
SMTP_USERNAME=apikey
SMTP_PASSWORD=your-sendgrid-api-key
```

#### 2. Mailgun

**مشخصات:**
- وب‌سایت: https://mailgun.com
- پلن رایگان: 5000 ایمیل/ماه (3 ماه اول)

**تنظیمات:**
```bash
SMTP_SERVER=smtp.mailgun.org
SMTP_PORT=587
SMTP_USERNAME=postmaster@your-domain.mailgun.org
SMTP_PASSWORD=your-mailgun-password
```

#### 3. Amazon SES

**مشخصات:**
- بسیار ارزان: $0.10 به ازای هر 1000 ایمیل
- نیاز به AWS Account

**تنظیمات:**
```bash
SMTP_SERVER=email-smtp.us-east-1.amazonaws.com
SMTP_PORT=587
SMTP_USERNAME=your-ses-username
SMTP_PASSWORD=your-ses-password
```

---

## 🧪 تست ایمیل

### تست در محیط Development

1. اجرای برنامه:
```bash
docker-compose -f docker-compose.dev.yml up -d
```

2. ثبت‌نام یک کاربر جدید در: http://localhost:5000

3. چک کردن ایمیل در MailHog: http://localhost:8025

### تست در محیط Production

**روش 1: از طریق برنامه**
- ثبت‌نام کاربر جدید
- فراموشی رمز عبور
- تایید فعالیت (اعلان ایمیل)

**روش 2: تست دستی با C#**

ایجاد فایل `TestEmail.cs`:

```csharp
using System.Net;
using System.Net.Mail;

var smtpClient = new SmtpClient("smtp.gmail.com")
{
    Port = 587,
    Credentials = new NetworkCredential("your-email@gmail.com", "your-app-password"),
    EnableSsl = true,
};

var mailMessage = new MailMessage
{
    From = new MailAddress("your-email@gmail.com", "Federation Platform"),
    Subject = "Test Email",
    Body = "این یک ایمیل تستی است.",
    IsBodyHtml = false,
};

mailMessage.To.Add("recipient@example.com");

try
{
    smtpClient.Send(mailMessage);
    Console.WriteLine("✅ ایمیل با موفقیت ارسال شد!");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ خطا: {ex.Message}");
}
```

اجرا:
```bash
dotnet script TestEmail.cs
```

---

## 🔒 امنیت

### نکات مهم:

1. **هرگز پسورد رو در کد commit نکن!**
   - از `.env` استفاده کن
   - `.env` رو به `.gitignore` اضافه کن

2. **استفاده از Environment Variables:**
```bash
# در Docker
docker run -e SMTP_PASSWORD=your-password ...

# در Kubernetes
kubectl create secret generic email-secret --from-literal=password=your-password
```

3. **محدود کردن دسترسی:**
   - فقط از IP های مشخص اجازه ارسال بده
   - استفاده از API Key به جای پسورد

---

## 🐛 عیب‌یابی

### مشکل 1: ایمیل ارسال نمیشه

**بررسی لاگ‌ها:**
```bash
docker-compose logs web | grep -i email
```

**چک کردن تنظیمات:**
```bash
docker-compose exec web cat /app/appsettings.json | grep -A 10 EmailSettings
```

### مشکل 2: Gmail App Password کار نمیکنه

**راه‌حل:**
- مطمئن شو 2-Step Verification فعاله
- پسورد رو بدون فاصله وارد کن: `abcdefghijklmnop`
- از Less Secure Apps استفاده نکن (منسوخ شده)

### مشکل 3: ایمیل‌ها به Spam میرن

**راه‌حل:**
- استفاده از دامنه اختصاصی
- تنظیم SPF و DKIM records
- استفاده از سرویس‌های حرفه‌ای (SendGrid, Mailgun)

### مشکل 4: Connection Timeout

**بررسی:**
```bash
# تست اتصال به SMTP
telnet smtp.gmail.com 587

# یا با openssl
openssl s_client -connect smtp.gmail.com:587 -starttls smtp
```

**راه‌حل:**
- چک کردن فایروال
- چک کردن پورت (587 یا 465)
- مطمئن شو EnableSsl درسته

---

## 📊 مقایسه سرویس‌ها

| سرویس | قیمت | ایمیل رایگان | مناسب برای | پشتیبانی فارسی |
|-------|------|--------------|------------|----------------|
| MailHog | رایگان | نامحدود | Development | - |
| Gmail | رایگان | 500/روز | تست | ❌ |
| MailFa | 50K/ماه | 1000/ماه | تولید | ✅ |
| Parsmail | 30K/ماه | - | تولید | ✅ |
| SendGrid | رایگان | 100/روز | تولید | ❌ |
| Mailgun | رایگان | 5000/ماه | تولید | ❌ |
| AWS SES | $0.1/1000 | - | مقیاس بزرگ | ❌ |

---

## 🎯 توصیه نهایی

**برای Development:**
- استفاده از MailHog (پیش‌فرض) ✅

**برای تست و Staging:**
- Gmail با App Password

**برای Production:**
- پروژه کوچک: MailFa یا Parsmail
- پروژه متوسط: SendGrid یا Mailgun
- پروژه بزرگ: AWS SES

---

## 📞 پشتیبانی

اگر مشکلی داشتید:
1. لاگ‌ها رو چک کنید
2. تنظیمات SMTP رو بررسی کنید
3. تست اتصال انجام بدید
4. مستندات سرویس ایمیل رو بخونید

---

**آخرین بروزرسانی:** 25 آوریل 2026
