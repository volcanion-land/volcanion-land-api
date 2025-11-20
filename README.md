# Real Estate Platform - Clean Architecture

[![.NET](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15+-blue)](https://www.postgresql.org/)
[![Redis](https://img.shields.io/badge/Redis-7+-red)](https://redis.io/)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)

Nền tảng bất động sản hiện đại được xây dựng với ASP.NET Core 8.0, tuân thủ nguyên tắc Clean Architecture.

## 🏗️ Kiến trúc

Dự án được tổ chức theo **Clean Architecture** với 4 layers riêng biệt:

```
src/
├── RealEstatePlatform.Domain/          # Core business entities & interfaces
├── RealEstatePlatform.Application/     # Business logic & DTOs
├── RealEstatePlatform.Infrastructure/  # Data access & external services
└── RealEstatePlatform.API/             # REST API & SignalR hubs
```

### Dependencies Flow
```
API → Infrastructure → Application → Domain
                    ↘     ↓
                      Domain (no dependencies)
```

## ✨ Tính năng chính

### Người dùng
- 🔐 Đăng ký, đăng nhập với JWT Authentication
- 🏠 Đăng tin bất động sản (Bán/Cho thuê)
- 🔍 Tìm kiếm nâng cao với filters
- ⭐ Yêu thích & theo dõi tin đăng
- 💬 Chat realtime với người bán
- 📊 Thống kê lượt xem
- ⚡ Gói tin VIP để tăng độ ưu tiên

### Admin
- 👥 Quản lý người dùng & phân quyền
- 📝 Duyệt/từ chối tin đăng
- 🎯 Quản lý Banner & FAQ
- 📈 Dashboard thống kê
- ⚙️ Cấu hình hệ thống

### Realtime Features
- 💬 Chat 1-1 với SignalR
- 🔔 Thông báo realtime
- 📍 Tìm kiếm theo vị trí với PostGIS

## 🚀 Tech Stack

| Category | Technology |
|----------|-----------|
| **Backend** | ASP.NET Core 8.0 Web API |
| **Database** | PostgreSQL 15+ with PostGIS |
| **ORM** | Entity Framework Core 8.0 |
| **Cache** | Redis 7+ |
| **Realtime** | SignalR |
| **Authentication** | JWT Bearer |
| **Email** | MailKit |
| **Logging** | Serilog |
| **Monitoring** | Prometheus |
| **API Docs** | Swagger/OpenAPI |
| **Validation** | FluentValidation |
| **Mapping** | AutoMapper |

## 📋 Yêu cầu hệ thống

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL 15+](https://www.postgresql.org/download/) with PostGIS extension
- [Redis 7+](https://redis.io/download)
- [Docker](https://www.docker.com/get-started) (optional)

## ⚡ Quick Start

### 1. Clone repository
```bash
git clone https://github.com/yourusername/volcano-land.git
cd volcano-land
```

### 2. Khởi động với Docker (Recommended)
```bash
docker-compose up -d
```

### 3. Hoặc cài đặt thủ công

#### Cấu hình Database
```bash
# Tạo database PostgreSQL
createdb RealEstatePlatform

# Enable PostGIS extension
psql -d RealEstatePlatform -c "CREATE EXTENSION IF NOT EXISTS postgis;"
```

#### Update Connection String
Cập nhật `src/RealEstatePlatform.API/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Database=RealEstatePlatform;Username=postgres;Password=yourpassword"
  },
  "Redis": {
    "Configuration": "localhost:6379"
  }
}
```

#### Apply Migrations
```bash
cd src/RealEstatePlatform.API
dotnet ef database update --project ../RealEstatePlatform.Infrastructure
```

#### Run Application
```bash
dotnet run --project src/RealEstatePlatform.API
```

API sẽ chạy tại: `https://localhost:7001` hoặc `http://localhost:5001`

## 📚 API Documentation

Sau khi chạy ứng dụng, truy cập Swagger UI:
- **Swagger UI**: https://localhost:7001/swagger
- **Postman Collection**: Import file `RealEstatePlatform.postman_collection.json`

### Các endpoint chính

#### Authentication
- `POST /api/auth/register` - Đăng ký tài khoản
- `POST /api/auth/login` - Đăng nhập
- `POST /api/auth/refresh-token` - Làm mới token

#### Properties & Listings
- `GET /api/listings` - Danh sách tin đăng
- `GET /api/listings/{id}` - Chi tiết tin đăng
- `POST /api/listings` - Đăng tin mới (yêu cầu auth)
- `PUT /api/listings/{id}` - Cập nhật tin đăng
- `DELETE /api/listings/{id}` - Xóa tin đăng

#### Search
- `GET /api/search` - Tìm kiếm nâng cao
- `GET /api/search/nearby` - Tìm BĐS gần vị trí

#### Messages (Realtime)
- `POST /api/messages/send` - Gửi tin nhắn
- `GET /api/messages/conversations` - Danh sách cuộc trò chuyện
- **SignalR Hub**: `/hubs/chat`

## 🗂️ Database Schema

Hệ thống bao gồm 28 entities chính:

- **Users & Auth**: ApplicationUser, Role, UserRole
- **Properties**: Property, PropertyListing, PropertyImage
- **Location**: Ward, District, Category
- **Features**: Amenity, Review, FavoriteListing
- **Communication**: Message, Conversation, Notification
- **Content**: BlogPost, Banner, FAQ
- **System**: SystemConfiguration, ListingPackage

## 🔧 Configuration

### Environment Variables
```bash
# Database
DATABASE_URL=postgresql://user:pass@localhost:5432/RealEstatePlatform

# Redis
REDIS_URL=localhost:6379

# JWT
JWT_SECRET=your-super-secret-key-here
JWT_ISSUER=RealEstatePlatform
JWT_AUDIENCE=RealEstatePlatformUsers
JWT_EXPIRY_MINUTES=60

# Email (SMTP)
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USERNAME=your-email@gmail.com
SMTP_PASSWORD=your-app-password
SMTP_FROM=noreply@realestate.com

# Storage
UPLOAD_PATH=wwwroot/uploads
MAX_FILE_SIZE=10485760
```

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true /p:CoverageReportsDirectory=./coverage
```

## 📊 Monitoring

### Health Checks
- **Endpoint**: `/health`
- Kiểm tra: Database, Redis, External Services

### Metrics (Prometheus)
- **Endpoint**: `/metrics`
- Metrics: Request count, duration, error rate

### Logging
- Console (Development)
- File logs: `logs/log-{Date}.txt`
- Optional: Elasticsearch integration

## 🐳 Docker Support

### Build Image
```bash
docker build -t realestate-api:latest .
```

### Run with Docker Compose
```bash
# Start all services
docker-compose up -d

# View logs
docker-compose logs -f api

# Stop services
docker-compose down
```

Services được expose:
- **API**: http://localhost:5001
- **PostgreSQL**: localhost:5432
- **Redis**: localhost:6379
- **Prometheus**: http://localhost:9090

## 📁 Project Structure

```
volcano-land/
├── src/
│   ├── RealEstatePlatform.Domain/
│   │   ├── Entities/           # Domain entities
│   │   ├── Enums/              # Enumerations
│   │   └── Common/             # Base classes & interfaces
│   │
│   ├── RealEstatePlatform.Application/
│   │   ├── DTOs/               # Data Transfer Objects
│   │   ├── Interfaces/         # Service & Repository interfaces
│   │   ├── Services/           # Business logic implementation
│   │   ├── Mappings/           # AutoMapper profiles
│   │   └── Validators/         # FluentValidation rules
│   │
│   ├── RealEstatePlatform.Infrastructure/
│   │   ├── Data/               # DbContext & Configurations
│   │   ├── Repositories/       # Repository implementations
│   │   ├── Migrations/         # EF Core migrations
│   │   └── Services/           # External service implementations
│   │
│   └── RealEstatePlatform.API/
│       ├── Controllers/        # API Controllers
│       ├── Hubs/               # SignalR Hubs
│       ├── Middleware/         # Custom middleware
│       ├── BackgroundServices/ # Hosted services
│       └── Program.cs          # Application entry point
│
├── docker-compose.yml
├── Dockerfile
├── RealEstatePlatform.postman_collection.json
└── README.md
```

## 🤝 Contributing

1. Fork the project
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👥 Authors

- **Your Name** - *Initial work*

## 🙏 Acknowledgments

- ASP.NET Core Team
- Clean Architecture principles by Robert C. Martin
- PostgreSQL & PostGIS communities

## 📞 Support

- 📧 Email: support@realestate.com
- 🐛 Issues: [GitHub Issues](https://github.com/yourusername/volcano-land/issues)
- 📖 Documentation: [Wiki](https://github.com/yourusername/volcano-land/wiki)

---

Made with ❤️ using ASP.NET Core 8.0
