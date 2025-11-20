# 🚀 Quick Start Guide

Hướng dẫn nhanh để chạy Real Estate Platform trên máy local của bạn.

## 📋 Yêu cầu

Đảm bảo bạn đã cài đặt:

- ✅ [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- ✅ [PostgreSQL 15+](https://www.postgresql.org/download/)
- ✅ [Redis](https://redis.io/download) (hoặc dùng Docker)
- ✅ [Git](https://git-scm.com/downloads)

## 🎯 Cài đặt trong 5 phút

### Bước 1: Clone Repository

```bash
git clone https://github.com/yourusername/volcano-land.git
cd volcano-land
```

### Bước 2: Cấu hình Database

#### Option A: Sử dụng Docker (Recommended)

```bash
# Start PostgreSQL và Redis với Docker Compose
docker-compose up -d postgres redis

# Đợi 10 giây để services khởi động
timeout /t 10
```

#### Option B: Cài đặt thủ công

**PostgreSQL:**
```bash
# Tạo database
createdb RealEstatePlatform

# Kết nối và enable PostGIS extension
psql -d RealEstatePlatform
```

```sql
CREATE EXTENSION IF NOT EXISTS postgis;
\q
```

**Redis:**
```bash
# Windows: Download từ https://redis.io/download hoặc dùng WSL
# Linux/Mac:
redis-server
```

### Bước 3: Cấu hình Connection Strings

Mở file `src/RealEstatePlatform.API/appsettings.Development.json` và cập nhật:

```json
{
  "ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Port=5432;Database=RealEstatePlatform;Username=postgres;Password=postgres"
  },
  "Redis": {
    "Configuration": "localhost:6379"
  },
  "Jwt": {
    "Secret": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "RealEstatePlatform",
    "Audience": "RealEstatePlatformUsers",
    "ExpiryMinutes": 60
  }
}
```

### Bước 4: Apply Database Migrations

```bash
# Di chuyển vào thư mục API
cd src/RealEstatePlatform.API

# Chạy migrations
dotnet ef database update --project ../RealEstatePlatform.Infrastructure --startup-project .
```

> ⚠️ **Lưu ý**: Nếu gặp lỗi "No executable found", cài đặt EF Core tools:
> ```bash
> dotnet tool install --global dotnet-ef
> ```

### Bước 5: Build Solution

```bash
# Quay lại thư mục root
cd ../..

# Build toàn bộ solution
dotnet build volcano-land.sln
```

### Bước 6: Run Application

```bash
# Chạy API project
dotnet run --project src/RealEstatePlatform.API
```

Hoặc trong Visual Studio: Set `RealEstatePlatform.API` làm Startup Project và nhấn F5.

### Bước 7: Kiểm tra

API đang chạy tại:
- 🌐 HTTPS: https://localhost:7001
- 🌐 HTTP: http://localhost:5001
- 📖 Swagger: https://localhost:7001/swagger
- ❤️ Health Check: https://localhost:7001/health

## 🧪 Test API

### 1. Đăng ký tài khoản

```bash
curl -X POST https://localhost:7001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "Password123!",
    "fullName": "Nguyen Van A",
    "phoneNumber": "0912345678"
  }'
```

### 2. Đăng nhập

```bash
curl -X POST https://localhost:7001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "Password123!"
  }'
```

Response:
```json
{
  "success": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "refreshToken": "...",
    "expiresAt": "2024-01-20T10:00:00Z"
  }
}
```

### 3. Lấy danh sách tin đăng

```bash
# Không cần authentication
curl https://localhost:7001/api/listings
```

### 4. Tạo tin đăng mới (Yêu cầu auth)

```bash
curl -X POST https://localhost:7001/api/listings \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -H "Content-Type: application/json" \
  -d '{
    "propertyId": "...",
    "title": "Căn hộ 2PN view biển",
    "price": 5000000000,
    "listingType": "Sell",
    "description": "Căn hộ đẹp, view thoáng..."
  }'
```

## 🎨 Postman Collection

Import file `RealEstatePlatform.postman_collection.json` vào Postman để test tất cả endpoints.

1. Mở Postman
2. Click **Import** → **File** → Chọn `RealEstatePlatform.postman_collection.json`
3. Set biến `{{baseUrl}}` = `https://localhost:7001`
4. Test endpoint **Auth → Login** để lấy token
5. Token sẽ tự động lưu vào biến `{{token}}`

## 🔑 Tài khoản Admin mặc định

Sau khi chạy migrations, hệ thống tự động tạo admin:

- **Email**: admin@realestate.com
- **Password**: Admin@123
- **Role**: Admin

> 🔒 **Bảo mật**: Đổi mật khẩu admin ngay sau lần đăng nhập đầu tiên!

## 📊 Monitoring & Logs

### Health Check
```bash
curl https://localhost:7001/health
```

Response:
```json
{
  "status": "Healthy",
  "checks": [
    {"name": "Database", "status": "Healthy"},
    {"name": "Redis", "status": "Healthy"}
  ]
}
```

### Metrics (Prometheus)
```bash
curl https://localhost:7001/metrics
```

### Logs
Logs được lưu tại: `src/RealEstatePlatform.API/logs/log-{Date}.txt`

```bash
# Xem logs realtime (PowerShell)
Get-Content src/RealEstatePlatform.API/logs/log-20240120.txt -Wait -Tail 50
```

## 🐛 Troubleshooting

### ❌ Lỗi: "No connection could be made"

**Nguyên nhân**: PostgreSQL hoặc Redis chưa chạy

**Giải pháp**:
```bash
# Kiểm tra PostgreSQL
psql -U postgres -c "SELECT version();"

# Kiểm tra Redis
redis-cli ping
# Phải trả về: PONG
```

### ❌ Lỗi: "Login failed for user"

**Nguyên nhân**: Sai connection string

**Giải pháp**: Kiểm tra lại username/password trong `appsettings.Development.json`

### ❌ Lỗi: "A migration is already applied"

**Nguyên nhân**: Database đã có migrations

**Giải pháp**:
```bash
# Drop và tạo lại database
dropdb RealEstatePlatform
createdb RealEstatePlatform
psql -d RealEstatePlatform -c "CREATE EXTENSION postgis;"

# Apply migrations lại
dotnet ef database update --project src/RealEstatePlatform.Infrastructure --startup-project src/RealEstatePlatform.API
```

### ❌ Lỗi: "The type or namespace name 'AutoMapper' could not be found"

**Nguyên nhân**: Packages chưa được restore

**Giải pháp**:
```bash
dotnet restore volcano-land.sln
dotnet build volcano-land.sln
```

### ❌ Port 5001 hoặc 7001 đã được sử dụng

**Giải pháp**: Đổi port trong `src/RealEstatePlatform.API/Properties/launchSettings.json`:
```json
{
  "applicationUrl": "https://localhost:7002;http://localhost:5002"
}
```

## 🚀 Chạy với Docker (All-in-one)

```bash
# Build và start tất cả services
docker-compose up -d

# Xem logs
docker-compose logs -f api

# Dừng services
docker-compose down
```

Services:
- API: http://localhost:5001
- PostgreSQL: localhost:5432
- Redis: localhost:6379
- Prometheus: http://localhost:9090

## 📚 Tài liệu bổ sung

- 📖 [README.md](README.md) - Chi tiết về kiến trúc và tính năng
- 🔌 [API Documentation](https://localhost:7001/swagger) - Swagger UI
- 📬 Postman Collection - Import file JSON để test

## 💡 Tips

### Hot Reload
```bash
# Tự động rebuild khi code thay đổi
dotnet watch run --project src/RealEstatePlatform.API
```

### Debug trong VS Code
Thêm vào `.vscode/launch.json`:
```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": ".NET Core Launch (web)",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build",
      "program": "${workspaceFolder}/src/RealEstatePlatform.API/bin/Debug/net8.0/RealEstatePlatform.API.dll",
      "args": [],
      "cwd": "${workspaceFolder}/src/RealEstatePlatform.API",
      "env": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  ]
}
```

### Seed Data mẫu
```bash
# TODO: Implement seed data script
dotnet run --project src/RealEstatePlatform.API -- --seed
```

## ✅ Checklist

- [ ] Clone repository
- [ ] Cài đặt .NET 8.0 SDK
- [ ] Cài đặt PostgreSQL & PostGIS
- [ ] Cài đặt Redis
- [ ] Cập nhật connection strings
- [ ] Apply migrations
- [ ] Build solution thành công
- [ ] Chạy API
- [ ] Truy cập Swagger
- [ ] Test endpoint /health
- [ ] Đăng nhập bằng tài khoản admin
- [ ] Import Postman collection

## 🎉 Hoàn thành!

Bạn đã sẵn sàng để phát triển! Nếu gặp vấn đề:
- 🐛 [Report Issues](https://github.com/yourusername/volcano-land/issues)
- 💬 [Discussions](https://github.com/yourusername/volcano-land/discussions)
- 📧 Email: support@realestate.com

Happy Coding! 🚀
