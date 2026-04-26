# Federation Platform - Infrastructure Documentation
## مستندات زیرساخت پلتفرم فدراسیون

---

## Architecture Overview

### System Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                         Internet                             │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                    Nginx Reverse Proxy                       │
│              (SSL/TLS, Load Balancing, Caching)             │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│              ASP.NET Core Web Application                    │
│                  (Docker Container)                          │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  Presentation Layer (MVC Controllers & Views)        │  │
│  └──────────────────────┬───────────────────────────────┘  │
│  ┌──────────────────────▼───────────────────────────────┐  │
│  │  Application Layer (Services, DTOs, Validators)      │  │
│  └──────────────────────┬───────────────────────────────┘  │
│  ┌──────────────────────▼───────────────────────────────┐  │
│  │  Infrastructure Layer (EF Core, Identity, Email)     │  │
│  └──────────────────────┬───────────────────────────────┘  │
│  ┌──────────────────────▼───────────────────────────────┐  │
│  │  Domain Layer (Entities, Interfaces)                 │  │
│  └──────────────────────────────────────────────────────┘  │
└────────────────────────┬────────────────────────────────────┘
                         │
         ┌───────────────┼───────────────┐
         ▼               ▼               ▼
┌─────────────┐  ┌─────────────┐  ┌─────────────┐
│ SQL Server  │  │   Redis     │  │   File      │
│  Database   │  │   Cache     │  │  Storage    │
└─────────────┘  └─────────────┘  └─────────────┘
```

### Container Architecture

```
Docker Network: federation-network
├── nginx (Reverse Proxy)
│   ├── Port: 80, 443
│   └── Volumes: nginx.conf, SSL certificates
│
├── web (ASP.NET Core Application)
│   ├── Port: 8080
│   ├── Volumes: uploads, logs
│   └── Depends on: sqlserver
│
├── sqlserver (SQL Server 2022)
│   ├── Port: 1433
│   └── Volumes: database data
│
└── redis (Redis Cache)
    ├── Port: 6379
    └── Volumes: cache data
```

---

## Infrastructure Components

### 1. Web Application Container

**Image**: Custom built from Dockerfile
**Base**: `mcr.microsoft.com/dotnet/aspnet:8.0`

**Configuration**:
- **CPU**: 2 cores
- **Memory**: 2GB
- **Storage**: 10GB
- **Restart Policy**: unless-stopped

**Environment Variables**:
```env
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080
ConnectionStrings__DefaultConnection=...
EmailSettings__SmtpServer=...
```

**Health Check**:
- Endpoint: `/health`
- Interval: 30s
- Timeout: 10s
- Retries: 3

### 2. SQL Server Container

**Image**: `mcr.microsoft.com/mssql/server:2022-latest`

**Configuration**:
- **CPU**: 2 cores
- **Memory**: 4GB
- **Storage**: 20GB
- **Restart Policy**: unless-stopped

**Environment Variables**:
```env
ACCEPT_EULA=Y
SA_PASSWORD=YourStrong@Passw0rd
MSSQL_PID=Developer
```

**Volumes**:
- `sqlserver-data:/var/opt/mssql`

**Backup Strategy**:
- Daily automated backups
- Retention: 30 days
- Location: `/backups`

### 3. Redis Cache Container

**Image**: `redis:7-alpine`

**Configuration**:
- **CPU**: 1 core
- **Memory**: 512MB
- **Storage**: 1GB
- **Restart Policy**: unless-stopped

**Volumes**:
- `redis-data:/data`

**Persistence**:
- RDB snapshots enabled
- AOF disabled (for performance)

### 4. Nginx Reverse Proxy

**Image**: `nginx:alpine`

**Configuration**:
- **CPU**: 1 core
- **Memory**: 256MB
- **Restart Policy**: unless-stopped

**Features**:
- SSL/TLS termination
- HTTP/2 support
- Gzip compression
- Rate limiting
- Static file caching
- Security headers

**Volumes**:
- `./nginx/nginx.conf:/etc/nginx/nginx.conf`
- `./nginx/ssl:/etc/nginx/ssl`

---

## Network Configuration

### Docker Network

**Name**: `federation-network`
**Driver**: bridge
**Subnet**: Auto-assigned

**Internal Communication**:
- web → sqlserver:1433
- web → redis:6379
- nginx → web:8080

### Port Mapping

| Service    | Internal Port | External Port | Protocol |
|------------|---------------|---------------|----------|
| Nginx      | 80            | 80            | HTTP     |
| Nginx      | 443           | 443           | HTTPS    |
| Web        | 8080          | 5000          | HTTP     |
| SQL Server | 1433          | 1433          | TCP      |
| Redis      | 6379          | 6379          | TCP      |

### Firewall Rules

```bash
# Allow HTTP and HTTPS
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp

# Allow SSH (for management)
sudo ufw allow 22/tcp

# Block direct access to application port
sudo ufw deny 5000/tcp

# Enable firewall
sudo ufw enable
```

---

## Storage and Volumes

### Docker Volumes

| Volume Name      | Mount Point              | Purpose              | Size  |
|------------------|--------------------------|----------------------|-------|
| sqlserver-data   | /var/opt/mssql          | Database files       | 20GB  |
| uploads-data     | /app/wwwroot/uploads    | User uploads         | 10GB  |
| logs-data        | /app/Logs               | Application logs     | 5GB   |
| redis-data       | /data                   | Cache data           | 1GB   |

### File Upload Structure

```
wwwroot/uploads/
├── activities/
│   ├── 2026/
│   │   ├── 01/
│   │   ├── 02/
│   │   └── ...
├── profiles/
│   └── avatars/
├── news/
│   └── images/
└── temp/
```

### Log Structure

```
Logs/
├── federation-platform-20260425.log
├── federation-platform-20260426.log
└── ...
```

**Log Rotation**:
- Daily rotation
- Retention: 30 days
- Max size: 10MB per file

---

## Security Configuration

### SSL/TLS

**Protocol**: TLS 1.2, TLS 1.3
**Cipher Suites**: HIGH:!aNULL:!MD5
**Certificate**: Let's Encrypt or Custom

**Configuration** (nginx.conf):
```nginx
ssl_protocols TLSv1.2 TLSv1.3;
ssl_ciphers HIGH:!aNULL:!MD5;
ssl_prefer_server_ciphers on;
ssl_session_cache shared:SSL:10m;
```

### Security Headers

```nginx
add_header Strict-Transport-Security "max-age=31536000; includeSubDomains" always;
add_header X-Frame-Options "SAMEORIGIN" always;
add_header X-Content-Type-Options "nosniff" always;
add_header X-XSS-Protection "1; mode=block" always;
add_header Referrer-Policy "no-referrer-when-downgrade" always;
```

### Rate Limiting

**General Requests**: 10 requests/second
**Login Attempts**: 5 requests/minute

```nginx
limit_req_zone $binary_remote_addr zone=general:10m rate=10r/s;
limit_req_zone $binary_remote_addr zone=login:10m rate=5r/m;
```

### Database Security

- Strong SA password
- TrustServerCertificate enabled
- Network isolation via Docker network
- Regular security updates

---

## Monitoring and Observability

### Health Checks

**Application Health Endpoint**: `/health`

**Checks**:
1. Database connectivity
2. Disk space availability
3. Memory usage
4. Application responsiveness

**Response Format**:
```json
{
  "status": "Healthy",
  "checks": {
    "database": "Healthy",
    "diskSpace": "Healthy",
    "memory": "Healthy"
  },
  "duration": "00:00:00.1234567"
}
```

### Logging

**Log Levels**:
- Production: Information, Warning, Error
- Staging: Debug, Information, Warning, Error
- Development: All levels

**Log Destinations**:
1. Console (Docker logs)
2. File (rotating daily)
3. Optional: Sentry, Application Insights

**Log Format**:
```
[2026-04-25 14:30:45] [Information] User 'admin@federation.ir' logged in successfully
[2026-04-25 14:31:12] [Warning] High memory usage detected: 1.5GB
[2026-04-25 14:32:00] [Error] Failed to send email: SMTP connection timeout
```

### Metrics

**Application Metrics**:
- Request count
- Response time
- Error rate
- Active users
- Database query performance

**Infrastructure Metrics**:
- CPU usage
- Memory usage
- Disk I/O
- Network traffic
- Container health

**Monitoring Tools** (Optional):
- Prometheus + Grafana
- Azure Application Insights
- Datadog
- New Relic

---

## Backup and Disaster Recovery

### Backup Strategy

**Automated Backups**:
- **Frequency**: Daily at 2:00 AM
- **Retention**: 30 days
- **Location**: `/backups` directory

**Backup Components**:
1. Database (SQL Server .bak)
2. Uploaded files (.tar.gz)
3. Application logs (.tar.gz)
4. Configuration files

**Backup Script**: `scripts/backup.sh`

### Disaster Recovery Plan

**RTO (Recovery Time Objective)**: 4 hours
**RPO (Recovery Point Objective)**: 24 hours

**Recovery Steps**:
1. Provision new infrastructure
2. Restore database from backup
3. Restore uploaded files
4. Deploy application containers
5. Verify health checks
6. Update DNS if needed

**Recovery Script**: `scripts/restore.sh`

### High Availability (Future)

**Planned Improvements**:
- Database replication (SQL Server Always On)
- Load balancing (multiple web containers)
- Redis Sentinel for cache HA
- Geographic redundancy

---

## Scaling Strategy

### Vertical Scaling

**Current Resources**:
- Web: 2 CPU, 2GB RAM
- Database: 2 CPU, 4GB RAM
- Redis: 1 CPU, 512MB RAM

**Scaling Up**:
```yaml
# docker-compose.yml
services:
  web:
    deploy:
      resources:
        limits:
          cpus: '4'
          memory: 4G
```

### Horizontal Scaling

**Load Balancing**:
```yaml
services:
  web:
    deploy:
      replicas: 3
```

**Session Management**:
- Use Redis for distributed sessions
- Sticky sessions in load balancer

**Database Scaling**:
- Read replicas for reporting
- Connection pooling
- Query optimization

---

## CI/CD Pipeline

### GitHub Actions Workflow

**Stages**:
1. **Build and Test**
   - Restore dependencies
   - Build solution
   - Run unit tests
   - Run integration tests
   - Code coverage

2. **Security Scan**
   - Trivy vulnerability scan
   - SAST analysis
   - Dependency check

3. **Docker Build**
   - Build Docker image
   - Push to registry
   - Scan image

4. **Deploy**
   - Deploy to staging (develop branch)
   - Deploy to production (main branch)
   - Run health checks

**Deployment Environments**:
- **Staging**: Auto-deploy from `develop` branch
- **Production**: Auto-deploy from `main` branch with approval

---

## Cost Optimization

### Resource Optimization

**Current Monthly Costs** (Estimated):
- VPS/Cloud Server: $50-100
- Domain + SSL: $15
- Email Service: $10
- Monitoring (optional): $20
- **Total**: ~$95-145/month

**Optimization Tips**:
1. Use Docker resource limits
2. Enable response caching
3. Optimize database queries
4. Compress static assets
5. Use CDN for static files (future)

### Infrastructure as Code

**Docker Compose**: Infrastructure definition
**GitHub Actions**: CI/CD automation
**Scripts**: Deployment and maintenance automation

---

## Maintenance Schedule

### Daily
- Automated backups
- Log rotation
- Health check monitoring

### Weekly
- Review application logs
- Check disk space
- Monitor performance metrics

### Monthly
- Security updates
- Database maintenance (reindex, update stats)
- Backup verification
- Performance review

### Quarterly
- Disaster recovery drill
- Security audit
- Capacity planning review
- Documentation update

---

## Support and Contacts

**Technical Support**: support@federation.ir
**Emergency Contact**: +98-XXX-XXX-XXXX
**Documentation**: `/docs` directory
**Repository**: https://github.com/your-org/FederationPlatform

---

**Last Updated**: April 25, 2026
**Version**: 1.0
**Maintained By**: Federation Platform DevOps Team
