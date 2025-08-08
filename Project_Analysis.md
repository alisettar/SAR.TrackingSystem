# SAR Tracking System - 3 Günlük Hızlı Geliştirme Planı

## Kesinleşen Kararlar

**Süre**: 3 gün  
**Tech Stack**: ASP.NET MVC + EF Core + SQL Server + IIS  
**Mevcut Varlıklar**: Clean Architecture template, CRUD patterns  
**Scope**: QR kod hariç temel functionality

---

## Domain Yapısı (Excel Analizinden)

### Entities
- **Volunteer**: TC, Ad-Soyad, Ekip, Kan Grubu, Telefon
- **Team**: A-D Timleri, Medikal, Lojistik, Yönetim vs.
- **Sector**: BOO, E-1, E-2, E2-A, E2-B, DIŞ, ALAN_DIŞI, ÇIKIŞ
- **Movement**: Gönüllü, Kaynak, Hedef, Zaman, IsGroupMovement, GroupId

### Business Rules
- **İntikal**: ALAN_DIŞI → BOO (ilk giriş)
- **Transfer**: Sektörler arası hareket
- **Çıkış**: Herhangi sektör → ÇIKIŞ
- **Grup Hareket**: IsGroupMovement=true, aynı GroupId

---

## 3 GÜNLÜK PLAN

### 🚀 GÜN 1: Foundation (8 saat)
**Template Adaptation + Data Setup**

#### AI Agent Görevleri:
1. **Template Agent** (2 saat)
   - Clean Architecture template → SAR.TrackingSystem
   - MVC + EF Core + SQL Server setup
   - Connection string configuration

2. **Entity Agent** (3 saat)
   - Entity models (Volunteer, Team, Sector, Movement)
   - DbContext + Navigation properties
   - Migrations create

3. **Migration Agent** (3 saat)
   - Excel data → SQL Server seed script
   - Initial data import (126 volunteers, teams, sectors)

**Çıktı**: Çalışan database + temel entities

---

### 🏗️ GÜN 2: CRUD + Business Logic (8 saat)
**Core Functionality**

#### AI Agent Görevleri:
4. **CRUD Agent** (4 saat)
   - Volunteer CRUD (mevcut pattern adapt)
   - Team, Sector management pages
   - Bootstrap UI integration

5. **Movement Agent** (4 saat)
   - Hareket formu: Bireysel/Grup seçimi
   - İntikal (ALAN_DIŞI→BOO), Transfer, Çıkış (→ÇIKIŞ)
   - IsGroupMovement flag + GroupId logic

**Çıktı**: Çalışan CRUD sayfaları + hareket kayıt

---

### 📊 GÜN 3: Reports + Deployment (8 saat)
**Dashboard + Production**

#### AI Agent Görevleri:
6. **Dashboard Agent** (3 saat)
   - Ana sayfa dashboard
   - Sayılar: İntikal sayısı, sektör dağılımı, ekip durumu
   - Simple charts/tables

7. **Report Agent** (3 saat)
   - 3 temel rapor sayfası
   - Filter/search basics
   - Grup hareket görüntüleme

8. **Deploy Agent** (2 saat)
   - IIS deployment config
   - Web.config production settings
   - Database connection setup

**Çıktı**: Production-ready sistem

---

## Minimal Tech Stack

### Backend
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="9.0" />
<PackageReference Include="EPPlus" Version="7.0" /> <!-- Excel export -->
```

### Database Strategy
- **Pure EntityFramework Core** (Repository pattern yok)
- Direct DbContext usage
- LINQ queries
- SQL Server on Windows

### Deployment
- **Windows Server** + IIS
- SQL Server database
- xcopy deployment veya publish folder

---

## Entity Relationships

```csharp
Movement {
  Id (PK)
  VolunteerId (FK) → Volunteer.Id
  SourceSectorId (FK) → Sector.Id (ALAN_DIŞI for intikal)  
  TargetSectorId (FK) → Sector.Id (ÇIKIŞ for exit)
  MovementTime
  IsGroupMovement (bool)
  GroupId (nullable)
}
```

---

## Page Structure

### Required Pages
1. **Dashboard** - sayılar + quick stats
2. **Volunteers** - CRUD + list
3. **Movements** - kayıt formu + history
4. **Reports** - 3 temel rapor + export

### Navigation
```
Dashboard → Gönüllüler → Hareketler → Raporlar
```

---

## AI Agent Delivery Format

### Her Agent Çıktısı:
- **Kod files** (Controllers, Models, Views)
- **SQL scripts** (migrations, seed data)
- **Configuration** (appsettings, connection strings)
- **Deployment notes**

### Testing Strategy
- Manual testing (3 gün için unit test yok)
- Happy path scenarios
- Data validation

---

## Risk Mitigations

**Yüksek Risk**: Excel data migration  
**Çözüm**: Staged import + validation

**Orta Risk**: Windows Server deployment  
**Çözüm**: IIS deployment checklist

**Düşük Risk**: EF Core performance  
**Çözüm**: Basit queries, indexing

---

## Success Criteria

✅ Gönüllü kayıt/düzenleme  
✅ Hareket kayıt (dropdown selection)  
✅ 3 temel rapor  
✅ Excel export  
✅ IIS'de çalışan production app  
✅ Excel verisi tamamen migrate  

**QR Code**: 4. gün+ (scope dışı)

---

## Next Steps

1. Clean Architecture template share
2. Excel dosya detay analizi
3. SQL Server connection string
4. Agent görevleri başlat

**Toplam**: 3 agent paralel çalışma + 24 saat development