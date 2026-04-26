# Federation Platform - Deployment Guide
## راهنمای استقرار پلتفرم فدراسیون

---

## Table of Contents
1. [Prerequisites](#prerequisites)
2. [Environment Setup](#environment-setup)
3. [Docker Deployment](#docker-deployment)
4. [Manual Deployment](#manual-deployment)
5. [Database Migration](#database-migration)
6. [SSL/TLS Configuration](#ssltls-configuration)
7. [Monitoring and Logging](#monitoring-and-logging)
8. [Backup and Restore](#backup-and-restore)
9. [Troubleshooting](#troubleshooting)

---

## Prerequisites

### System Requirements
- **Operating System**: Linux (Ubuntu 20.04+ recommended) or Windows Server 2019+
- **CPU**: 2+ cores
- **RAM**: 4GB minimum, 8GB recommended
- **Storage**: 50GB minimum, SSD recommended
- **Network**: Static IP address, open ports 80, 443, 1433

### Software Requirements
- Docker 24.0+
- Docker Compose 2.20+
- .NET 8.0 SDK (for manual deployment)
- SQL Server 2019+ or Docker SQL Server
- Git

### Domain and SSL
- Registered domain name
- SSL certificate (Let's Encrypt recommended)

---

## Environment Setup

### 1. Clone Repository
```bash
git clone https://github.com/your-org/FederationPlatform.git
cd FederationPlatform
```

### 2. Configure Environment Variables
```bash
# Copy example environment file
cp .env.example .env

# Edit environment variables
nano .env
```

Required environment variables:
```env
# Database
DB_PASSWORD=YourStrong@Passw0rd
DB_NAME=FederationPlatformDb

# Email
SMTP_USERNAME=your-email@gmail.com
SMTP_PASSWORD=your-app-password
SMTP_SERVER=smtp.gmail.com
SMTP_PORT=587

# Application
ASPNETCORE_ENVIRONMENT=Production
DOMAIN_NAME=federation.ir
ADMIN_EMAIL=admin@federation.ir
```

### 3. Create Required Directories
```bash
mkdir -p backups
mkdir -p nginx/ssl
mkdir -p logs
```

---

## Docker Deployment

### Production Deployment

#### 1. Build and Start Containers
```bash
# Using deployment script (recommended)
./scripts/deploy.sh production

# Or manually
docker-compose up -d
```

#### 2. Verify Deployment
```bash
# Check container status
docker-compose ps

# Check logs
docker-compose logs -f web

# Health check
curl http://localhost:5000/health
```

#### 3. Run Database Migrations
```bash
docker-compose exec web dotnet ef database update
```

### Development Deployment

```bash
# Start development environment
docker-compose -f docker-compose.dev.yml up -d

# Access MailHog for email testing
# Web UI: http://localhost:8025
```

### Staging Deployment

```bash
# Copy staging environment file
cp .env.example .env.staging

# Edit staging configuration
nano .env.staging

# Deploy to staging
./scripts/deploy.sh staging
```

---

## Manual Deployment

### 1. Install .NET 8.0 SDK
```bash
# Ubuntu/Debian
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 8.0
```

### 2. Build Application
```bash
cd src/FederationPlatform.Web
dotnet publish -c Release -o /var/www/federation-platform
```

### 3. Configure SQL Server
```bash
# Install SQL Server
# Update connection string in appsettings.Production.json
```

### 4. Run Migrations
```bash
cd /var/www/federation-platform
dotnet ef database update
```

### 5. Configure Systemd Service
```bash
sudo nano /etc/systemd/system/federation-platform.service
```

```ini
[Unit]
Description=Federation Platform Web Application
After=network.target

[Service]
WorkingDirectory=/var/www/federation-platform
ExecStart=/usr/bin/dotnet /var/www/federation-platform/FederationPlatform.Web.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=federation-platform
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

```bash
# Enable and start service
sudo systemctl enable federation-platform
sudo systemctl start federation-platform
sudo systemctl status federation-platform
```

---

## Database Migration

### Initial Setup
```bash
# Create database and run migrations
docker-compose exec web dotnet ef database update
```

### Update Database Schema
```bash
# After code changes with new migrations
docker-compose exec web dotnet ef database update
```

### Rollback Migration
```bash
# Rollback to specific migration
docker-compose exec web dotnet ef database update <MigrationName>
```

### Generate Migration Script
```bash
# Generate SQL script for manual execution
dotnet ef migrations script -o migration.sql
```

---

## SSL/TLS Configuration

### Using Let's Encrypt with Certbot

#### 1. Install Certbot
```bash
sudo apt-get update
sudo apt-get install certbot
```

#### 2. Obtain Certificate
```bash
sudo certbot certonly --standalone -d federation.ir -d www.federation.ir
```

#### 3. Copy Certificates
```bash
sudo cp /etc/letsencrypt/live/federation.ir/fullchain.pem nginx/ssl/
sudo cp /etc/letsencrypt/live/federation.ir/privkey.pem nginx/ssl/
```

#### 4. Auto-Renewal
```bash
# Add to crontab
sudo crontab -e

# Add this line
0 0 1 * * certbot renew --quiet && docker-compose restart nginx
```

### Using Custom SSL Certificate

```bash
# Copy your certificate files
cp your-certificate.crt nginx/ssl/fullchain.pem
cp your-private-key.key nginx/ssl/privkey.pem

# Set proper permissions
chmod 600 nginx/ssl/privkey.pem
```

---

## Monitoring and Logging

### Application Logs

#### View Logs
```bash
# Real-time logs
docker-compose logs -f web

# Last 100 lines
docker-compose logs --tail=100 web

# Logs from specific time
docker-compose logs --since 2024-01-01T00:00:00 web
```

#### Log Files Location
- Container: `/app/Logs/`
- Host: `./logs/` (mounted volume)

### Health Checks

#### Endpoints
- **Application Health**: `http://localhost:5000/health`
- **Database Health**: Included in health endpoint
- **Disk Space**: Included in health endpoint
- **Memory Usage**: Included in health endpoint

#### Monitoring Script
```bash
#!/bin/bash
# health-monitor.sh

while true; do
    if ! curl -f http://localhost:5000/health > /dev/null 2>&1; then
        echo "Health check failed at $(date)" | mail -s "Federation Platform Alert" admin@federation.ir
    fi
    sleep 300
done
```

### Performance Monitoring

#### Docker Stats
```bash
# Real-time resource usage
docker stats federation-web federation-sqlserver
```

#### Database Performance
```bash
# Connect to SQL Server
docker exec -it federation-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd

# Check active connections
SELECT * FROM sys.dm_exec_sessions WHERE is_user_process = 1;

# Check long-running queries
SELECT * FROM sys.dm_exec_requests WHERE status = 'running';
```

---

## Backup and Restore

### Automated Backup

#### Create Backup
```bash
# Run backup script
./scripts/backup.sh
```

This creates:
- Database backup (`.bak`)
- Uploaded files backup (`.tar.gz`)
- Application logs backup (`.tar.gz`)
- Backup manifest (`.txt`)

#### Schedule Automated Backups
```bash
# Add to crontab
crontab -e

# Daily backup at 2 AM
0 2 * * * /path/to/FederationPlatform/scripts/backup.sh
```

### Restore from Backup

```bash
# List available backups
ls -lh backups/

# Restore specific backup
./scripts/restore.sh 20260425_120000
```

### Manual Database Backup

```bash
# Backup
docker exec federation-sqlserver /opt/mssql-tools/bin/sqlcmd \
    -S localhost -U sa -P YourStrong@Passw0rd \
    -Q "BACKUP DATABASE [FederationPlatformDb] TO DISK = N'/var/opt/mssql/manual_backup.bak'"

# Copy to host
docker cp federation-sqlserver:/var/opt/mssql/manual_backup.bak ./backups/
```

### Manual Database Restore

```bash
# Copy backup to container
docker cp ./backups/manual_backup.bak federation-sqlserver:/var/opt/mssql/

# Restore
docker exec federation-sqlserver /opt/mssql-tools/bin/sqlcmd \
    -S localhost -U sa -P YourStrong@Passw0rd \
    -Q "RESTORE DATABASE [FederationPlatformDb] FROM DISK = N'/var/opt/mssql/manual_backup.bak' WITH REPLACE"
```

---

## Troubleshooting

### Common Issues

#### 1. Container Won't Start

**Problem**: Web container fails to start

**Solution**:
```bash
# Check logs
docker-compose logs web

# Check if port is already in use
sudo netstat -tulpn | grep :5000

# Remove and recreate containers
docker-compose down
docker-compose up -d
```

#### 2. Database Connection Failed

**Problem**: Cannot connect to SQL Server

**Solution**:
```bash
# Check SQL Server status
docker-compose ps sqlserver

# Check SQL Server logs
docker-compose logs sqlserver

# Verify connection string
docker-compose exec web cat /app/appsettings.json | grep ConnectionStrings

# Test connection
docker exec federation-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -Q "SELECT 1"
```

#### 3. File Upload Fails

**Problem**: Cannot upload files

**Solution**:
```bash
# Check upload directory permissions
docker-compose exec web ls -la /app/wwwroot/uploads

# Fix permissions
docker-compose exec web chmod -R 755 /app/wwwroot/uploads

# Check disk space
df -h
```

#### 4. Email Not Sending

**Problem**: Emails are not being sent

**Solution**:
```bash
# Check email configuration
docker-compose exec web cat /app/appsettings.json | grep EmailSettings

# Check logs for email errors
docker-compose logs web | grep -i email

# Test SMTP connection
telnet smtp.gmail.com 587
```

#### 5. High Memory Usage

**Problem**: Application consuming too much memory

**Solution**:
```bash
# Check memory usage
docker stats federation-web

# Restart application
docker-compose restart web

# Check for memory leaks in logs
docker-compose logs web | grep -i "memory\|exception"
```

### Performance Optimization

#### 1. Enable Response Caching
```bash
# Already configured in appsettings.Production.json
# Verify Redis is running
docker-compose ps redis
```

#### 2. Database Optimization
```sql
-- Update statistics
EXEC sp_updatestats;

-- Rebuild indexes
ALTER INDEX ALL ON Activities REBUILD;
ALTER INDEX ALL ON Users REBUILD;
```

#### 3. Clean Up Old Logs
```bash
# Remove logs older than 30 days
find ./logs -name "*.log" -mtime +30 -delete
```

### Getting Help

- **Documentation**: Check `/docs` directory
- **Logs**: Always check application and container logs first
- **Health Check**: Use `/health` endpoint to diagnose issues
- **Support**: Contact support@federation.ir

---

## Security Checklist

- [ ] Change default passwords in `.env`
- [ ] Configure SSL/TLS certificates
- [ ] Enable firewall (UFW or iptables)
- [ ] Configure rate limiting in Nginx
- [ ] Set up automated backups
- [ ] Enable security headers
- [ ] Configure CORS properly
- [ ] Use strong JWT secrets
- [ ] Regularly update Docker images
- [ ] Monitor security logs
- [ ] Implement intrusion detection
- [ ] Regular security audits

---

## Production Checklist

- [ ] Environment variables configured
- [ ] SSL certificates installed
- [ ] Database backups scheduled
- [ ] Monitoring configured
- [ ] Log rotation enabled
- [ ] Health checks working
- [ ] Email service configured
- [ ] Domain DNS configured
- [ ] Firewall rules set
- [ ] Performance tested
- [ ] Security hardened
- [ ] Documentation updated

---

**Last Updated**: April 25, 2026
**Version**: 1.0
