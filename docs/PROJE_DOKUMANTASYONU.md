# SAR Tracking System - Proje Dokümantasyonu

**Tarih:** 09 Ağustos 2025  
**Durum:** %100 Backend + Web UI Complete ✅  
**Süre:** 3 günlük hızlı geliştirme planı (TAMAMLANDI)

## 📋 Proje Özeti

Arama Kurtarma Ekip Üyesi Takip Sistemi - SAR operasyonlarında ekip üyelerinin sektörler arası hareketlerini takip eden web uygulaması.

## 🏗️ Mimari Yapı

### Clean Architecture Pattern
```
SAR.TrackingSystem/
├── src/
│   ├── SAR.TrackingSystem.Domain/          # Entities, Enums, Configuration
│   ├── SAR.TrackingSystem.Application/     # CQRS, Commands, Queries  
│   ├── SAR.TrackingSystem.Infrastructure/  # EF Core, Repositories
│   ├── SAR.TrackingSystem.Api/            # Carter API Endpoints
│   └── SAR.TrackingSystem.Web/            # MVC Web UI ✅
└── tests/
    └── SAR.TrackingSystem.UnitTests/       # Test Infrastructure ✅
```

## 📊 Domain Model

### Core Entities
- **Volunteer**: TcKimlik, FullName, TeamId, BloodType, Phone, Buddy1/2 (Ekip Üyesi)
- **Team**: A-D Timleri, Medikal, Lojistik, Yönetim (9 tim) - Constructor pattern
- **Sector**: BOO, E-1, E-2, E2-A, E2-B, DIŞ, ALAN_DIŞI, ÇIKIŞ (7 sektör) - Constructor pattern + `IsCriticalForBusinessRules`
- **Movement**: Hareket kaydı (From→To, DateTime, IsGroupMovement) - Static factory

### Business Rules (IMPLEMENTED ✅)
- **Rule 1 - İntikal**: ALAN_DIŞI → BOO (ilk giriş zorunlu)
- **Rule 2 - Hub Transfer**: Tüm sektör geçişleri BOO üzerinden (E-1→E-2 yasak, E-1→BOO→E-2 ✅)
- **Rule 3 - Çıkış**: Sadece BOO → ÇIKIŞ (sektörden direkt çıkış yasak)
- **Rule 4 - Grup**: IsGroupMovement=true + GroupId zorunlu

### Configuration-Based Approach ✅
```json
"SectorSettings": {
  "EntryCode": "ALAN_DIŞI",
  "HubCode": "BOO", 
  "ExitCode": "ÇIKIŞ"
}
```

## ✅ Tamamlanan Özellikler (%100)

### Backend API (Carter + MediatR) ✅
```
✅ /volunteers        - Full CRUD (GET, POST, PUT, DELETE)
✅ /teams            - Read-only (GET, GET/{id})  
✅ /sectors          - Read-only (GET, GET/{id})
✅ /movements        - Create + Read (POST, GET, GET/{id})
```

### Business Rules Validation ✅
```csharp
✅ Movement.BusinessRules.IsValidEntry()      - İntikal kontrolü
✅ Movement.BusinessRules.IsValidTransfer()   - Hub model kontrolü
✅ Movement.BusinessRules.IsValidExit()       - Çıkış kontrolü
✅ Movement.BusinessRules.IsValidGroupMovement() - Grup hareket
✅ CreateMovementCommandValidator             - FluentValidation async
```

### Database (SQLite + EF Core) ✅
```sql
✅ Volunteers table   - 126 volunteer seed data
✅ Teams table       - 9 team seed data  
✅ Sectors table     - 7 sector seed data + IsCriticalForBusinessRules
✅ Movements table   - Business rule validation
```

### Testing Infrastructure ✅
- **xUnit + Moq + FluentAssertions + InMemory EF**
- **Mock Data Factories**: Team, Sector, Volunteer, Movement
- **Unit Tests**: Domain business rules, Repository operations, CQRS commands
- **Integration Tests**: Real SQLite database operations

### Technical Stack ✅
- ✅ **Backend**: .NET 9, EF Core, SQLite
- ✅ **API**: Carter (Minimal APIs), OpenAPI/Swagger
- ✅ **CQRS**: MediatR + FluentValidation + Business Rules
- ✅ **Pattern**: Repository + Constructor/Factory Methods + Configuration
- ✅ **Data**: PaginationRequest/Response
- ✅ **Testing**: xUnit, Moq, FluentAssertions

### Domain Protection ✅
```csharp
/// BUSİNESS CRİTİCAL: Bu sektörler SAR operasyon kuralları için kritiktir:
/// - ALAN_DIŞI: İlk giriş noktası (Entry rule)
/// - BOO: Hub sektör (Transfer rule) 
/// - ÇIKIŞ: Çıkış noktası (Exit rule)
public bool IsCriticalForBusinessRules { get; set; }
```

## ✅ YENİ ÖZELLİKLER (Son Güncellemeler)

### Dashboard Geliştirmeleri ✅
- **Otomatik Güncelleme**: 30 saniyede bir AJAX ile yenileme
- **Son 5 Hareket**: Sistem durumu kısmında real-time hareket takibi
- **Hızlı İşlemler Menüsü**: Takım ve Sektör ekleme butonları
- **Terminoloji Güncellemesi**: "Gönüllü" → "Ekip Üyesi" tüm UI'da

### Web UI Complete Stack ✅
- **MVC Controllers**: Home, Volunteers, Movements, Teams, Sectors
- **Razor Views**: Bootstrap 5 + Custom SAR theme
- **API Integration**: HttpClient services with error handling
- **Form Validation**: FluentValidation + business rules display

### Web UI (MVC) ✅
```
✅ Dashboard           - Statistics, recent movements, auto-refresh (30s AJAX)
✅ Volunteer CRUD      - Create/Read/Update/Delete forms (Ekip Üyesi)
✅ Movement Entry      - Bireysel/Grup hareket kayıt formu
✅ Bootstrap UI        - Responsive design + SAR theme
✅ Quick Actions       - Dashboard shortcuts (Team/Sector create)
```

## 🧪 Test Architecture

### Test Structure
```
tests/SAR.TrackingSystem.UnitTests/
├── Factories/           # Mock data generators
│   ├── TeamMockFactory.cs      - Team(constructor) samples
│   ├── SectorMockFactory.cs    - Sector(constructor) samples  
│   ├── VolunteerMockFactory.cs - Volunteer.Create() samples
│   └── MovementMockFactory.cs  - Movement.Create() scenarios
├── Domain/              # Business logic tests
│   └── MovementBusinessRulesTests.cs - Rule validation
├── Infrastructure/      # Repository tests
│   └── VolunteerRepositoryTests.cs - CRUD with InMemory DB
├── Application/         # CQRS tests
│   └── CreateMovementCommandTests.cs - Command validation
└── Integration/         # Database tests
    └── DatabaseIntegrationTests.cs - Real SQLite operations
```

### Test Coverage
- ✅ Business Rules: Entry, Transfer, Exit, Group movement validation
- ✅ Repository Operations: InMemory database CRUD
- ✅ CQRS Validation: Command business rule integration
- ✅ Integration: Production database with mock data

## 🎯 Sonuç - Proje Tamamlandı 🎉

PROJE DURUMU: **%100 COMPLETE**
- ✅ Backend API + Business Rules 
- ✅ Database + Testing Infrastructure
- ✅ Web UI + Dashboard + AJAX
- ✅ Ekip Üyesi terminolojisi güncellemesi
- ✅ Production-ready SAR Tracking System

**İşletim Talimatları:**
1. API: `cd src/SAR.TrackingSystem.Api && dotnet run` (Port: 5039)
2. Web: `cd src/SAR.TrackingSystem.Web && dotnet run` (Port: 5257)
3. Test: `cd tests && dotnet test`

## 📝 Business Rules API Validation

### Movement Creation
```http
POST /movements
{
  "volunteerId": "guid",
  "fromSectorId": "guid", 
  "toSectorId": "guid",
  "isGroupMovement": false
}

// Validation Responses:
400: "İlk hareket ALAN_DIŞI'ndan BOO'ya yapılmalıdır."
400: "Sektör geçişleri BOO üzerinden yapılmalıdır."
400: "Çıkış sadece BOO'dan yapılabilir."
```

## 🗄️ Enhanced Database Schema

### Sectors Table
```sql
Id (GUID), Code (NVARCHAR), Name (NVARCHAR), 
IsEntryPoint (BIT), IsExitPoint (BIT), IsActive (BIT),
IsCriticalForBusinessRules (BIT) -- KORUMA ALANI
```

### Movements Table  
```sql
Id (GUID), VolunteerId (GUID), FromSectorId (GUID), ToSectorId (GUID),
MovementTime (DATETIME), Type (INT), IsGroupMovement (BIT), 
GroupId (GUID), Notes (NVARCHAR)
```

## 📚 Architecture & Patterns

### Domain-Driven Design ✅
- Rich domain entities with business logic
- Configuration-based rules (not hardcoded)
- Self-documenting business rules with XML docs
- Domain protection against critical data deletion

### CQRS + Repository ✅
- Command/Query separation
- Async repository pattern  
- FluentValidation with business rules
- Response mapping

### Clean Architecture ✅
- Domain → Application → Infrastructure → API
- Dependency inversion
- Configuration injection
- Testable business logic
- Comprehensive test coverage

## 🔧 Configuration

### appsettings.json ✅
```json
{
  "SectorSettings": {
    "EntryCode": "ALAN_DIŞI",
    "HubCode": "BOO",
    "ExitCode": "ÇIKIŞ"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=SarTrackingDb.db"
  }
}
```

### DI Registration ✅
```csharp
builder.Services.Configure<SectorConfiguration>(
    builder.Configuration.GetSection(SectorConfiguration.SectionName));
```

## 📞 Development Notes

- ✅ Business rules implemented with detailed XML documentation
- ✅ Config-based approach prevents hardcoding
- ✅ Critical sector protection implemented
- ✅ Comprehensive validation with meaningful error messages
- ✅ Complete testing infrastructure with mock factories
- ✅ Integration tests with production database
- ✅ Web UI development COMPLETE - Dashboard + AJAX + Ekip Üyesi terminology
- ✅ Production-ready SAR Tracking System

**Critical Success:** Backend + Testing infrastructure complete with comprehensive coverage

---

**Son Güncelleme:** 09 Ağustos 2025  
**Geliştirici:** AI Assistant  
**Durum:** Production-Ready SAR Tracking System COMPLETE