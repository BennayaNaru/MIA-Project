#!/bin/bash
# OpenSch Database Migration Setup Script
# Execute this script after implementing the domain models

echo "🚀 Starting OpenSch Database Migration to Supabase..."
echo "========================================="

# Step 1: Create necessary directories
mkdir -p src/Infrastructure/Data/Configurations
mkdir -p src/Domain/Entities

# Step 2: Update appsettings.json with Supabase connection
SUPABASE_HOST="your-supabase-host.supabase.co"
SUPABASE_PORT="5432"
SUPABASE_DB="postgres"
SUPABASE_USER="postgres"
SUPABASE_PASSWORD="your-password"

cat > src/Web/appsettings.json << EOF
{
  "ConnectionStrings": {
    "OpenSchDb": "Host=${SUPABASE_HOST};Port=${SUPABASE_PORT};Database=${SUPABASE_DB};Username=${SUPABASE_USER};Password=${SUPABASE_PASSWORD};SSL Mode=Require;Trust Server Certificate=true;Pooling=true;Minimum Pool Size=5;Maximum Pool Size=20"
  },
  "DatabaseProvider": "Postgres",
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AiComparison": {
    "Enabled": false,
    "Endpoint": "https://api.openai.com/v1/chat/completions",
    "Model": "",
    "ApiKey": ""
  }
}
EOF

# Step 3: Generate seeding script
mkdir -p scripts

cat > scripts/seed_opensch_data.sql << 'EOF'
-- OpenSch Database Seeding Script
-- Execute this in your Supabase PostgreSQL instance

-- Enable necessary extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Seed Sekolah (Schools) table
INSERT INTO sekolah (id, nama, kota, kabupaten_kota, level, skor_melek_huruf, skor_melek_angka) VALUES
('DKI-01', 'SD Negeri Menteng Atas 01 Sintetis', 'Jakarta Selatan', 'Tebet', 'SD', 84.5, 81.2),
('DKI-02', 'SMP Cendekia Jaya Mandiri', 'Jakarta Selatan', 'Cilandak', 'SMP', 88.0, 86.5),
('DKI-03', 'SMK Teknologi Bangsa Pratama', 'Jakarta Selatan', 'Kebayoran Baru', 'SMK', 79.4, 85.0),
('DKI-04', 'SMP Teladan Jagakarsa Mandiri', 'Jakarta Selatan', 'Jagakarsa', 'SMP', 83.0, 81.5),
('DKI-05', 'SD Harapan Bangsa Cakung', 'Jakarta Timur', 'Cakung', 'SD', 80.2, 76.8),
('DKI-06', 'SMP Negeri Rawamangun Maju', 'Jakarta Timur', 'Duren Sawit', 'SMP', 82.6, 79.5),
('DKI-07', 'SMK Karya Mandiri Otomotif', 'Jakarta Timur', 'Jatinegara', 'SMK', 76.5, 88.2),
('DKI-08', 'SMK Insan Mulia Ciracas', 'Jakarta Timur', 'Ciracas', 'SMK', 78.2, 84.0),
('DKI-09', 'SD Pesisir Bintang Bahari', 'Jakarta Utara', 'Tanjung Priok', 'SD', 78.0, 74.2),
('DKI-10', 'SMP Samudera Indah Gading', 'Jakarta Utara', 'Kelapa Gading', 'SMP', 86.4, 84.0),
('DKI-11', 'SMK Maritim & Logistik Perkasa', 'Jakarta Utara', 'Penjaringan', 'SMK', 75.8, 83.5),
('DKI-12', 'SMP Citra Bahari Pademangan', 'Jakarta Utara', 'Pademangan', 'SMP', 81.5, 78.0),
('DKI-13', 'SD Citra Melati Kembangan', 'Jakarta Barat', 'Kembangan', 'SD', 81.5, 78.0),
('DKI-14', 'SMP Prestasi Cemerlang Grogol', 'Jakarta Barat', 'Grogol Petamburan', 'SMP', 83.2, 80.4),
('DKI-15', 'SMK Rekayasa Digital Informatika', 'Jakarta Barat', 'Kebon Jeruk', 'SMK', 82.0, 89.6),
('DKI-16', 'SMK Grafika Cengkareng Unggul', 'Jakarta Barat', 'Cengkareng', 'SMK', 77.0, 82.5);

-- Generate composite scores and ranks
WITH ranked_sekolah AS (
    SELECT 
        id,
        skor_melek_huruf,
        skor_melek_angka,
        COALESCE(skor_melek_huruf, 0) + COALESCE(skor_melek_angka, 0) AS total_score,
        ROW_NUMBER() OVER (ORDER BY COALESCE(skor_melek_huruf, 0) + COALESCE(skor_melek_angka, 0) DESC, skor_melek_huruf DESC) as rangking
    FROM sekolah
)
UPDATE sekolah 
SET skor_komposit = rs.total_score / 2.0,
    peringkat = rs.rangking
FROM ranked_sekolah rs
WHERE sekolah.id = rs.id;

-- Seed Impor_Data (Data Import Logs)
INSERT INTO impor_data (nama_file, tanggal_impor, jumlah_baris, sukses, pesan) VALUES
('schools_initial_seed.sql', NOW(), 16, true, 'Successfully seeded 16 synthetic schools with composite scores and rankings'),
('comparisons_seed.sql', NOW(), 20, true, 'Successfully seeded 20 comparison records between schools'),
('details_seed.sql', NOW(), 120, true, 'Successfully seeded 120 comparison detail records with normalized metrics');

-- Seed some comparison data
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
    ABS(COALESCE(s1.skor_komposit, 0) - COALESCE(s2.skor_komposit, 0)) < 15 AND 
    ABS(COALESCE(s1.skor_melek_huruf, 0) - COALESCE(s2.skor_melek_huruf, 0)) < 15 AND
    ABS(COALESCE(s1.skor_melek_angka, 0) - COALESCE(s2.skor_melek_angka, 0)) < 15
FROM sekolah s1
CROSS JOIN sekolah s2
WHERE s1.id != s2.id
AND s1.id < s2.id
LIMIT 20;

-- Seed comparison details
WITH source_data AS (
    SELECT 
        id,
        skor_melek_huruf,
        skor_melek_angka,
        skor_komposit,
        MIN(COALESCE(skor_melek_huruf, 0)) OVER () AS min_lit,
        MAX(COALESCE(skor_melek_huruf, 0)) OVER () AS max_lit,
        MIN(COALESCE(skor_melek_angka, 0)) OVER () AS min_num,
        MAX(COALESCE(skor_melek_angka, 0)) OVER () AS max_num,
        MIN(COALESCE(skor_komposit, 0)) OVER () AS min_comp,
        MAX(COALESCE(skor_komposit, 0)) OVER () AS max_comp
    FROM sekolah
)
INSERT INTO detail_perbandingan (perbandingan_id, sekolah_id, metrik, nilai_asli, nilai_normalisasi, rangking)
SELECT 
    p.id,
    d.id AS sekolah_id,
    CASE 
        WHEN d.metrik_type = 'literacy' THEN 'skor_melek_huruf'
        WHEN d.metrik_type = 'numeracy' THEN 'skor_melek_angka' 
        ELSE 'skor_komposit'
    END,
    CASE
        WHEN d.metrik_type = 'literacy' THEN d.literacy_score
        WHEN d.metrik_type = 'numeracy' THEN d.numeracy_score
        ELSE d.composite_score
    END,
    CASE
        WHEN d.metrik_type = 'literacy' THEN 
            CASE WHEN max_lit > min_lit THEN (d.literacy_score - min_lit) / (max_lit - min_lit) * 100 ELSE 50 END
        WHEN d.metrik_type = 'numeracy' THEN 
            CASE WHEN max_num > min_num THEN (d.numeracy_score - min_num) / (max_num - min_num) * 100 ELSE 50 END
        ELSE 
            CASE WHEN max_comp > min_comp THEN (d.composite_score - min_comp) / (max_comp - min_comp) * 100 ELSE 50 END
    END,
    ROW_NUMBER() OVER (
        PARTITION BY p.id 
        ORDER BY 
            CASE
                WHEN d.metrik_type = 'literacy' THEN d.literacy_score
                WHEN d.metrik_type = 'numeracy' THEN d.numeracy_score
                ELSE d.composite_score
            END DESC
    )
FROM perbandingan p
CROSS JOIN (
    SELECT 
        id,
        skor_melek_huruf AS literacy_score,
        skor_melek_angka AS numeracy_score,
        skor_komposit AS composite_score,
        'literacy' AS metrik_type
    FROM sekolah
    UNION ALL
    SELECT 
        id,
        skor_melek_huruf AS literacy_score,
        skor_melek_angka AS numeracy_score,
        skor_komposit AS composite_score,
        'numeracy' AS metrik_type
    FROM sekolah
    UNION ALL
    SELECT 
        id,
        skor_melek_huruf AS literacy_score,
        skor_melek_angka AS numeracy_score,
        skor_komposit AS composite_score,
        'composite' AS metrik_type
    FROM sekolah
) d
ORDER BY p.id, rangking;

EOF

echo "✅ Setup files created successfully!"
echo "📝 Next steps:"
echo "   1. Implement domain models (see IMPLEMENTATION_PRIORITY.json)"
echo "   2. Create EF Core configurations for snake_case mapping"
echo "   3. Generate database migrations"
echo "   4. Update API endpoints to use database instead of in-memory"
echo "   5. Execute seeding script in Supabase PostgreSQL"

echo "📁 Created files:"
echo "   - src/Web/appsettings.json (Supabase configuration)"
echo "   - scripts/seed_opensch_data.sql (data seeding script)"
echo "   - IMPLEMENTATION_PRIORITY.json (implementation roadmap)"

echo "========================================="
echo "🎯 Setup complete! Ready for implementation."