# OpenSch – Papan Sekolah Terbuka

Aplikasi web yang membantu orang tua mencari sekolah dan membandingkan 2–3 sekolah lewat tabel angka dan penjelasan singkat, serta menampilkan peringkat literasi dan numerasi dengan konteks yang jelas. Seluruh data adalah **data demo (sintetis), bukan data resmi**.

## Tech Stack

- **Frontend:** React (Vite), React Router, Tailwind CSS
- **Backend:** ASP.NET Core (.NET 10), Clean Architecture
- **Database:** SQLite (sementara untuk pengembangan)

## Prasyarat

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) 20.19 atau lebih baru (disarankan LTS terbaru)
- Git

## Cara Menjalankan

1. Clone repository:

   ```bash
   git clone -b frontend https://github.com/BennayaNaru/MIA-Project.git
   cd <nama-repo>
   ```

2. Jalankan **frontend** (cukup langkah ini untuk melihat seluruh tampilan saat ini):

   ```bash
   cd OpenSch/src/Web/ClientApp
   npm install
   npm run dev
   ```

   Buka http://localhost:5173

## Progress Saat Ini

- [x] Landing page (Beranda)
- [x] Cari Sekolah (filter, tampilan kartu/tabel, modal detail)
- [x] Keranjang perbandingan dan halaman Bandingkan
- [x] Tentang Data (metodologi dan batasan data)
- [x] Halaman Masuk/Daftar
- [x] API katalog dan perbandingan sekolah (`/api/schools`, `/api/compare`)
- [x] Ringkasan perbandingan AI di `POST /api/compare/ai` dengan payload JSON terstruktur dan fallback deterministik
- [ ] Halaman Peringkat dan ringkasan otomatis dari backend

Catatan: seluruh data adalah sintetis. Untuk perbandingan AI, browser hanya mengirim ID sekolah; server mengambil metrik kanonis,
membentuk JSON, dan mengirimkannya ke penyedia AI yang dikonfigurasi. Respons AI selalu harus diperlakukan sebagai bantuan,
bukan fakta atau rekomendasi akhir.

## Konfigurasi AI (opsional)

Secara default, endpoint AI menggunakan fallback deterministik dan tidak menghubungi penyedia eksternal. Untuk mengaktifkan
penyedia dengan API yang kompatibel dengan Chat Completions, simpan rahasia di user-secrets atau environment variables - jangan
menuliskan kunci di `appsettings.json`:

```powershell
dotnet user-secrets set "AiComparison:Enabled" "true" --project OpenSch/src/Web/Web.csproj
dotnet user-secrets set "AiComparison:Endpoint" "https://your-provider.example/v1/chat/completions" --project OpenSch/src/Web/Web.csproj
dotnet user-secrets set "AiComparison:Model" "your-model" --project OpenSch/src/Web/Web.csproj
dotnet user-secrets set "AiComparison:ApiKey" "your-secret" --project OpenSch/src/Web/Web.csproj
```

## Tim

| Nama | Peran |
|---|---|
| <Nathaniel Dennis Yosafat Sidauruk> | <Peran> |
| <Bennaya Muhammad Caesar> | <Peran> |
| <Hadi Sumanjaya> | <Peran> |
