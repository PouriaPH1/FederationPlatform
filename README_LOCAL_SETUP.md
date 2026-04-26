# 🚀 Quick Start - Run Locally

## Easiest Way (Using Docker) - Recommended

### Windows Users

1. **Install Docker Desktop**
   - Download: https://www.docker.com/products/docker-desktop
   - Install and restart your computer
   - Make sure Docker Desktop is running

2. **Run the project**
   ```cmd
   start-local.bat
   ```

3. **Open your browser**
   - Go to: http://localhost:5000
   - Login with: `admin@federation.ir` / `Admin@123`

### Linux/Mac Users

1. **Install Docker**
   ```bash
   # Mac
   brew install docker
   
   # Ubuntu/Debian
   sudo apt-get install docker.io docker-compose
   ```

2. **Run the project**
   ```bash
   ./start-local.sh
   ```

3. **Open your browser**
   - Go to: http://localhost:5000
   - Login with: `admin@federation.ir` / `Admin@123`

---

## Manual Way (Without Docker)

### Prerequisites
- .NET 8.0 SDK: https://dotnet.microsoft.com/download/dotnet/8.0
- SQL Server or SQL Server Express

### Steps

1. **Install SQL Server** (if not installed)
   ```bash
   # Or use Docker for SQL Server only
   docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Dev@Passw0rd123" \
      -p 1433:1433 --name sqlserver-dev \
      -d mcr.microsoft.com/mssql/server:2022-latest
   ```

2. **Restore packages**
   ```bash
   dotnet restore FederationPlatform.sln
   ```

3. **Update database**
   ```bash
   cd src/FederationPlatform.Web
   dotnet ef database update
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Open browser**
   - Go to: http://localhost:5000

---

## What You Get

- ✅ Web Application on http://localhost:5000
- ✅ Email Testing UI on http://localhost:8025
- ✅ SQL Server Database
- ✅ Sample data with admin user

---

## Default Login Credentials

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@federation.ir | Admin@123 |
| Representative | rep@university1.ir | Rep@123 |
| User | user@university1.ir | User@123 |

---

## Useful Commands

### View Logs
```bash
docker-compose -f docker-compose.dev.yml logs -f
```

### Stop Everything
```bash
docker-compose -f docker-compose.dev.yml down
```

### Restart
```bash
docker-compose -f docker-compose.dev.yml restart
```

### Clean Start (Remove all data)
```bash
docker-compose -f docker-compose.dev.yml down -v
./start-local.sh  # or start-local.bat on Windows
```

---

## Troubleshooting

### Port 5000 is already in use
```bash
# Find and kill the process
# Windows
netstat -ano | findstr :5000

# Linux/Mac
lsof -i :5000
kill -9 <PID>
```

### Docker not starting
- Make sure Docker Desktop is running
- Restart Docker Desktop
- Check if you have enough RAM (need at least 4GB)

### Database connection error
```bash
# Check if SQL Server container is running
docker ps | grep sqlserver

# Restart SQL Server
docker-compose -f docker-compose.dev.yml restart sqlserver
```

### Need help?
Check the detailed guide: `docs/LOCAL_SETUP_GUIDE.md`

---

## Next Steps

1. ✅ Application is running
2. 📖 Explore the features
3. 🔧 Make changes and see them live (hot reload enabled)
4. 🧪 Run tests: `dotnet test`

---

**Happy Coding! 🎉**
