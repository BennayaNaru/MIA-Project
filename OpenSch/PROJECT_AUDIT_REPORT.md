# OPEN SCH PROJECT AUDIT REPORT

## Executive Summary

**Status**: ⚠️ **CRITICAL - 4/5 Required Tables Missing**

The OpenSch ASP.NET Core backend is partially implemented with PostgreSQL support but missing critical database entities for the Sekolah application. The project has infrastructure for Supabase but lacks the actual database schema for the required tables.

## Current State Analysis

### ✅ Existing Strengths

1. **EF Core PostgreSQL Infrastructure**
   - `Npgsql.EntityFrameworkCore.PostgreSQL` already in `Infrastructure.csproj`
   - PostgreSQL provider configured in `DependencyInjection.cs`
   - DatabaseProvider="Postgres" in `AppHost/Program.cs`

2. **Identity Management**
   - `ApplicationUser` entity for `pengguna` (Users) table
   - ASP.NET Core Identity integration
   - Role-based authorization with "Administrator" role

3. **Modern Architecture**
   - Clean architecture with separation of concerns
   - Aspire distributed application setup
   - Minimal API endpoints with IEndpointGroup pattern
   - Comprehensive error handling and validation

### ❌ Critical Issues Found

#### 1. Missing Database Schema (4/5 Tables)
   - ❌ **Sekolah (Schools)**: No entity model, no database table
   - ❌ **Perbandingan (Comparisons)**: No entity model, no database table
   - ❌ **Detail_Perbandingan (Comparison Details)**: No entity model, no database table
   - ❌ **Impor_Data (Data Import Logs)**: No entity model, no database table
   - ✅ **Pengguna (Users)**: Implemented via ASP.NET Core Identity

#### 2. Configuration Issues
   - ❌ No Supabase connection string in `appsettings.json`
   - ❌ SQLite configuration is the current default (`Data Source=OpenSchDb.db`)
   - ❌ No snake_case table/column mappings for PostgreSQL

#### 3. Missing Functionality
   - ❌ No fallback logic for null literacy/numeracy metrics
   - ❌ No synthetic school data seeding script
   - ❌ No database migrations for required tables

## Technical Details

### Current Database Setup
```sql
-- Existing: ASP.NET Core Identity tables (AspNetUsers, AspNetRoles, etc.)
-- Missing: 4 additional required tables
```

### Current Configuration
```json
{
  "ConnectionStrings": {
    "OpenSchDb": "Data Source=OpenSchDb.db"
  }
}
```

### Required PostgreSQL Schema

#### 1. `sekolah` (Schools)
```sql
CREATE TABLE IF NOT EXISTS sekolah (
    id VARCHAR(20) PRIMARY KEY,
    nama VARCHAR(255) NOT NULL,
    kota VARCHAR(100) NOT NULL,
    kabupaten_kota VARCHAR(100) NOT NULL,
    level VARCHAR(20) NOT NULL,
    skor_melek_huruf DECIMAL(5,2),
    skor_melek_angka DECIMAL(5,2),
    skor_komposit DECIMAL(5,2),
    peringkat INTEGER,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);
```

#### 2. `perbandingan` (Comparisons)
```sql
CREATE TABLE IF NOT EXISTS perbandingan (
    id BIGSERIAL PRIMARY KEY,
    sekolah_sumber_id VARCHAR(20) NOT NULL,
    sekolah_tujuan_id VARCHAR(20) NOT NULL,
    tanggal_perbandingan TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    metode_perbandingan VARCHAR(50) NOT NULL,
    pemimpin_huruf_id VARCHAR(20),
    pemimpin_angka_id VARCHAR(20),
    pemimpin_komposit_id VARCHAR(20),
    rentang_huruf DECIMAL(5,2),
    rentang_angka DECIMAL(5,2),
    perbandingan_adil BOOLEAN,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_sekolah_sumber FOREIGN KEY (sekolah_sumber_id) REFERENCES sekolah(id),
    CONSTRAINT fk_sekolah_tujuan FOREIGN KEY (sekolah_tujuan_id) REFERENCES sekolah(id)
);
```

#### 3. `detail_perbandingan` (Comparison Details)
```sql
CREATE TABLE IF NOT EXISTS detail_perbandingan (
    id BIGSERIAL PRIMARY KEY,
    perbandingan_id BIGINT NOT NULL,
    sekolah_id VARCHAR(20) NOT NULL,
    metrik VARCHAR(50) NOT NULL,
    nilai_asli DECIMAL(5,2),
    nilai_normalisasi DECIMAL(5,2),
    rangking INTEGER,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_perbandingan FOREIGN KEY (perbandingan_id) REFERENCES perbandingan(id),
    CONSTRAINT fk_sekolah_detail FOREIGN KEY (sekolah_id) REFERENCES sekolah(id)
);
```

#### 4. `impor_data` (Data Import Logs)
```sql
CREATE TABLE IF NOT EXISTS impor_data (
    id BIGSERIAL PRIMARY KEY,
    nama_file VARCHAR(255) NOT NULL,
    tanggal_impor TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    jumlah_baris INTEGER NOT NULL,
    sukses BOOLEAN NOT NULL,
    pesan TEXT,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);
```

## Solution Plan

### Phase 1: Infrastructure Setup (Days 1-3)

#### 1.1 Update Configuration
```json
{
  "ConnectionStrings": {
    "OpenSchDb": "Host=your-supabase-host.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=your-password;SSL Mode=Require;Trust Server Certificate=true;Pooling=true;Minimum Pool Size=5;Maximum Pool Size=20"
  },
  "DatabaseProvider": "Postgres",
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

#### 1.2 Implement Fallback Logic
Create a `MetricsCalculator` service with null-safe calculations:
```csharp
public class MetricsCalculator
{
    public decimal GetSafeLiteracyScore(decimal? score) => score ?? 0;
    public decimal GetSafeNumeracyScore(decimal? score) => score ?? 0;
    public decimal GetSafeCompositeScore(decimal? literacy, decimal? numeracy) 
    {
        var lit = literacy ?? 0;
        var num = numeracy ?? 0;
        return Math.Round((lit + num) / 2, 1);
    }
}
```

### Phase 2: Database Schema Implementation (Days 4-10)

#### 2.1 Create Domain Models
```csharp
// Sekolah Entity
public class Sekolah : BaseAuditableEntity
{
    public string Id { get; set; } = string.Empty;
    public string Nama { get; set; } = string.Empty;
    public string Kota { get; set; } = string.Empty;
    public string KabupatenKota { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public decimal? SkorMelekHuruf { get; set; }
    public decimal? SkorMelekAngka { get; set; }
    public decimal? SkorKomposit { get; set; }
    public int? Peringkat { get; set; }
}

// Perbandingan Entity
public class Perbandingan : BaseAuditableEntity
{
    public string SekolahSumberId { get; set; } = string.Empty;
    public string SekolahTujuanId { get; set; } = string.Empty;
    public DateTimeOffset TanggalPerbandingan { get; set; } = DateTimeOffset.UtcNow;
    public string MetodePerbandingan { get; set; } = string.Empty;
    public string? PemimpinHurufId { get; set; }
    public string? PemimpinAngkaId { get; set; }
    public string? PemimpinKompositId { get; set; }
    public decimal? RentangHuruf { get; set; }
    public decimal? RentangAngka { get; set; }
    public bool? PerbandinganAdil { get; set; }
}

// DetailPerbandingan Entity
public class DetailPerbandingan : BaseEntity
{
    public long PerbandinganId { get; set; }
    public string SekolahId { get; set; } = string.Empty;
    public string Metrik { get; set; } = string.Empty;
    public decimal? NilaiAsli { get; set; }
    public decimal? NilaiNormalisasi { get; set; }
    public int? Rangking { get; set; }
}

// ImporData Entity
public class ImporData : BaseEntity
{
    public string NamaFile { get; set; } = string.Empty;
    public DateTimeOffset TanggalImpor { get; set; } = DateTimeOffset.UtcNow;
    public int JumlahBaris { get; set; }
    public bool Sukses { get; set; }
    public string? Pesan { get; set; }
}
```

#### 2.2 EF Core Configuration (Snake_Case)
```csharp
public class SekolahConfiguration : IEntityTypeConfiguration<Sekolah>
{
    public void Configure(EntityTypeBuilder<Sekolah> builder)
    {
        builder.ToTable("sekolah");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnName("id");
        builder.Property(s => s.Nama).HasColumnName("nama").IsRequired();
        builder.Property(s => s.Kota).HasColumnName("kota").IsRequired();
        builder.Property(s => s.KabupatenKota).HasColumnName("kabupaten_kota").IsRequired();
        builder.Property(s => s.Level).HasColumnName("level").IsRequired();
        
        // Nullable fields for metrics
        builder.Property(s => s.SkorMelekHuruf).HasColumnName("skor_melek_huruf");
        builder.Property(s => s.SkorMelekAngka).HasColumnName("skor_melek_angka");
        builder.Property(s => s.SkorKomposit).HasColumnName("skor_komposit");
        builder.Property(s => s.Peringkat).HasColumnName("peringkat");
    }
}
```

#### 2.3 Generate Database Migrations
```bash
dotnet ef migrations add InitialCreate --project src/Infrastructure/Infrastructure.csproj
```

### Phase 3: Data Seeding (Day 11)

#### 3.1 Seeding Script
```sql
-- Insert initial 16 schools (matching existing SchoolCatalog.cs)
INSERT INTO sekolah (id, nama, kota, kabupaten_kota, level, skor_melek_huruf, skor_melek_angka)
VALUES 
('DKI-01', 'SD Negeri Menteng Atas 01 Sintetis', 'Jakarta Selatan', 'Tebet', 'SD', 84.5, 81.2),
('DKI-02', 'SMP Cendekia Jaya Mandiri', 'Jakarta Selatan', 'Cilandak', 'SMP', 88.0, 86.5),
-- ... (continue with all 16 schools)
;

-- Generate composite scores and rankings
WITH ranked_sekolah AS (
    SELECT 
        id,
        skor_melek_huruf,
        skor_melek_angka,
        COALESCE(skor_melek_huruf, 0) + COALESCE(skor_melek_angka, 0) AS total_score,
        ROW_NUMBER() OVER (ORDER BY COALESCE(skor_melek_huruf, 0) + COALESCE(skor_melek_angka, 0) DESC) as rangking
    FROM sekolah
)
UPDATE sekolah 
SET skor_komposit = rs.total_score / 2.0,
    peringkat = rs.rangking
FROM ranked_sekolah rs
WHERE sekolah.id = rs.id;
```

#### 3.2 Seed Comparison Data
```sql
-- Generate comparison records between school pairs
INSERT INTO perbandingan (sekolah_sumber_id, sekolah_tujuan_id, metode_perbandingan, pemimpin_huruf_id, pemimpin_angka_id, pemimpin_komposit_id, rentang_huruf, rentang_angka, perbandingan_adil)
SELECT 
    s1.id,
    s2.id,
    'deterministic',
    CASE WHEN s1.skor_melek_huruf >= s2.skor_melek_huruf THEN s1.id ELSE s2.id END,
    CASE WHEN s1.skor_melek_angka >= s2.skor_melek_angka THEN s1.id ELSE s2.id END,
    GREATEST(COALESCE(s1.skor_komposit, 0), COALESCE(s2.skor_komposit, 0)) > LEAST(COALESCE(s1.skor_komposit, 0), COALESCE(s2.skor_komposit, 0)) ? 
        (CASE WHEN s1.skor_komposit > s2.skor_komposit THEN s1.id ELSE s2.id END) : NULL,
    ABS(COALESCE(s1.skor_melek_huruf, 0) - COALESCE(s2.skor_melek_huruf, 0)),
    ABS(COALESCE(s1.skor_melek_angka, 0) - COALESCE(s2.skor_melek_angka, 0)),
    ABS(COALESCE(s1.skor_melek_huruf, 0) - COALESCE(s2.skor_melek_huruf, 0)) < 15 AND 
    ABS(COALESCE(s1.skor_melek_angka, 0) - COALESCE(s2.skor_melek_angka, 0)) < 15 AND
    ABS(COALESCE(s1.skor_komposit, 0) - COALESCE(s2.skor_komposit, 0)) < 15
FROM sekolah s1
CROSS JOIN sekolah s2
WHERE s1.id != s2.id
AND s1.id < s2.id
LIMIT 20;
```

### Phase 4: API Integration (Day 12-14)

#### 4.1 Update Endpoints
- Modify `Schools` endpoint to query from database instead of in-memory
- Update `Comparisons` endpoint to use database comparison logic
- Add new endpoints for CRUD operations on schools

#### 4.2 Integration Tests
```csharp
[IntegrationTest]
public class SekolahApiTests
{
    [Fact]
    public async Task GetSchools_ReturnsDataFromDatabase()
    {
        // Test that schools endpoint now queries from Supabase
    }
    
    [Fact]
    public async Task CreateComparison_UsesDatabaseLogic()
    {
        // Test that comparisons use database instead of in-memory
    }
}
```

## Risk Mitigation

### 1. Database Migration Risks
- **Solution**: Implement gradual migration - keep SQLite fallback during transition
- **Strategy**: Blue-green deployment with feature flags

### 2. Data Loss Risks
- **Solution**: Implement comprehensive backup strategy
- **Strategy**: Daily database snapshots, data validation before production cutover

### 3. Performance Risks
- **Solution**: Implement proper indexing and connection pooling
- **Strategy**: Monitor Supabase connection pool metrics, implement caching

## Immediate Actions Required

1. **High Priority** (Critical for migration):
   - Update `appsettings.json` with Supabase connection string
   - Create domain models for all 4 missing tables
   - Generate EF Core migrations

2. **Medium Priority** (Enhancement):
   - Implement fallback logic for null metrics
   - Create comprehensive seeding script
   - Update API endpoints to use database

3. **Low Priority** (Nice to have):
   - Performance optimization
   - Advanced monitoring
   - Multi-environment configuration

## Expected Timeline

- **Week 1**: Infrastructure setup, configuration updates
- **Week 2**: Database schema implementation, migrations
- **Week 3**: Data seeding, API integration
- **Week 4**: Testing, deployment, monitoring

## Success Criteria

1. ✅ All 5 required tables exist in Supabase database
2. ✅ Tables use proper snake_case naming conventions
3. ✅ Null literacy/numeracy metrics handled gracefully
4. ✅ Synthetic school data successfully seeded
5. ✅ APIs return data from database (not in-memory)
6. ✅ Connection pooling configured for Supabase
7. ✅ SSL/TLS security enforced for database connections

---

**Recommendation**: Proceed with Phase 1 implementation immediately to establish database connectivity, followed by systematic implementation of remaining components.